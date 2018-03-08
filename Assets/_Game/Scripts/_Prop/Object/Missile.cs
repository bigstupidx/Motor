//
// Missile.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using GameClient;
using UnityEngine;

namespace Game {
	/// <summary>
	/// 导弹
	/// </summary>
	public class Missile : PropObjectMoveBase {
		public override PropType Type {
			get { return PropType.Missile; }
		}

		public override void OnCatch(BikeBase bike) {
			if (bike.bikeControl != User.bikeControl) {
				if (Random.Range(0, 100) < bike.bikeHealth.PropDef) {
					return;
				}
				var result = bike.bikeHealth.Damage(100);
				if (result) {
					bike.bikeHealth.Die();
					if (User.gameObject.CompareTag(Tags.Ins.Player)) {
						Client.EventMgr.SendEvent(EventEnum.Game_KillEnemy, 1);
					}
				}
				OnStop();
			}
		}

	}
}