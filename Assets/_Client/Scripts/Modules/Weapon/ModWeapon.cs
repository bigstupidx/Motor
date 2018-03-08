using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModWeapon : ClientModule {

		protected Dictionary<int, WeaponData> WeaponDataList = new Dictionary<int, WeaponData>();

		public List<WeaponInfo> GetSortedInfo() {
			List<WeaponInfo> ret = new List<WeaponInfo>();

			foreach (var weaponData in this.WeaponDataList) {
				ret.Add(GetWeaponInfo(weaponData.Key));
			}
			ret.Sort((l, r) => l.WeaponData.Sort - r.WeaponData.Sort);
			return ret;
		}

		/// <summary>
		/// 获取指定道具数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public WeaponData this[int id] {
			get {
				WeaponData data;
				this.WeaponDataList.TryGetValue(id, out data);
				return data;
			}
		}

		public WeaponInfo GetWeaponInfo(int id) {
			WeaponInfo ret = null;
			if (!Client.User.UserInfo.WeaponList.TryGetValue(id, out ret)) {
				var data = Client.Weapon[id];
                ret = new WeaponInfo(data);
			}
			return ret;
		}

		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ExecuteQuery("select * from Weapon left join Item on Item.ID == Weapon.ID");
			while (reader.Read()) {
				WeaponData data = new WeaponData(reader);
				this.WeaponDataList.Add(data.ID, data);
			}
		}

		public override void ResetData() {
			this.WeaponDataList.Clear();
		}

		public override void InitInfo(string s) {
			base.InitInfo(s);
			var weaponInfoList = JsonMapper.ToObject<Dictionary<int, WeaponInfo>>(s);
			if (weaponInfoList == null) {
				weaponInfoList = new Dictionary<int, WeaponInfo>();
			}
			Client.User.UserInfo.WeaponList = weaponInfoList;
			foreach (var e in Client.User.UserInfo.WeaponList) {
				e.Value.InitData(Client.Weapon[e.Key]);
			}
		}
		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.WeaponList);
		}

		public bool BuyWeapon(WeaponInfo info) {
			if (!info.WeaponData.Consum) {//不消耗的武器不能购买
				return false;
			}
			var cost = Client.Item.GetItem(info.WeaponData.Currency.ID);
			if (cost.ChangeAmount(-info.WeaponData.CurrencyAmount)) {
				info.ChangeAmount(1);
				if (cost.Data.Type == ItemType.Diamond)
				{
					AnalyticsMgrBase.Ins.Purchase(info.Data.Name, 1, info.WeaponData.CurrencyAmount);
				}
                AnalyticsMgrBase.Ins.BuyWeapon(info.Data.Name, 1, info.WeaponData.CurrencyAmount, cost.Data.ID);
				return true;
			}
			else {
				return false;
			}
		}

		public WeaponData Random()
		{
			return this.WeaponDataList.Random();
		}

		/// <summary>
		/// 是否需要显示小红点
		/// </summary>
		/// <returns></returns>
		public bool CheckRedPoint() {
			return Client.User.UserInfo.WeaponList.Values.Any(info => info.RedPointState == RedPointState.ShouldShow);
		}

		/// <summary>
		/// 设置小红点为已显示
		/// </summary>
		/// <param name="id"></param>
		public void SetRedPointShowed(int id) {
			WeaponInfo info = GetWeaponInfo(id);
			info.RedPointState = RedPointState.AlreadyShow;
			Client.Weapon.SaveData();
		}
	}
}