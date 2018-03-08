//
// BuffInvincible.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace Game
{
	/// <summary>
	/// BUFF - 无敌护盾
	/// </summary>
	public class BuffInvincible : BuffBaseWithEffect {
		public override bool Start(float buffTime) {
			if (isAffect)
			{
				time = buffTime;
				return true;
			}

			effectPos = bike.bikeControl.bikeSetting.MainBody.position;
			var result = base.Start(buffTime, "buff/buff_invincible");
			return result;
		}
	}
}

