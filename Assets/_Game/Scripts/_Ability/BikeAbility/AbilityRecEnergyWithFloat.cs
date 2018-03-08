//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 浮空恢复大量氮气（漂移恢复速度2倍）
	/// </summary>
	public class AbilityRecEnergyWithFloat : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			var bike = target.Bike;
			bike.bikeControl.EnergyRecoverSpeedFloat = bike.bikeControl.EnergyRecoverSpeedDrift * 3;
		}
	}
}
