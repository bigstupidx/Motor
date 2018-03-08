//
// PropSceneCoin.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;
using GameUI;

namespace Game {
	/// <summary>
	/// 道具 - 金币
	/// </summary>
	public class PropSceneCoin : PropSceneBase {
		private int Value = 1;

		public override bool Gain(BikeBase bike) {
			var result = base.Gain(bike);
			if (!result) return false;

			if (bike.CompareTag(Tags.Ins.Player)) {
				if (GamePlayBoard.Ins != null) {
					GamePlayBoard.Ins.UpdateCoin(Value);
				}

				Client.EventMgr.SendEvent(EventEnum.Game_GainProp, PropType.Coin, Value);
				// 获得金币
				var count = bike.info.GetStat<int>("PropType" + (int)PropType.Coin);
				bike.info.SetStat("PropType" + (int)PropType.Coin, count + Value);
				PlayAudio(SfxType.SFX_GetCoin);
			}
			return true;
		}
	}
}