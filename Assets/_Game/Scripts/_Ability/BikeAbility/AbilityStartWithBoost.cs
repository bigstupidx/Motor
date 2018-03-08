//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 开局冲刺（相当于加速带效果）
	/// </summary>
	public class AbilityStartWithBoost : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			target.Bike.racerInfo.OnStart += bike => {
				bike.bikeControl.Rigidbody.velocity = bike.transform.forward * this.Value;
				bike.bikeControl.Boosting = true;
				bike.bikeControl.BosstingWithoutEnergy = true;
				bike.bikeInput.OnBosstWithoutEnergyTiming();
			};
		}
	}

}