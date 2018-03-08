//
// PropShield.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)


using GameClient;

namespace Game {
	/// <summary>
	/// 道具 - 护盾
	/// </summary>
	public class PropShield : PropBase {
		public override PropType Type {
			get { return PropType.Shield; }
		}


		public override bool Use(BikeBase bike) {
			base.Use(bike);
			bike.bikeBuff.buffInvincible.Start(5);
			return true;
		}
	}
}

