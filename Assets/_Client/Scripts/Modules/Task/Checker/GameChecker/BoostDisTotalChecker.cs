//
// BoostDisTotalChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 总计氮气加速达到一定距离
	/// </summary>
	public class BoostDisTotalChecker : TaskChecker {
		private float _boostDis = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameBoosting += GameBoosting;
		}

		private void GameBoosting(float disPerFrame, float deltaTime) {
			_boostDis += disPerFrame;
			if (_boostDis >= 1f) {
				_boostDis -= 1f;
				SetStat("BoostDisTotal", TaskProgress + 1);
			}
			Check();
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameBoosting -= GameBoosting;
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("BoostDisTotal");
			}
		}

	}
}