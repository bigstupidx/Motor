//
// GameChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 完成指定次数的游戏
	/// </summary>
	public class GameChecker : TaskChecker {

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("Game");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			SetStat("Game", TaskProgress + 1);
			base.OnEvent(eventType, args);
		}
	}
}