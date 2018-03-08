//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 金币加成
	/// </summary>
	public class AbilityCoinAddition : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			GameModeBase.Ins.OnGetReward += info => {
				var count = info.GetStat<int>("PropType" + (int)PropType.Coin);
				info.SetStat("PropType" + (int)PropType.Coin, (int)(count * (1 + Value / 100)));
			};
		}
	}

}
