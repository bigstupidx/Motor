//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 加速带时间延长指定时间
	/// </summary>
	public class AbilityAFExtend : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			target.Bike.bikeControl.BoostingWithoutEnergyTime += Value;
		}
	}

}
