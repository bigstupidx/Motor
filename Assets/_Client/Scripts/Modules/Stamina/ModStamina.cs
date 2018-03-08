using XPlugin.Data.Json;
using XPlugin.Security.AnitiCheatValue;

namespace GameClient {
	public class ModStamina : ClientModule {

		/// <summary>
		/// 回复最大值
		/// </summary>
		public int LimitValue {
			get { return Client.System.GetMiscValue<int>("Stamina.LimitValue"); }
		}

		/// <summary>
		/// 回复间隔
		/// </summary>
		public int RestoreTime {
			get {
//				return 20;
				return Client.System.GetMiscValue<int>("Stamina.RestoreTime"); 
				
			}
		}

		public int InitValue {
			get {
//				return 20;
				return Client.System.GetMiscValue<int>("Stamina.InitValue"); 
			}
		}

		public int MaxValue {
			get { return Client.System.GetMiscValue<int>("Stamina.MaxValue"); }
		}

		public override void InitInfo(string s) {
			Client.User.UserInfo.Stamina = new StaminaInfo(LimitValue, RestoreTime, MaxValue);
			JObject saved = JObject.Parse(s);//因为体力数据的特殊性，这里使用手动解析
			if (saved == null) {
				Client.User.UserInfo.Stamina.SetValue(InitValue, Client.System.TimeOnLocal);
			} else {
				Client.User.UserInfo.Stamina.SetValue(saved["NowValue"].OptInt(0), saved["RestoreStart"].OptLong(Client.System.TimeOnLocal));
			}
		}

		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.Stamina);
		}

		public bool ChangeStamina(int delta) {
			return Client.User.UserInfo.Stamina.ChangeValue(delta);
		}

	}
}