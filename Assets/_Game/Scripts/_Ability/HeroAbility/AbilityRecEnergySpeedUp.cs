//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 氮气收集速度上升
	/// </summary>
	public class AbilityRecEnergySpeedUp : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			var bike = target.Bike;
			bike.bikeControl.EnergyRecoverSpeedDrift *= (1 + Value / 100);
		}
	}

}