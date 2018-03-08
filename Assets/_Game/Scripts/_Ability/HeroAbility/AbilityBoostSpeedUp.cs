//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 氮气速度上升
	/// </summary>
	public class AbilityBoostSpeedUp : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			var bike = target.Bike;
			bike.bikeControl.bikeSetting.LimitBoostSpeed *= (1 + Value / 100);
		}
	}

}
