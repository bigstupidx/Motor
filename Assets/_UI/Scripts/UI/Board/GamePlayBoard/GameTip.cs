//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HeavyDutyInspector;

namespace GameUI {
	public class GameTip : Singleton<GameTip> {
		[ComplexHeader("漂移 氮气", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public GameObject DriftTip;
		public GameObject BoostTip;
		public Text DriftText;
		public Text BoostText;
		public float IntervalDis = 2f;

		private float _lastDriftDis;
		private float _lastBoostDis;
		private TweenScale _driftScale;
		private TweenScale _boostScale;
		private TweenCanvasGroupAlpha _driftAlpha;
		private TweenCanvasGroupAlpha _boostAlpha;
		private bool _driftShow;
		private bool _boostShow;
		private float _timer;
		private float _intervial = 0.05f;

		[ComplexHeader("道具", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public GameObject PropTip;
		public Image PropImage;

		private string _usePropPath = "UI/Modules/Menu/GamePlayBoard(Clone)/BtnPlayMode/BtnGameProp";
		private GameObject _usePropBtn;
		private TweenPosition _propShowPosiion;
		private Coroutine _propMoveCoroutine;
		private Coroutine _propCloseCoroutine;

		[ComplexHeader("任务", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public GameObject TaskTip;
		public Text TaskInfoText;
		public Text TaskProgressText;

		private TweenPosition _taskPosition;
		private CanvasGroup _taskCanvas;
		private List<bool> _taskResult;

		[ComplexHeader("击杀", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public KillEnermyTip KillEnermyTip;
		private int _killCount;

		[ComplexHeader("被锁定", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public GameObject LockedTip;
		public Text LockedText;
		public GameObject LockedUsePropText;

		private TweenCanvasGroupAlpha _lockedAlpha;
		private Coroutine _lockedCoroutine;

		[ComplexHeader("游戏结束", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public GameObject GameOverTip;
		public Text GameOverText;
		public Text GameOverTextRank;
		public TweenCanvasGroupAlpha[] GameOverAlphas;
		public TweenPosition GameOverPosition;

		[ComplexHeader("计时赛", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public GameObject RecoverTimeTip;
		public Text RecoverTimeText;
		public Text RecoverTimeValueText;
		public Text RecoverTimePerfectText;

		public TweenPosition _recoverTimePosition;
		public TweenScale _recoverTimeScale;
		public TweenCanvasGroupAlpha _recoverTimeAlpha;

		private bool isGameOver = false;

		#region Reset
		void Reset() {
			isGameOver = false;
			// 漂移
			DriftTip.SetActive(true);
			_driftScale = DriftText.transform.GetComponent<TweenScale>();
			_driftAlpha = DriftTip.GetComponent<TweenCanvasGroupAlpha>();
			DriftTip.GetComponent<CanvasGroup>().alpha = 0;
			_driftShow = false;

			// 氮气
			BoostTip.SetActive(true);
			_boostScale = BoostText.transform.GetComponent<TweenScale>();
			_boostAlpha = BoostTip.GetComponent<TweenCanvasGroupAlpha>();
			BoostTip.GetComponent<CanvasGroup>().alpha = 0;
			_boostShow = false;

			// 道具
			PropTip.SetActive(false);

			// 任务
			TaskTip.SetActive(false);
			_taskPosition = TaskTip.GetComponent<TweenPosition>();
			_taskCanvas = TaskTip.GetComponent<CanvasGroup>();
			_taskResult = new List<bool>() { false, false, false };

			// 击杀
			KillEnermyTip.Reset();
			_killCount = 0;

			// 被锁定
			LockedTip.SetActive(false);
			LockedUsePropText.SetActive(false);

			// 游戏结束
			GameOverTip.SetActive(false);

			// 计时赛
			RecoverTimeTip.SetActive(false);
			//			_recoverTimePosition = RecoverTimeText.GetComponent<TweenPosition>();
			//			_recoverTimeScale = RecoverTimeText.GetComponent<TweenScale>();
			//			_recoverTimeAlpha = RecoverTimeText.GetComponent<TweenCanvasGroupAlpha>();

			#region Locked Tip

			BikeManager.Ins.CurrentBike.bikeAttack.OnBeLocked += (bike, type) => {
				var str = Client.Prop.GetDataByType(type).Name;
				LockedText.text = (LString.GAMEUI_GAMETIP_RESET).ToLocalized() + str + (LString.GAMEUI_GAMETIP_RESET_1).ToLocalized();

				LockedTip.SetActive(true);
				if (_lockedAlpha != null) _lockedAlpha.ResetToBeginning();
				_lockedAlpha = TweenCanvasGroupAlpha.Begin(LockedTip, 0.2f, 1f);
				if (_lockedCoroutine != null) StopCoroutine(_lockedCoroutine);

				// 自动使用护盾
				var player = BikeManager.Ins.CurrentBike;
				// 优先使用捡到的
				var propCatch = player.bikeProp.PropSlots.Has(p => p.Type == PropType.Shield);
				if (propCatch != null) {
					LockedUsePropText.SetActive(true);
					GamePlayBoard.Ins.GameControl.BtnGameProp.OnClick();
					//					Debug.Log("使用 道具 护盾");
				} else if (Client.User.UserInfo.EquipedPropList.Count > 0) {
					var propEquip = Client.User.UserInfo.EquipedPropList.Has(id => Client.Prop[id].PropType == PropType.Shield);
					if (propEquip != 0) {
						LockedUsePropText.SetActive(true);
						GamePlayBoard.Ins.GameControl.BtnEquipProp.OnClick();
						//						Debug.Log("使用 装备道具 护盾");
					}
				}
			};
			BikeManager.Ins.CurrentBike.bikeAttack.OnBeLockedCancel += () => {
				LockedUsePropText.SetActive(false);
				_lockedAlpha = TweenCanvasGroupAlpha.Begin(LockedTip, 0.2f, 0f);
				if (!this.gameObject.activeSelf) return;
				_lockedCoroutine = this.DelayInvoke(() => {
					LockedTip.SetActive(false);
				}, 0.2f);
			};

			#endregion
		}
		#endregion

		#region Enable / Disable
		void OnEnable() {
			Reset();
			Client.EventMgr.AddListener(EventEnum.Game_GainProp, OnGainPropEvent);
			Client.EventMgr.AddListener(EventEnum.Game_KillEnemy, ShowKillEnermyTip);

			BikeManager.Ins.CurrentBike.racerInfo.OnFinish += OnGameOver;

			if (RaceManager.Ins.RaceMode == RaceMode.Timing) {
				GameModeTiming.Ins.OnRecoverTime += OnRecoverTime;
			}

			if (GameModeBase.Ins.Match.MatchMode == MatchMode.Online || GameModeBase.Ins.Match.MatchMode == MatchMode.OnlineRandom) {
				return;
			}
			var tasks = GameModeBase.Ins.Tasks;
			foreach (var task in tasks) {
				task.TaskChecker.OnTaskComplete += OnTaskComplete;
			}
		}

		void OnDisable() {
			StopAllCoroutines();
			Client.EventMgr.RemoveListener(EventEnum.Game_GainProp, OnGainPropEvent);
			Client.EventMgr.RemoveListener(EventEnum.Game_KillEnemy, ShowKillEnermyTip);

			if (RaceManager.Ins != null && BikeManager.Ins.CurrentBike != null) {
				BikeManager.Ins.CurrentBike.racerInfo.OnFinish -= OnGameOver;
			}

			if (RaceManager.Ins != null && GameModeBase.Ins != null && RaceManager.Ins.RaceMode == RaceMode.Timing) {
				GameModeTiming.Ins.OnRecoverTime -= OnRecoverTime;
			}

			if (RaceManager.Ins == null || GameModeBase.Ins == null 
				|| GameModeBase.Ins.Match.MatchMode == MatchMode.Online
			    || GameModeBase.Ins.Match.MatchMode == MatchMode.OnlineRandom) {
				return;
			}
			var tasks = GameModeBase.Ins.Tasks;
			foreach (var task in tasks) {
				task.TaskChecker.OnTaskComplete -= OnTaskComplete;
			}
		}
		#endregion

		#region RecoverTime Tip
		public void OnRecoverTime(bool isPerfect, float recoverValue) {
			RecoverTimeTip.SetActive(true);
			RecoverTimePerfectText.gameObject.SetActive(isPerfect);
			RecoverTimeValueText.text = "+ " + (int)recoverValue + " s";
			_recoverTimePosition.ResetToBeginning();
			_recoverTimePosition.PlayForward();
			_recoverTimeAlpha.ResetToBeginning();
			_recoverTimeAlpha.PlayForward();
			_recoverTimeScale.ResetToBeginning();
			_recoverTimeScale.PlayForward();
		}
		#endregion

		#region KillEnemy Tip
		public void ShowKillEnermyTip(EventEnum eventEnum, object[] args) {
			_killCount++;
			KillEnermyTip.Reset();
			KillEnermyTip.SetCount(_killCount);
			KillEnermyTip.PlayForward();
		}
		#endregion

		#region Prop Tip
		protected void OnGainPropEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 1) {
				var type = (PropType)args[0];
				var prop = Client.Prop.GetDataByType(type);
				if (prop == null) {
					return;
				}

				stopPropCoroutines();

				PropImage.sprite = prop.Icon.Sprite;
				PropTip.SetActive(true);
				PropTip.transform.localScale = Vector3.zero;
				//				if (_propShowPosiion != null) _propShowPosiion.ResetToBeginning();
				//				PropTip.transform.position = Vector3.zero;
				_usePropBtn = GameObject.Find(_usePropPath);
				if (_usePropBtn != null) {
					PropTip.transform.position = _usePropBtn.transform.position;
					//					_propShowPosiion = TweenPosition.Begin(PropTip, 0.2f, _usePropBtn.transform.position, true);
				}
				TweenScale.Begin(PropTip, 0.2f, Vector3.one);
				_propMoveCoroutine = this.DelayInvoke(() => {

					PropTip.transform.localScale = Vector3.one;
					TweenScale.Begin(PropTip, 0.2f, Vector3.zero);
					stopPropCoroutines();
					_propCloseCoroutine = this.DelayInvoke(() => {
						//						_propShowPosiion.ResetToBeginning();
						PropTip.transform.position = Vector3.zero;
						PropTip.SetActive(false);
					}, 0.2f);
				}, 0.7f);
			}
		}

		private void stopPropCoroutines() {
			if (_propMoveCoroutine != null) StopCoroutine(_propMoveCoroutine);
			if (_propCloseCoroutine != null) StopCoroutine(_propCloseCoroutine);
		}

		#endregion

		#region Task Tip
		protected void OnTaskComplete() {
			for (var i = 0; i < 3; i++) {
				var task = GameModeBase.Ins.Tasks[i];
				if (task.State == TaskState.Completed && !_taskResult[i]) {
					_taskResult[i] = true;
					TaskInfoText.text = task.Data.Description;
					TaskProgressText.text = task.ProgressStr;
					TaskTip.SetActive(true);
					_taskCanvas.alpha = 1;
					_taskPosition.ResetToBeginning();
					_taskPosition.PlayForward();
					this.DelayInvoke(() => {
						TweenCanvasGroupAlpha.Begin(TaskTip, 0.2f, 0f);
					}, 3f);
					break;
				}
			}
		}
		#endregion

		#region GameOver Tip
		public void OnGameOver(BikeBase self) {
			isGameOver = true;
			GameOverTip.SetActive(true);
			if (RaceManager.Ins.RaceMode == RaceMode.Timing) {
				if (BikeManager.Ins.CurrentBike.racerInfo.IsCompleteGame) {
					GameOverText.text = (LString.GAMEUI_GAMETIP_ONGAMEOVER).ToLocalized() + GameModeTiming.Ins.TimeLeft.ToString("f2") + (LString.GAMEUI_GAMETIP_ONGAMEOVER_1).ToLocalized();
				} else {
					GameOverText.text = (LString.GAMEUI_GAMETIP_ONGAMEOVER_2).ToLocalized();
				}
				GameOverTextRank.text = "";
			} else {
				GameOverText.text = (LString.GAMEUI_GAMETIP_ONGAMEOVER_5).ToLocalized();
				GameOverTextRank.text = self.racerInfo.Rank.ToString();
			}
			foreach (TweenCanvasGroupAlpha GameOverAlpha in GameOverAlphas) {
				GameOverAlpha.ResetToBeginning();
				GameOverAlpha.PlayForward();
			}

			GameOverPosition.ResetToBeginning();
			GameOverPosition.PlayForward();
		}
		#endregion

		void Update() {
			if (DeviceLevel.Score < 20 ) {
				DriftTip.SetActive(false);
				BoostTip.SetActive(false);
				return;
			}

			_timer += Time.deltaTime;
			if (_timer < _intervial) {
				return;
			}
			_timer = 0;
			if (isGameOver) {
				DriftTip.SetActive(false);
				BoostTip.SetActive(false);
				return;
			}
			// 漂移
			var drift = BikeManager.Ins.CurrentBike.bikeControl.DriftDis;
			if (drift > 0) DriftText.text = drift.ToString("f1");
			if (drift > 10) {
				if (!_driftShow) {
					_driftAlpha.PlayForward();
					_driftShow = true;
				}
				// TweenCanvasGroupAlpha.Begin(DriftTip, 0.2f, 1);
			} else {
				if (_driftShow) {
					_driftAlpha.PlayReverse();
					_driftShow = false;
				}
				// TweenCanvasGroupAlpha.Begin(DriftTip, 0.1f, 0);
				_lastDriftDis = 0;
			}
			//if (drift - _lastDriftDis >= IntervalDis)
			//{
			//	_lastDriftDis = drift;
			//	_driftScale.ResetToBeginning();
			//	_driftScale.PlayForward();
			//}
			// 氮气加速
			var boost = BikeManager.Ins.CurrentBike.bikeControl.BoostingDis;
			if (boost > 0) BoostText.text = boost.ToString("f1");
			if (boost > 10) {
				if (!_boostShow) {
					_boostAlpha.PlayForward();
					_boostShow = true;
				}
				// TweenCanvasGroupAlpha.Begin(BoostTip, 0.2f, 1);
			} else {
				if (_boostShow) {
					_boostAlpha.PlayReverse();
					_boostShow = false;
				}
				// TweenCanvasGroupAlpha.Begin(BoostTip, 0.1f, 0);
				_lastBoostDis = 0;
			}
			//if (boost - _lastBoostDis >= IntervalDis)
			//{
			//	_lastBoostDis = boost;
			//	_boostScale.ResetToBeginning();
			//	_boostScale.PlayForward();
			//}
		}
	}

}
