using System;
using System.Collections.Generic;
using UnityEngine;
using Game;
using GameClient;
using Joystick;
using Orbbec;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class GamePlayBoard : SingleUIStackBehaviour<GamePlayBoard> {
		public const string UIPrefabPath = "UI/Board/GamePlayBoard/GamePlayBoard";

		public const string RacingRaceInfo = "UI/Board/GamePlayBoard/RaceInfo/RaceInfo_Racing";
		public const string EliminationRaceInfo = "UI/Board/GamePlayBoard/RaceInfo/RaceInfo_Elimination";
		public const string TimingRaceInfo = "UI/Board/GamePlayBoard/RaceInfo/RaceInfo_Timing";

		public const string KillTimingRaceInfo = "UI/Board/GamePlayBoard/RaceInfo/RaceInfo_KilleTiming";
		public const string GameTip = "UI/Board/GamePlayBoard/GameTip";

		public static string[] GetUINames(GameMode mode) {
			switch (mode) {
				case GameMode.Racing:
				case GameMode.RacingProp:
					return new[] { UIPrefabPath, RacingRaceInfo };
				case GameMode.Elimination:
				case GameMode.EliminationProp:
					return new[] { UIPrefabPath, EliminationRaceInfo };
				case GameMode.Timing:
					return new[] { UIPrefabPath, TimingRaceInfo };
				default:
					return new[] { UIPrefabPath, RacingRaceInfo };
			}
		}

		public static void Show(bool destroyBefore = false) {
			Inited = false;

			var list = new List<string>(GetUINames(Client.Game.MatchInfo.Data.GameMode));
			list.Add(GameTip);
			ModMenu.Ins.Cover(list, "GamePlayBoard", destroyBefore);
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			Init();
		}

		public override void OnUILeaveStack() {
			base.OnUILeaveStack();
			if (OrbbecSensingManager.instance != null) {
				OrbbecSensingManager.instance.showTrackingUI = false;
			}
		}

		public static bool Inited;

		[NonSerialized]
		public UIRaceInfoBase RaceInfo;
		[NonSerialized]
		public UIGameControlBase GameControl;

		public GameObject BtnReset;
		public UIGameControlBase[] GameControls;

		public Image NitrogenProgress;
		public Image NitrogenProgressAdd;

		public GameObject MiniMap;

		private float _energyVal;

		public void OnEnable() {
			if (!Inited) {
				_energyVal = 0;
				NitrogenProgress.fillAmount = 0;
				NitrogenProgressAdd.fillAmount = 0;
			}
			try {
				BikeManager.Ins.CurrentBike.racerInfo.OnFinish += OnGameOver;
				if (MiniMapCamera.Ins != null) {
					MiniMapCamera.Ins.MiniMapUi = MiniMap;
				}
			} catch (Exception e) {
				Debug.LogException(e);
			}
		}

		public void OnDisable() {
			if (RaceManager.Ins != null && BikeManager.Ins.CurrentBike != null) {
				BikeManager.Ins.CurrentBike.racerInfo.OnFinish -= OnGameOver;
			}
		}

		public void Init() {
			SetControlMode(ModGame.GetControlMode());

			if (Inited) {
				return;
			}
			ShowBtnReset(false);
			RegisterBikeEvent();
		}

		public void UpdateCoin(int value) {
			RaceInfo.UpdateCoin(value);
		}

		public void SetControlMode(GameControlMode mode) {
			foreach (UIGameControlBase t in this.GameControls) {
				t.gameObject.SetActive(false);
			}
			int modeNum = (int)mode;
			this.GameControl = this.GameControls[modeNum];
			this.GameControls[modeNum].gameObject.SetActive(true);
		}

		public void ShowBtnReset(bool isShow) {
			BtnReset.SetActive(isShow);
		}

		public void ShowPropGetFromGame(PropType propType) {
			GameControl.BtnGameProp.SetProp(propType);
		}


		private void RegisterBikeEvent() {
			BikeManager.Ins.CurrentBike.racerInfo.OnWrongDirection += bike => {
				ShowBtnReset(true);
			};
			BikeManager.Ins.CurrentBike.racerInfo.OnRightDirection += bike => {
				ShowBtnReset(false);
			};
			BikeManager.Ins.CurrentBike.racerInfo.OnReset += bike => {
				ShowBtnReset(false);
			};
			BikeManager.Ins.CurrentBike.racerInfo.OnStuck += bike => {
				if (!bike.bikeHealth.IsAlive || RaceManager.Ins.GamePhase == GamePhase.Over) return;
				ShowBtnReset(true);
			};
			BikeManager.Ins.CurrentBike.racerInfo.OnUnStuck += bike => {
				ShowBtnReset(false);
			};
		}

		protected void Update() {
			var energy = BikeManager.Ins.CurrentBike.bikeControl.BoostingEnergy;

			if (energy > 100) {
				_energyVal = (energy - 100) / 100;

				if (Math.Abs(_energyVal - NitrogenProgressAdd.fillAmount) > 1e-6) {
					NitrogenProgressAdd.fillAmount = Mathf.MoveTowards(NitrogenProgressAdd.fillAmount, _energyVal, Time.deltaTime * 1f);
				}

				if (Math.Abs(NitrogenProgress.fillAmount - 1) > 1e-6) {
					NitrogenProgress.fillAmount = 1;
				}

			} else {
				_energyVal = energy / 100;

				if (Math.Abs(NitrogenProgressAdd.fillAmount) > 1e-6) {
					NitrogenProgressAdd.fillAmount = 0;
				}

				if (Math.Abs(_energyVal - NitrogenProgress.fillAmount) > 1e-6) {
					NitrogenProgress.fillAmount = Mathf.MoveTowards(NitrogenProgress.fillAmount, _energyVal, Time.deltaTime * 1f);
				}
			}
		}

		public void OnGameOver(BikeBase bikeBase) {
			gameObject.SetActive(false);
		}
	}

}

