//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 开局护盾
	/// </summary>
	public class AbilityStartWithShield : AbilityBase {
		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			target.Bike.racerInfo.OnStart += bike => {
				bike.bikeBuff.buffInvincible.Start(Value);
			};
		}
	}

}