//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 附加氮气槽
	/// </summary>
	public class AbilityAttachEnergy : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			target.Bike.racerInfo.OnStart += bike => {
				bike.bikeControl.BoostingEnergyMax *= 2;
				bike.bikeControl.BoostingEnergy = bike.bikeControl.BoostingEnergyMax;
			};
		}
	}

}
