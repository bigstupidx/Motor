//
// BoostTimeTotalChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 总计氮气加速达到一定时间
	/// </summary>
	public class BoostTimeTotalChecker : TaskChecker {

		private float _boostTime = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameBoosting += GameBoosting;
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameBoosting -= GameBoosting;
		}

		private void GameBoosting(float disPerFrame, float deltaTime) {
			_boostTime += deltaTime;
			if (_boostTime >= 1f) {
				_boostTime -= 1f;
				SetStat("BoostTimeTotal", TaskProgress + 1);
			}
			Check();
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("BoostTimeTotal");
			}
		}
	}
}