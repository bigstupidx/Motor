//
// GameOnlineChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 完成指定次数的在线游戏
	/// </summary>
	public class GameOnlineChecker : TaskChecker {

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GameOnline");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 3 && (bool)args[2]) {
				SetStat("GameOnline", TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}