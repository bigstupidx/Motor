//
// PropBox.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System.Collections.Generic;
using GameClient;
using UnityEngine;

namespace Game {
	/// <summary>
	/// 道具 - 宝箱
	/// </summary>
	public class PropBox : PropSceneBase {

		private static List<PropType> RandomList = new List<PropType>() {
//			PropType.Bubble,
			PropType.Energy,
			PropType.Missile,
			PropType.Shield,
		};

		public override bool Gain(BikeBase bike) {
			var result = base.Gain(bike);
			if (!result) return false;
			//随机一个道具
			var prop = RandomList[Random.Range(0, RandomList.Count)];
			bike.bikeProp.Gain(prop);
			PlayAudio(SfxType.SFX_GetProp);
			return true;
		}
	}

}
