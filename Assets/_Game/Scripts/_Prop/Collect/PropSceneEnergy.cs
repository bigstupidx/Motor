//
// PropSceneEnergy.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;
using GameUI;
using UnityEngine;

namespace Game {
	/// <summary>
	/// 道具 - 氮气
	/// </summary>
	public class PropSceneEnergy : PropSceneBase {
		private float Value = 33.4f;

		public override bool Gain(BikeBase bike) {
			var result = base.Gain(bike);
			if (!result) return false;
			// 获得氮气
			bike.bikeControl.BoostingEnergy += Value;
			PlayAudio(SfxType.SFX_GetProp);
			return true;
		}

		public override void OnGainedEffectSpawnd(GameObject effect, BikeBase bike) {
			base.OnGainedEffectSpawnd(effect, bike);
			effect.AddComponent<FollowTarget>().Target = bike.transform;
		}
	}
}