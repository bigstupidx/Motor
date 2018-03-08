//
// UpgradeBikeChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 升级指定车辆达到一定次数
	/// </summary>
	public class UpgradeBikeChecker : TaskChecker {
		private int _bikeID;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Bike_Upgrade };
			}
		}

		public override void SetTaskParam(string paramStr) {
			_bikeID = int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("UpgradeBike" + (int)_bikeID);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 1) {
				if ((int)args[0] != (int)_bikeID) return;
				SetStat("UpgradeBike" + (int)_bikeID, TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}