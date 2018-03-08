//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 漂移不减速
	/// </summary>
	public class AbilityDriftKeepSpeed : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			var bike = target.Bike;
			bike.bikeControl.bikeSetting.DriftReduceSpeed = 1f;
		}
	}
}