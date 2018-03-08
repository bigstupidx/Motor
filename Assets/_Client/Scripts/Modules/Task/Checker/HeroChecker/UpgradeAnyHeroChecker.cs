//
// UpgradeAnyHeroChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;

namespace GameClient {

	/// <summary>
	/// 升级任意英雄达到一定次数
	/// </summary>
	public class UpgradeAnyHeroChecker : TaskChecker {
		private ITaskStat _taskStat;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Hero_Upgrade };
			}
		}


		public override int TaskProgress {
			get {
				return GetStat<int>("UpgradeHero");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			SetStat("UpgradeHero", TaskProgress + 1);
			base.OnEvent(eventType, args);
		}
	}
}