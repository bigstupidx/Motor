//
// BoostDisSingleChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 单次氮气加速达到一定距离
	/// </summary>
	public class BoostDisSingleChecker : TaskChecker {

		private float _boostDis = 0;


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

		private void GameBoost(float dis, float time) {
			_boostDis = 0;
			if (dis > TaskProgress) {
				SetStat("BoostDisSingle", dis);
			}
			Check();
		}

		private void GameBoosting(float disPerFrame, float deltaTime) {
			_boostDis += disPerFrame;
			if (_boostDis > TaskProgress) {
				SetStat("BoostDisSingle", Convert.ToInt32(_boostDis));
			}
			Check();
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("BoostDisSingle");
			}
		}
	}
}