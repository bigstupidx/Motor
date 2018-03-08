//
// DriftDisSingleChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace GameClient {

	/// <summary>
	/// 单次漂移达到一定距离
	/// </summary>
	public class DriftDisSingleChecker : TaskChecker {

		private float _driftDis = 0;

		protected override void OnEnable() {
			base.OnEnable();
			Client.EventMgr.GameDrift += GameDrift;
			Client.EventMgr.GameDrifting += GameDrifting;
		}

		protected override void OnDisable() {
			base.OnDisable();
			Client.EventMgr.GameDrift -= GameDrift;
			Client.EventMgr.GameDrifting -= GameDrifting;
		}

		private void GameDrifting(float disPerFrame, float deltaTime) {
			_driftDis += disPerFrame;
			if (_driftDis > TaskProgress) {
				SetStat("DriftDisSingle", Convert.ToInt32(_driftDis));
			}
			Check();
		}

		private void GameDrift(float dis, float time) {
			_driftDis = 0;
			if (dis > TaskProgress) {
				SetStat("DriftDisSingle", dis);
			}
			Check();
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("DriftDisSingle");
			}
		}
	}
}