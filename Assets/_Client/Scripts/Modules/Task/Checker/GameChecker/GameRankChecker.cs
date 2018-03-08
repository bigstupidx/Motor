//
// GameRankChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 达到指定名次
	/// </summary>
	public class GameRankChecker : TaskLessChecker {
		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override string ProgressStr {
			get { return GetStat<int>("GameRank") <= TaskGoal ? (LString.GAMECLIENT_GAMECRASHCHECKER).ToLocalized() : (LString.GAMECLIENT_GAMECRASHCHECKER_1).ToLocalized(); }
		}

		public override int TaskProgress {
			get {
				var value = GetStat<int>("GameRank");
				if (value == 0) {
					value = TaskGoal + 1;
					SetStat("GameRank", value);
				}
				return value;
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 7) {
				SetStat("GameRank", (int)args[6]);
				base.OnEvent(eventType, args);
			}
		}
	}
}
