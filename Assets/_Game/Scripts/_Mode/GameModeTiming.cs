//
// GameModeTiming.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

namespace Game {
	/// <summary>
	/// 游戏模式 - 计时赛
	/// </summary>
	public class GameModeTiming : GameModeBase<GameModeTiming> {

		/// <summary>
		/// 比赛进行时间
		/// </summary>
		public float TimeRun { get; private set; }

		/// <summary>
		/// 剩余时间
		/// </summary>
		public float TimeLeft;

		/// <summary>
		/// 检查点序号
		/// </summary>
		public int TimingCheckerIndex { get; private set; }

		/// <summary>
		/// 检查点
		/// </summary>
		public List<TimeCheckPoint> Checkers = new List<TimeCheckPoint>();

		/// <summary>
		/// 总检查点数<para/>
		/// 起点需要多通过1次
		/// </summary>
		public int CheckerCount {
			get { return Checkers.Count * Match.Data.Turn + 1; }
		}

		/// <summary>
		/// 通过计数
		/// </summary>
		public int PassCount { get; private set; }

		/// <summary>
		/// 时间恢复倍率
		/// </summary>
		public float RecoverRate { get; private set; }

		/// <summary>
		/// 恢复时间<para/>
		/// 参数 : 是否完美通过， 恢复量
		/// </summary>
		public Action<bool, float> OnRecoverTime = delegate { };

		/// <summary>
		/// 通过点序号
		/// </summary>
		private int _passIndex;
		/// <summary>
		/// 恢复点序号
		/// </summary>
		private int _recoverIndex;
		/// <summary>
		/// 回复量
		/// </summary>
		private float _recoverValue;
		/// <summary>
		/// 是否完美通过
		/// </summary>
		private bool _isPerfectPass;

		/// <summary>
		/// 计时赛任务，测试用
		/// </summary>
		//		public override void InitLevelTask()
		//		{
		//			Client.Task.LevelStatValue = new JObject();
		//			Tasks = new List<LevelTaskInfo>();
		//			for (var i = 0; i < 3; i++)
		//			{
		//				var time = (i + 1)*15;
		//				var info = new LevelTaskInfo(new TaskData
		//				{
		//					TaskMode = TaskMode.LevelTask,
		//					TaskType = "GameTimeLeft",
		//					Description = "剩余" + time + "秒完成游戏",
		//					TaskGoal = time
		//				});
		//				info.Enable();
		//				Tasks.Add(info);
		//			}
		//		}


		void Start() {
			Checkers = RaceManager.Ins.raceLine.TimeCheckerManager.GetCheckPointList();
			if (Checkers == null || Checkers.Count == 0) {
				throw new Exception("[Game Timing] No time check points !!!");
			}
		}

		public override void PrepareGame(MatchInfo matchInfo, PlayerInfo player) {
			base.PrepareGame(matchInfo, player);
			// 初始化数据
			TimeRun = 0;
			TimingCheckerIndex = 0;
			RecoverRate = matchInfo.Data.TimeRate;
			_passIndex = -1;
			_recoverIndex = -1;
			PassCount = 0;
			_recoverValue = 0;
			TimeLeft = 10f;
		}

		private void Update() {
			if (RaceManager.Ins.GamePhase != GamePhase.Gaming) {
				return;
			}
			// 计时
			TimeRun += Time.deltaTime;
			TimeLeft -= Time.deltaTime;
			TimeLeft = TimeLeft < 0 ? 0 : TimeLeft;

			// 恢复时间
			if (_recoverValue > 0) {
				TimeLeft += _recoverValue;
				OnRecoverTime(_isPerfectPass, _recoverValue);

				// Debug.Log("<color=green>[Game Timing]</color> Time left:" + TimeLeft + "  Is Perfect :" + _isPerfectPass + "  Recover time " + _recoverValue);

				_isPerfectPass = false;
				_recoverValue = 0f;

				_recoverIndex = NextCheckerIndex(_recoverIndex);
			}

			// 结束
			if (TimeLeft <= 0) {
				BikeManager.Ins.CurrentBike.racerInfo.DoFinish();
			}
		}

		public void PassTimeCheckPoint(TimeCheckPoint checker) {
			if (!IsCheckerVailed(checker, _passIndex)) return;

			_passIndex = NextCheckerIndex(_passIndex);
			PassCount++;

			// 恢复上一个通过的检查点
			PreChecker().gameObject.SetActive(true);

			// Debug.Log("<color=green>[Game Timing]</color> Pass time check point " + PassCount + " / " + CheckerCount);

			// 到达最后一个检查点
			if (PassCount == CheckerCount) {
				BikeManager.Ins.CurrentBike.racerInfo.DoFinish();
			}
		}

		public void RecoverTime(TimeCheckPoint checker, float recoverTime, float rewardRate = -1f) {
			if (!IsCheckerVailed(checker, _recoverIndex)) return;

			// 最后一个点时游戏结束，不恢复
			if (PassCount == CheckerCount) {
				RaceManager.Ins.raceLine.TimeCheckerManager.HideTimeChecker();
				return;
			}
			// 时间 = 剩余时间 + 检查点恢复时间 * 恢复倍率
			var value = recoverTime * RecoverRate * (rewardRate < 0 ? 1f : rewardRate);
			_recoverValue += value;

			if (rewardRate > 0) _isPerfectPass = true;
		}

		public float NextDis() {
			return RaceManager.Ins.GamePhase != GamePhase.Gaming
				? 0
				: RaceManager.Ins.raceLine.WaypointManager.GetDisToWayPoint(BikeManager.Ins.CurrentBike, NextChecker().WayPoint);
		}

		public TimeCheckPoint NextChecker() {
			return Checkers[NextCheckerIndex(_passIndex)];
		}

		public TimeCheckPoint PreChecker() {
			return Checkers[PreCheckerIndex(_passIndex)];
		}

		private bool IsCheckerVailed(TimeCheckPoint checker, int nowIndex) {
			var index = Checkers.IndexOf(checker);
			return index == NextCheckerIndex(nowIndex);
		}

		private int NextCheckerIndex(int index) {
			var result = index + 1;
			if (result >= Checkers.Count) {
				result = 0;
			}
			return result;
		}

		private int PreCheckerIndex(int index) {
			var result = index - 1;
			if (result < 0) {
				result = Checkers.Count - 1;
			}
			return result;
		}
	}
}

