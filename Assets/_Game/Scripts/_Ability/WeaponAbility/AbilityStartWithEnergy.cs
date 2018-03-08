//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 初始氮气
	/// </summary>
	public class AbilityStartWithEnergy : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			target.Bike.racerInfo.OnStart += bike => {
				bike.bikeControl.BoostingEnergy = bike.bikeControl.BoostingEnergyMax * (Value / 100f);
			};
		}
	}
}
