//
// BuffFloat.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;

namespace Game
{
	/// <summary>
	/// BUFF - 浮空
	/// </summary>
	public class BuffFloat : BuffBaseWithEffect
	{
		private Vector3 _height = Vector3.up*4;

		private float _radian = 0; // 弧度  
		private float _perRadian = 4f; // 每次变化的弧度  
		private float _radius = 0.8f; // 半径  
		private Vector3 _oldPos; // 开始时候的坐标  
		private bool _achieve;

		public override bool Start(float buffTime) {

			if (isAffect)
			{
				time = buffTime;
				return true;
			}

			effectPos = bike.bikeDriver.DriverPos[bike.bikeDriver.BikeDriverModel.DriverPosIndex].position + Vector3.up * 0.5f;
			var result = base.Start(buffTime, "buff/buff_float");

			BikeManager.Ins.SetBikeActive(bike, false);
			bike.bikeControl.Rigidbody.isKinematic = true;
			_achieve = false;
			_oldPos = bike.transform.localPosition + _height;
			//TweenPosition.Begin(bike.gameObject, 0.5f, _oldPos);	

			return result;
		}

		public override void OnBuffStop() {
			base.OnBuffStop();
			BikeManager.Ins.SetBikeActive(bike, true);
			bike.bikeControl.Rigidbody.isKinematic = false;
			// TweenPosition.Begin(bike.gameObject, 0.5f, bike.transform.localPosition - _height);
		}

		public override void FixedUpdateWhenAffect() {
			base.FixedUpdateWhenAffect();
			if (!_achieve)
			{
				bike.transform.localPosition = Vector3.MoveTowards(bike.transform.localPosition, _oldPos, Time.fixedDeltaTime*10);
				if (Vector3.Distance(bike.transform.localPosition, _oldPos) <= 0.01f)
				{
					_achieve = true;
				}
			}
			else
			{
				_radian += _perRadian * Time.fixedDeltaTime;
				var dy = Mathf.Cos(_radian)*_radius;
				bike.transform.localPosition = _oldPos + new Vector3(0, dy, 0);
			}
		}
	}

}
