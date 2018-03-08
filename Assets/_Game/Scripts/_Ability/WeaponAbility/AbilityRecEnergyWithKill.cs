//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;
using UnityEngine;


namespace Game
{
	/// <summary>
	/// 特殊能力 - 初始满氮气
	/// </summary>
	public class AbilityRecEnergyWithKill : AbilityBase
	{
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			target.Bike.bikeAttack.OnKillEnemy += bike => {
				bike.bikeControl.BoostingEnergy += bike.bikeControl.BoostingEnergyMax * (Value / 100f);
			};
		}
	}
}
