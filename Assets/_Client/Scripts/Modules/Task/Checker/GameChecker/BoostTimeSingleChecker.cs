//
// BoostTimeSingleChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 单次氮气加速达到一定时间
	/// </summary>
	public class BoostTimeSingleChecker : TaskChecker {

		private float _boostTime = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameBoost += GameBoost;
			Client.EventMgr.GameBoosting += GameBoosting;
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameBoost -= GameBoost;
			Client.EventMgr.GameBoosting -= GameBoosting;
		}

		private void GameBoosting(float disPerFrame, float deltaTime) {
			_boostTime += deltaTime;
			if (_boostTime > TaskProgress) {
				SetStat("BoostTimeSingle", Convert.ToInt32(_boostTime));
			}
			Check();
		}

		private void GameBoost(float dis, float time) {
			_boostTime = 0;
			if (time > TaskProgress) {
				SetStat("BoostTimeSingle", time);
			}
			Check();
		}



		public override int TaskProgress {
			get {
				return GetStat<int>("BoostTimeSingle");
			}
		}

	}
}