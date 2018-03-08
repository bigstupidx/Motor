//
// GameChampionshipChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 完成指定次数的锦标赛游戏
	/// </summary>
	public class GameChampionshipChecker : TaskChecker {

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GameChampionship");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 4 && (bool)args[3]) {
				SetStat("GameChampionship", TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}