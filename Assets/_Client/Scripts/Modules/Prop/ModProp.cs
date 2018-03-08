using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModProp : ClientModule {

		[NonSerialized]
		public bool GamingBuy = true;

		protected Dictionary<int, PropData> PropDataList = new Dictionary<int, PropData>();
		protected Dictionary<PropType, PropData> PropDataDic = new Dictionary<PropType, PropData>();

		/// <summary>
		/// 随机获取一种道具数据
		/// </summary>
		public PropData Random() {
			return PropDataList.Random();
		}

		public PropData GetDataByType(PropType type) {
			return PropDataDic.GetValue(type);
		}

		public List<PropInfo> GetSortedInfos() {
			List<PropInfo> ret = new List<PropInfo>();
			foreach (var propData in this.PropDataList) {
				ret.Add(GetPropInfo(propData.Key));
			}
			ret.Sort((l, r) => l.PropData.Sort - r.PropData.Sort);
			return ret;
		}

		/// <summary>
		/// 获取指定道具数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public PropData this[int id] {
			get {
				PropData data;
				this.PropDataList.TryGetValue(id, out data);
				return data;
			}
		}

		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ExecuteQuery("select * from Prop left join Item on Item.ID == Prop.ID");
			while (reader.Read()) {
				PropData data = new PropData(reader);
				this.PropDataList.Add(data.ID, data);
				this.PropDataDic.Add(data.PropType, data);
			}
		}

		public override void ResetData() {
			this.PropDataList.Clear();
		}

		public override void InitInfo(string s) {
			base.InitInfo(s);
			var propList = JsonMapper.ToObject<Dictionary<int, PropInfo>>(s);
			if (propList == null) {
				propList = new Dictionary<int, PropInfo>();
			}
			Client.User.UserInfo.PropList = propList;
			foreach (var e in Client.User.UserInfo.PropList) {
				e.Value.InitData(Client.Prop[e.Key]);
			}
		}
		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.PropList);
		}

		/// <summary>
		/// 获得道具
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public PropInfo GetPropInfo(int id) {
			PropInfo ret;
			if (!Client.User.UserInfo.PropList.TryGetValue(id, out ret)) {
				ret = new PropInfo(Client.Prop[id]);
				Client.User.UserInfo.PropList.Add(id, ret);
			}
			return ret;
		}

		/// <summary>
		/// 装备道具
		/// </summary>
		/// <param name="propId"></param>
		public void EquipProp(int propId) {
			Client.User.UserInfo.EquipedPropList.Clear();
			Client.User.UserInfo.EquipedPropList.Add(propId);
			Client.User.SaveData();
		}

		/// <summary>
		/// 卸载道具
		/// </summary>
		public void RemoveProp() {
			Client.User.UserInfo.EquipedPropList.Clear();
			Client.User.SaveData();
		}

		public bool BuyProp(PropInfo info) {
			var cost = Client.Item.GetItem(info.PropData.Currency.ID);
			if (cost.ChangeAmount(-info.PropData.CurrencyAmount)) {
				info.ChangeAmount(1);
				if (cost.Data.Type == ItemType.Diamond) {
					AnalyticsMgrBase.Ins.Purchase(info.Data.Name, 1, info.PropData.CurrencyAmount);
				}
                AnalyticsMgrBase.Ins.BuyWeapon(info.Data.Name, 1, info.PropData.CurrencyAmount, cost.Data.ID);
                return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// 是否需要显示小红点
		/// </summary>
		/// <returns></returns>
		public bool CheckRedPoint() {
			return Client.User.UserInfo.PropList.Values.Any(info => info.RedPointState == RedPointState.ShouldShow);
		}

		/// <summary>
		/// 设置小红点为已显示
		/// </summary>
		/// <param name="id"></param>
		public void SetRedPointShowed(int id) {
			PropInfo info = GetPropInfo(id);
			info.RedPointState = RedPointState.AlreadyShow;
			Client.Prop.SaveData();
		}
	}

}

