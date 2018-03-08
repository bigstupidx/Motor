//
// GameModeElimination.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using GameClient;

namespace Game {
	/// <summary>
	/// 游戏模式 - 淘汰赛
	/// </summary>
	public class GameModeElimination : GameModeBase<GameModeElimination> {
		/// <summary>
		/// 比赛进行时间
		/// </summary>
		public float TimeRun { get; private set; }
		/// <summary>
		/// 比赛开始后经过多久开始倒计时
		/// </summary>
		public float TimeWait { get; private set; }
		/// <summary>
		/// 淘汰玩家时间间隔
		/// </summary>
		public float TimeInterval { get; private set; }
		/// <summary>
		/// 本轮淘汰剩余时间
		/// 小于0为非淘汰时段，大于等于0为剩余时间
		/// </summary>
		public float TimeTurn { get; private set; }

		/// <summary>
		/// 开始淘汰玩家
		/// 等待预热时间结束，倒计时开始
		/// </summary>
		public Action OnEliminateStart = delegate { };
		/// <summary>
		/// 淘汰玩家                                                                                                         
		/// </summary>
		public Action<BikeBase> OnEliminate = delegate { };
		/// <summary>
		/// 玩家进入被淘汰状态
		/// 处于最后一名的玩家发生变化时触发                                                                                                         
		/// </summary>
		public Action<BikeBase> OnEliminateEnter = delegate { };
		/// <summary>
		/// 玩家离开被淘汰状态
		/// </summary>
		public Action<BikeBase> OnEliminateLeave = delegate { };

		private BikeBase _last;

		protected override void Awake() {
			base.Awake();
			// 初始化数据
			TimeRun = 0;
			TimeWait = 6f;
			TimeInterval = 14f;
			TimeTurn = -1;
		}

		private void Update() {
			if (RaceManager.Ins.GamePhase != GamePhase.Gaming || RaceManager.Ins.PlayerList.Count == 0) {
				return;
			}
			TimeRun += Time.deltaTime;
			// 等待一定时间后，开始进入淘汰计时
			if (TimeRun >= TimeWait && TimeTurn < 0) {
				TimeTurn = TimeInterval;
				OnEliminateStart();
			}
			if (TimeTurn > 0) {
				// 淘汰倒计时，每次归零时淘汰一位玩家
				TimeTurn -= Time.deltaTime;
				if (TimeTurn <= 0) {
					TimeTurn = TimeInterval;
					EliminateLast();
					return;
				}
			}
			// 切换即将被淘汰的玩家
			var nowLast = RaceManager.Ins.Last;
			if (nowLast != null) {
				var change = false;
				if (_last != null) {
					if (_last.bikeControl != nowLast.bikeControl) {
						OnEliminateLeave(_last);
						change = true;
					}
				} else {
					change = true;
				}
				if (change) {
					_last = nowLast;
					if (RaceManager.Ins.PlayerList.Count > 1) OnEliminateEnter(nowLast);
				}
			}
			if (RaceManager.Ins.PlayerList.Count == 1) {
				//				FinishGame();
				BikeManager.Ins.CurrentBike.racerInfo.DoFinish();

			}
		}

		/// <summary>
		/// 淘汰最后一人
		/// </summary>
		public void EliminateLast() {
			if (RaceManager.Ins.PlayerList.Count < 1) return;
			var bike = RaceManager.Ins.Last;
			bike.racerInfo.DoFinish();
			OnEliminate(bike);

			// 玩家被淘汰则游戏结束
			if (bike.CompareTag(Tags.Ins.Player)) {
				BikeManager.Ins.CurrentBike.racerInfo.DoFinish();
			} else {
				BikeManager.Ins.SetBikeActive(bike, false);
//				bike.gameObject.SetActive(false);
//				bike.bikeDriver.Driver.gameObject.SetActive(false);
			}
			bike.bikeState.Fsm.processEvent(BikeFSM.Event.Crash, BikeDeathType.Eliminated);
			var hud = bike.GetComponent<DisplayHud>();
			if (hud != null) {
				hud.enabled = false;
			}
		}

	}
}

