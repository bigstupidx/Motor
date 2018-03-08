//
// BuffVertigo.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;

namespace Game
{
	/// <summary>
	/// BUFF - 眩晕
	/// </summary>
	public class BuffVertigo : BuffBaseWithEffect {

		public override bool Start(float buffTime)
		{
			if (isAffect)
			{
				time = buffTime;
				return true;
			}

			effectPos = bike.transform.position;
//			effectPos = bike.bikeDriver.DriverPos[bike.bikeDriver.BikeDriverModel.DriverPosIndex].position
//						+ Vector3.up*(1f + 1*bike.bikeDriver.BikeDriverModel.DriverPosIndex);
			var result = base.Start(buffTime, "buff/buff_tesla");
//			bike.bikeControl.ActiveControl = false;
			bike.bikeControl.Boosting = false;

			return result;
		}

		public override void UpdateWhenAffect()
		{
			base.UpdateWhenAffect();
			bike.bikeControl.Boosting = false;
//				var control = bike.bikeControl;
//				// 减速到0
//				if (bike.bikeControl.Speed > 0.01f)
//				{
//					control.Rigidbody.velocity = Vector3.MoveTowards(control.Rigidbody.velocity, Vector3.zero, Time.deltaTime*50);
//				} 
		}

//		public override void OnBuffStop()
//		{
//			base.OnBuffStop();
//			bike.bikeControl.ActiveControl = true;
//		}
	}
}

