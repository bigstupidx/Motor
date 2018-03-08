//
// DriftDisTotalChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 总计漂移达到一定距离
	/// </summary>
	public class DriftDisTotalChecker : TaskChecker {

		private float _driftDis = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameDrifting += GameDrifting;
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameDrifting -= GameDrifting;
		}

		private void GameDrifting(float disPerFrame, float deltaTime) {
			_driftDis += disPerFrame;
			if (_driftDis >= 1f) {
				_driftDis -= 1f;
				SetStat("DriftDisTotal", TaskProgress + 1);
			}
			Check();
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("DriftDisTotal");
			}
		}

	}
}