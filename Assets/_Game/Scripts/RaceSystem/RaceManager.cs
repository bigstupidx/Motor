//
// RacerManager.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using System.Collections.Generic;

namespace Game {
	public enum GamePhase {
		/// <summary>
		/// 等待
		/// </summary>
		Waiting = 0,
		/// <summary>
		/// 倒计时
		/// </summary>
		CountDown = 1,
		/// <summary>
		/// 游戏中
		/// </summary>
		Gaming = 2,
		/// <summary>
		/// 结束
		/// </summary>
		Over = 3,
	}

	/// <summary>
	/// 比赛模式
	/// </summary>
	public enum RaceMode {
		None = 0,
		/// <summary>
		/// 竞速赛
		/// </summary>
		Racing = 1,
		/// <summary>
		/// 淘汰赛
		/// </summary>
		Elimination = 2,
		/// <summary>
		/// 计时赛
		/// </summary>
		Timing = 3,
	}

	public class RaceManager : Singleton<RaceManager> {
		/// <summary>
		/// 比赛状态
		/// </summary>
		public GamePhase GamePhase { get; set; }
		/// <summary>
		/// 比赛中的玩家数据列表
		/// </summary>
		[NonSerialized] public List<RacerInfo> PlayerList = new List<RacerInfo>();

		/// <summary>
		/// 结束比赛的玩家数据列表
		/// 竞速模式：顺序存入，结果即为最终排名。		显示顺序：FinishList, PlayerList
		/// 淘汰模式：逆序存入，结果即为最终排名。		显示顺序：PlayerList, FinishList 
		/// </summary>
		[NonSerialized] public List<RacerInfo> FinishList = new List<RacerInfo>();
		[NonSerialized] public RaceMode RaceMode;
		[NonSerialized] public RaceLine raceLine;
		/// <summary>
		/// 游戏人数
		/// </summary>
		public int PlayerNum { get; private set; }
		/// <summary>
		/// 圈数
		/// 小于1时无效，大于等于1时为比赛结束条件
		/// </summary>
		public int Turn { get; private set; }
		/// <summary>
		/// 第一名
		/// </summary>
		public BikeBase First {
			get {
				return PlayerList.Count < 1 ? null : PlayerList[0];
			}
		}

		/// <summary>
		/// 最后一名(包含已经结束比赛的)
		/// </summary>
		public BikeBase Last {
			get {
				return PlayerList.Count < 1 ? null : PlayerList[PlayerList.Count - 1];
			}
		}

		// 事件
		public Action OnCountDownStart = delegate { };
		public Action OnMatchStart = delegate { };
		public Action OnGameFinish = delegate { };
		public Action OnWaitingStart = delegate { };

		/// <summary>
		/// 赛道初始化
		/// </summary>
		public void PrepareRace(int turn, int playerNum, string raceLine, string ObjLine, RaceMode mode) {
			RaceMode = mode;
			Turn = turn;
			PlayerNum = playerNum;
			// 初始化路线
			this.raceLine = RaceLineManager.Ins.SpawnLine(raceLine);
			// 物品路线
			ObjLineManager.Ins.SpawnLine(ObjLine);
		}

		/// <summary>
		/// 获取排名
		/// </summary>
		public int GetPlayerRank(RacerInfo player) {
			var rank = -1;
			switch (RaceMode) {
				case RaceMode.Racing:
					rank = FinishList.IndexOf(player) + 1;
					return rank > 0 ? rank : PlayerList.IndexOf(player) + 1 + FinishList.Count;
				case RaceMode.Elimination:
					rank = PlayerList.IndexOf(player) + 1;
					return rank > 0 ? rank : FinishList.IndexOf(player) + 1 + PlayerList.Count;
				case RaceMode.Timing:
					return 1;
				default:
					throw new Exception("Race mode " + RaceMode + " not exist");
			}
		}

		/// <summary>
		/// 根据排名取得玩家
		/// </summary>
		public RacerInfo GetPlayerByRank(int rank) {
			if (rank < 1 || rank > PlayerNum) return null;
			switch (RaceMode) {
				case RaceMode.Racing:
					return FinishList.Count >= rank ? FinishList[rank - 1] : PlayerList[rank - FinishList.Count - 1];
				case RaceMode.Elimination:
					return PlayerList.Count >= rank ? PlayerList[rank - 1] : FinishList[rank - PlayerList.Count - 1];
				case RaceMode.Timing:
				default:
					throw new Exception("GetPlayerByRank 在该模式下不可用");
			}
		}

		public void StartWaiting() {
			GamePhase = GamePhase.Waiting;
			OnWaitingStart();
		}

		public void StartCountDown() {
			GamePhase = GamePhase.CountDown;
			OnCountDownStart();
		}

		public void StartMatch() {
			GamePhase = GamePhase.Gaming;
			for (var i = 0; i < PlayerList.Count; i++) {
				PlayerList[i].DoStart(this.raceLine.WaypointManager);
			}
			OnMatchStart();
			GameModeBase.Ins.State = GameState.Gaming;
		}

		public void FinishGame() {
			GamePhase = GamePhase.Over;
			OnGameFinish();
		}
		void Update() {
			//更新排名
			PlayerList.Sort((m1, m2) => (int)(m2.Distance*1000f - m1.Distance*1000f));
		}

	}
}

