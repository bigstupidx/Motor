//
// PropMissile.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using GameClient;
using UnityEngine;

namespace Game {
	public class PropMissile : PropBase {
		public override PropType Type {
			get { return PropType.Missile;}
		}

		public override bool Use(BikeBase bike) {
			base.Use(bike);
			var missile = GameObjectUtility.LoadAndIns("Missile").GetComponent<Missile>();
			missile.User = bike;
			// missile.Target = bike.bikeAttack.LockTarget;
			// 设置位置
			missile.transform.localPosition = bike.transform.localPosition;
			var pos = WaypointMath.AttachRoadPoint(missile.transform.localPosition);
			if (Vector3.Distance(pos, missile.transform.localPosition) > 0.01f) {
				missile.transform.localPosition = pos + Vector3.up * 2f;
			}
			missile.transform.localRotation = Quaternion.LookRotation(bike.transform.forward);
			return true;
		}
	}
}

