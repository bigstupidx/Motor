//
// PropEnergy.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using GameClient;

namespace Game {
	/// <summary>
	/// 道具 - 氮气
	/// </summary>
	public class PropEnergy : PropBase {
		public override PropType Type {
			get {
				return PropType.Energy;
			}
		}

		public override bool Use(BikeBase bike) {
			base.Use(bike);
			bike.bikeControl.BoostingEnergy += 100f;
			return true;
		}
	}
}
