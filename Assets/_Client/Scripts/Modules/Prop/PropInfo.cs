
using XPlugin.Data.Json;

namespace GameClient {
	public class PropInfo : ItemInfo {

		[JsonIgnore]
		public PropData PropData {
			get { return (GameClient.PropData)this.Data; }
		}
		
		public PropInfo() {
		}

		public PropInfo(int itemID, int amount = 0) : base(itemID, amount) {
		}
		public PropInfo(ItemData data, int amount = 0) : base(data, amount) {
		}

		//存储到ModProp
		public override void SaveData() {
			if (!Client.User.UserInfo.PropList.ContainsKey(this.Data.ID)) {
				Client.User.UserInfo.PropList.Add(this.Data.ID, this);
			}
			Client.Prop.SaveData();
		}


	}

}

