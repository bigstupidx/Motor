using XPlugin.Data.Json;

namespace GameClient {
	public class ModSetting : ClientModule {

		public override void InitInfo(string s) {
			SettingInfo setting = JsonMapper.ToObject<SettingInfo>(s);
			if (setting == null) {
				setting = new SettingInfo();
				if (Client.Config.DefaultLowQuality) {
					setting.GameQuality = GameQuality.Low;
				}
				else {
					setting.GameQuality = DeviceLevel.Score > 60 ? GameQuality.High : GameQuality.Low;
				}
			}
			Client.User.UserInfo.Setting = setting;

			SfxManager.Ins.SetMusicVolume(setting.MusicVolume);
			SfxManager.Ins.SetSfxVolume(setting.SfxVolume);
		}

		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.Setting);
		}
	}
}