//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using GameClient;

namespace Game {
	/// <summary>
	/// 特殊能力 - 体力恢复时间缩短
	/// </summary>
	public class AbilityStaminaRestoreUp : AbilityBase {
		private int _orginalRestoreTime;

		public override AbilityType Type { get { return AbilityType.Menu; } }

		public override void Enable(PlayerInfo target) {
			base.Enable(target);
			_orginalRestoreTime = Client.User.UserInfo.Stamina.RestoreTime;
			var newRestoreTime = (int)(_orginalRestoreTime * (1 - Value / 100));
			Client.User.UserInfo.Stamina.SetRestoreTime(newRestoreTime);
		}

		public override void Disable(PlayerInfo target) {
			base.Disable(target);
			Client.User.UserInfo.Stamina.SetRestoreTime(_orginalRestoreTime);
		}
	}
}
