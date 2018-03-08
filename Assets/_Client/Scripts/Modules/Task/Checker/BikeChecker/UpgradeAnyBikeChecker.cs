//
// UpgradeAnyBikeChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 升级任意车辆达到一定次数
	/// </summary>
	public class UpgradeAnyBikeChecker : TaskChecker {

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Bike_Upgrade };
			}
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("UpgradeBike");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			SetStat("UpgradeBike", TaskProgress + 1);
			base.OnEvent(eventType, args);
		}
	}
}