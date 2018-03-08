//
// GameStarChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 获得一定星数
	/// </summary>
	public class GameStarChecker : TaskChecker {
		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_Star };
			}
		}

		public override string ProgressStr {
			get { return GetStat<int>("GameStar") + "/" + TaskGoal; }
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GameStar");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 1) {
				SetStat("GameStar", TaskProgress + (int)args[0]);
				base.OnEvent(eventType, args);
			}
		}
	}
}
