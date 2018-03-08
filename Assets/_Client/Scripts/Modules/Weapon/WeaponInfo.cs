using System.Text;
using XPlugin.Data.Json;

namespace GameClient {
	public class WeaponInfo : ItemInfo {

		public WeaponInfo() {
		}

		public WeaponInfo(int id, int amount = 0) : base(id, amount) {
		}

		public WeaponInfo(ItemData data, int amount = 0) : base(data, amount) {
		}

		[JsonIgnore]
		public WeaponData WeaponData {
			get { return (WeaponData)this.Data; }
		}

		[JsonIgnore]
		public string AbilityDesc {
			get {
				string desc = WeaponData.AbilityDesc;
				for (int i = 1; i <= WeaponData.AbilityValues.Length; i++) {
					var value = WeaponData.AbilityValues[i - 1];
					desc = desc.Replace("#" + i, ((float)value).ToString("0.#"));
				}
				return desc;
			}
		}

		//存储到ModWeapon
		public override void SaveData() {
			if (!Client.User.UserInfo.WeaponList.ContainsKey(this.Data.ID)) {
				Client.User.UserInfo.WeaponList.Add(this.Data.ID, this);
			}
			Client.Weapon.SaveData();
		}

		public JObject ToJson() {
			JObject ret = new JObject();
			ret["ID"] = this.Data.ID;
			return ret;
		}

		public WeaponInfo(JObject json) :
			this(Client.Weapon[json["ID"].AsInt()]) {
		}

	}
}