//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 漂移氮气回复量增加指定百分比
	/// </summary>
	public class AbilityRecEnergyWithDrift : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			var bike = target.Bike;
			bike.bikeControl.EnergyRecoverSpeedDrift *= (1 + Value / 100f);
		}
	}
}
