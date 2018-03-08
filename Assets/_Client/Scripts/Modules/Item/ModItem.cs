using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModItem : ClientModule {
		protected Dictionary<int, ItemData> itemList = new Dictionary<int, ItemData>();

		/// <summary>
		/// 获取指定物品数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ItemData this[int id] {
			get {
				ItemData data;
				itemList.TryGetValue(id, out data);
				return data;
			}
		}


		/// <summary>
		/// 金币Data
		/// </summary>
		public ItemData CoinData;

		/// <summary>
		/// 钻石ID
		/// </summary>
		public ItemData DiamondData;

		/// <summary>
		/// 体力ID
		/// </summary>
		public ItemData StaminaData;

		/// <summary>
		/// 人民币Data
		/// </summary>
		public ItemData RMBData;

		/// <summary>
		/// 缓存常用物品
		/// </summary>
		/// <param name="data"></param>
		public void CacheData(ItemData data) {
			if (data.Type == ItemType.Coin) {
				this.CoinData = data;
			} else if (data.Type == ItemType.Diamond) {
				this.DiamondData = data;
			} else if (data.Type == ItemType.Stamina) {
				this.StaminaData = data;
			} else if (data.Type == ItemType.RMB) {
				this.RMBData = data;
			}
		}


		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ReadFullTable("Item");
			while (reader.Read()) {
				ItemData data = new ItemData(reader);
				itemList.Add(data.ID, data);
			}

		}

		public override void InitInfo(string s) {
			base.InitInfo(s);
			var itemInfos = JsonMapper.ToObject<Dictionary<int, ItemInfo>>(s);
			if (itemInfos == null) {
				itemInfos = new Dictionary<int, ItemInfo>();
			}
			Client.User.UserInfo.OwnedItems = itemInfos;
			foreach (var ownedItem in Client.User.UserInfo.OwnedItems) {
				ownedItem.Value.InitData(this.itemList[ownedItem.Key]);
			}
		}

		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.OwnedItems);
		}

		public override void ResetData() {
			itemList.Clear();
		}


		/// <summary>
		/// 快捷获取金币
		/// </summary>
		[JsonIgnore]
		public ItemInfo Coin {
			get {
				int id = Client.Item.CoinData.ID;
				ItemInfo info;
				if (!Client.User.UserInfo.OwnedItems.TryGetValue(id, out info)) {
					info = new ItemInfo(id);
					Client.User.UserInfo.OwnedItems.Add(id, info);
				}
				return info;
			}
		}

		/// <summary>
		/// 快捷获取钻石
		/// </summary>
		[JsonIgnore]
		public ItemInfo Diamond {
			get {
				int id = Client.Item.DiamondData.ID;
				ItemInfo info;
				if (!Client.User.UserInfo.OwnedItems.TryGetValue(id, out info)) {
					info = new ItemInfo(id);
					Client.User.UserInfo.OwnedItems.Add(id, info);
				}
				return info;
			}
		}

		/// <summary>
		/// 获取物品
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ItemInfo GetItem(int id) {
			ItemInfo info;
			if (!Client.User.UserInfo.OwnedItems.TryGetValue(id, out info)) {
				info = new ItemInfo(id);
				Client.User.UserInfo.OwnedItems.Add(id, info);
			}
			return info;
		}

		/// <summary>
		/// 领取奖励,可选择已拥有时是否折算为货币
		/// </summary>
		/// <param name="rewardList"></param>
		public void GetRewards(List<RewardItemInfo> rewardList, bool reduceToCost) {
			foreach (var i in rewardList) {
				GainItem(i.Data, i.Amount, reduceToCost);
			}
		}

		/// <summary>
		/// 获取物品，会自动处理车辆，角色还是其他。可选择已拥有时是否折算为货币
		/// </summary>
		public void GainItem(int itemId, int amount, bool reduceToCost) {
			GainItem(this[itemId], amount, reduceToCost);
		}

		/// <summary>
		/// 获取物品，会自动处理车辆，角色还是其他。可选择已拥有时是否折算为货币
		/// </summary>
		public void GainItem(ItemData itemData, int amount, bool reduceToCost) {
			ItemInfo itemInfo = null;
			switch (itemData.Type) {
				case ItemType.Bike:
					if (reduceToCost) {
						if (Client.Bike.IsAlreadyHave(itemData.ID)) {
							BikeData bikeData = Client.Bike[itemData.ID];
							itemInfo = Client.Item.GetItem(bikeData.Cost.ID);
							amount = bikeData.CostAmount;
						}
					}
					Client.Bike.GainBike(itemData.ID);
					break;
				case ItemType.Hero:
					if (reduceToCost) {
						if (Client.Hero.IsAlreadyHave(itemData.ID)) {
							HeroData heroData = Client.Hero[itemData.ID];
							itemInfo = Client.Item.GetItem(heroData.CostItem.ID);
							amount = heroData.CostAmount;
						}
					}
					Client.Hero.GainHero(itemData.ID);
					break;
				case ItemType.Prop:
					itemInfo = Client.Prop.GetPropInfo(itemData.ID);
					break;
				case ItemType.Weapon:
					itemInfo = Client.Weapon.GetWeaponInfo(itemData.ID);
					break;
				case ItemType.Stamina:
					Client.User.UserInfo.Stamina.ChangeValue(amount);
					break;
				default:
					itemInfo = Client.Item.GetItem(itemData.ID);
					break;
			}
			if (itemInfo != null) {
				itemInfo.ChangeAmount(amount);
			}
		}
	}
}
