//
// DriftTimeSingleChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 单次漂移达到一定时间
	/// </summary>
	public class DriftTimeSingleChecker : TaskChecker {

		private float _driftTime = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameDrift += GameDrift;
			Client.EventMgr.GameDrifting += GameDrifting;
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameDrift += GameDrift;
			Client.EventMgr.GameDrifting += GameDrifting;
		}

		private void GameDrifting(float disPerFrame, float deltaTime) {
			_driftTime += deltaTime;
			if (_driftTime > TaskProgress) {
				SetStat("DriftTimeSingle", Convert.ToInt32(_driftTime));
			}
			Check();
		}

		private void GameDrift(float dis, float time) {
			_driftTime = 0;
			if (time > TaskProgress) {
				SetStat("DriftTimeSingle", time);
			}
			Check();
		}


		public override int TaskProgress {
			get {
				return GetStat<int>("DriftTimeSingle");
			}
		}

	}
}