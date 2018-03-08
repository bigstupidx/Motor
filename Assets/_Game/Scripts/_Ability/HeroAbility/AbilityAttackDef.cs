//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 一定概率躲避近战攻击
	/// </summary>
	public class AbilityAttackDef : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			var bike = target.Bike;
			bike.bikeHealth.AttackDef *= (1 + Value / 100);
		}
	}

}
