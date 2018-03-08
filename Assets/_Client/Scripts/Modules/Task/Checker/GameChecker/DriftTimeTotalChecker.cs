//
// DriftTimeTotalChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 总计漂移达到一定时间
	/// </summary>
	public class DriftTimeTotalChecker : TaskChecker {

		private float _driftTime = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameDrifting += GameDrifting;
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameDrifting -= GameDrifting;
		}

		private void GameDrifting(float disPerFrame, float deltaTime) {
			_driftTime += deltaTime;
			if (_driftTime >= 1f) {
				_driftTime -= 1f;
				SetStat("DriftTimeTotal", TaskProgress + 1);
			}
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("DriftTimeTotal");
			}
		}

	}
}