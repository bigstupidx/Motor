using System;
using System.Collections;
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class UIRaceInfoElimination : UIRaceInfoBase {
		#region base

		public override void OnUIDespawn() {
			EliminatePlayerTipTW.gameObject.SetActive(false);
			UnRegisterEvent();
			base.OnUIDespawn();
		}

		#endregion

		public Text rank;
		public Text EliminatePlayerTip;
		public TweenPosition EliminatePlayerTipTW;
		public AudioSource SfxEliminateOver;

		protected int currentRank;//当前排名
		protected float lastUpdateRankTime;//上一次更新排名的时间
		protected bool isEliminateStart;
		protected bool isEliminatingSelf;

		protected override void Awake() {
			base.Awake();
			if (SfxManager.Ins != null) {
				SfxEliminateOver.outputAudioMixerGroup = SfxManager.Ins.SfxGroup;
			}
		}

		public override void Init() {
			base.Init();
			RegisterEliminationGameEvent();
			RaceTime.SetColor(Color.white);
			EliminatePlayerTipTW.gameObject.SetActive(false);
			if (GamePlayBoard.Inited) {
				return;
			}

			currentRank = 0;
			lastUpdateRankTime = 0;
			isEliminateStart = false;
			isEliminatingSelf = false;
			RaceTime.gameObject.SetActive(false);
			rank.text = RaceManager.Ins.PlayerNum + "/" + RaceManager.Ins.PlayerNum;

			GamePlayBoard.Inited = true;
		}

		public override void Update() {
			if (Time.frameCount % 2 != 0) {
				return;
			}
			base.Update();
			UpdateRank();
		}

		public override void UpdateRaceTime() {
			if (isEliminateStart) {
				RaceTime.SetTime(GameModeElimination.Ins.TimeTurn);
			}
			RaceTime.SetColor(this.isEliminatingSelf ? Color.red : Color.white);
		}

		public void UpdateRank() {
			if (currentRank != BikeManager.Ins.CurrentBike.racerInfo.Rank && Time.timeSinceLevelLoad - lastUpdateRankTime > 0.5f) {
				currentRank = BikeManager.Ins.CurrentBike.racerInfo.Rank;
				lastUpdateRankTime = Time.timeSinceLevelLoad;
				rank.text = currentRank.ToStringFast() + "/" + RaceManager.Ins.PlayerList.Count.ToStringFast();
			}
		}

		public void UpdateRanSum() {
			rank.text = currentRank.ToStringFast() + "/" + RaceManager.Ins.PlayerList.Count.ToStringFast();
		}

		public void ShowEliminateTip(int i, bool isSelf) {
			if (!EliminatePlayerTipTW.gameObject.activeSelf) {
				EliminatePlayerTipTW.gameObject.SetActive(true);
			}
			EliminatePlayerTip.text = isSelf ? (LString.GAMEUI_UIRACEINFOELIMINATION_SHOWELIMINATETIP).ToLocalized() : (LString.GAMEUI_UIRACEINFOELIMINATION_SHOWELIMINATETIP_1).ToLocalized() + i + (LString.GAMEUI_UIRACEINFOELIMINATION_SHOWELIMINATETIP_2).ToLocalized();
			EliminatePlayerTipTW.ResetToBeginning();
			EliminatePlayerTipTW.PlayForward();
		}

		private void RegisterEliminationGameEvent() {
			GameModeElimination.Ins.OnEliminateStart += OnEliminateStart;
			GameModeElimination.Ins.OnEliminateEnter += OnEliminateEnter;
			GameModeElimination.Ins.OnEliminateLeave += OnEliminateLeave;
			GameModeElimination.Ins.OnEliminate += OnEliminate;
		}

		private void UnRegisterEvent() {
			GameModeElimination.Ins.OnEliminateStart -= OnEliminateStart;
			GameModeElimination.Ins.OnEliminateEnter -= OnEliminateEnter;
			GameModeElimination.Ins.OnEliminateLeave -= OnEliminateLeave;
			GameModeElimination.Ins.OnEliminate -= OnEliminate;
		}

		private void OnEliminate(BikeBase bike) {
			try {//这里在真机上偶尔有一个奇怪的nullreference，暂时不确定在哪行，先try住
				if (bike.gameObject.CompareTag(Tags.Ins.Player)) {
					ShowEliminateTip(bike.racerInfo.Rank, true);
					return;
				}
				ShowEliminateTip(bike.racerInfo.Rank, false);
				UpdateRanSum();
				//			int i = BikeManager.Ins.Bikes.IndexOf(bike.racerInfo) - 1;
				//OtherRaceProgress[i].gameObject.SetActive(false);
				MiniMapTag tag = bike.GetComponent<MiniMapTag>();
				if (tag != null && tag.tagInstance != null) {
					tag.tagInstance.gameObject.SetActive(false);
				}
			} catch (Exception e) {
				Debug.LogError(e);
			}
		}

		private void OnEliminateLeave(BikeBase bike) {
			if (bike.gameObject.CompareTag(Tags.Ins.Player)) {
				// Debug.Log("==>is not eliminating self");
				if (_checkTimeAnim != null) {
					StopCoroutine(_checkTimeAnim);
				}
				isEliminatingSelf = false;

				SfxEliminateOver.Stop();
			}
		}

		private void OnEliminateEnter(BikeBase bike) {
			if (bike.gameObject.CompareTag(Tags.Ins.Player)) {
				// Debug.Log("==>is eliminating self");
				isEliminatingSelf = true;
				if (isEliminateStart) {
					var time = GameModeElimination.Ins.TimeTurn;
					var timeTarget = Mathf.FloorToInt(time);
					if (_checkTimeAnim != null) {
						StopCoroutine(_checkTimeAnim);
					}
					_checkTimeAnim = StartCoroutine(CheckTimeAnim(time - timeTarget));
					if (GameModeElimination.Ins.TimeTurn <= 3f && !SfxEliminateOver.isPlaying) {
						SfxEliminateOver.Play();
					}
				}
			}
		}

		private Coroutine _checkTimeAnim;
		IEnumerator CheckTimeAnim(float _targetTime) {
			yield return new WaitForSeconds(_targetTime);
			while (true) {
				if (isEliminatingSelf) {
					if (GameModeElimination.Ins.TimeTurn > 3f) {
						SfxManager.Ins.PlayOneShot(SfxType.SFX_Countdown_Elimination);
					} else if (GameModeElimination.Ins.TimeTurn <= 3f && !SfxEliminateOver.isPlaying) {
						SfxEliminateOver.Play();
					}
				}
				yield return new WaitForSeconds(1);
			}
		}

		private void OnEliminateStart() {
			isEliminateStart = true;
			RaceTime.gameObject.SetActive(true);
		}
	}




}
