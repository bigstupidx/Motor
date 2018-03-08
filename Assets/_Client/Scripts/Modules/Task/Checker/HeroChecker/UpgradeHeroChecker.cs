//
// UpgradeHeroChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 升级指定英雄达到一定次数
	/// </summary>
	public class UpgradeHeroChecker : TaskChecker {
		private int _heroID;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Hero_Upgrade };
			}
		}

		public override void SetTaskParam(string paramStr) {
			_heroID = int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("UpgradeHero" + (int)_heroID);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 1) {
				if ((int)args[0] != (int)_heroID) return;
				SetStat("UpgradeHero" + (int)_heroID, TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}