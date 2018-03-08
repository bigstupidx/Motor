using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModHero : ClientModule {

		protected Dictionary<int, HeroData> HeroDataList = new Dictionary<int, HeroData>();

		public List<HeroData> GetSortedDatas() {
			List<HeroData> ret = new List<HeroData>(this.HeroDataList.Values);
			ret.Sort((l, r) => {
				return l.Sort - r.Sort;
			});
			return ret;
		}

		public HeroData Random() {
			return HeroDataList.Random();
		}

		/// <summary>
		/// 获取指定人物数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public HeroData this[int id] {
			get {
				HeroData data;
				this.HeroDataList.TryGetValue(id, out data);
				return data;
			}
		}

		public int MaxLevel {
			get { return Client.System.GetMiscValue<int>("Hero.LevelMax"); }
		}

		#region base
		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ExecuteQuery("select * from Hero left join Item on Item.ID == Hero.ID");
			while (reader.Read()) {
				HeroData data = new HeroData(reader);
				this.HeroDataList.Add(data.ID, data);
			}
		}

		public override void InitInfo(string s) {
			if (string.IsNullOrEmpty(s)) {
				return;
			}
			var heroList = JsonMapper.ToObject<Dictionary<int, HeroInfo>>(s);
			Client.User.UserInfo.HeroList = heroList;
			foreach (var e in heroList) {
				e.Value.InitData(Client.Hero[e.Key]);
			}
		}

		public override void ResetData() {
			this.HeroDataList.Clear();
		}

		public override string ToJson(UserInfo info) {
			if (info == null) {
				return JsonMapper.ToJson(Client.User.UserInfo.HeroList);
			}

			return JsonMapper.ToJson(info.HeroList);
		}
		#endregion

		/// <summary>
		/// 获取已拥有的人物
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public HeroInfo GetHeroInfo(int id) {
			HeroInfo info = null;
			if (Client.User.UserInfo.HeroList.ContainsKey(id)) {
				info = Client.User.UserInfo.HeroList[id];
			} else {
				info = new HeroInfo(this[id]);
			}
			return info;

		}

		/// <summary>
		/// 是否已经拥有某角色
		/// </summary>
		public bool IsAlreadyHave(int id) {
			return Client.User.UserInfo.HeroList.ContainsKey(id);
		}

		/// <summary>
		/// 购买人物
		/// </summary>
		/// <param name="id"></param>
		public bool BuyHero(int id) {
			HeroInfo info = GetHeroInfo(id);
			ItemInfo currency = Client.Item.GetItem(info.Data.CostItem.ID);
			if (currency.ChangeAmount(-info.Data.CostAmount)) {
				if (currency.Data.Type == ItemType.Diamond) {
					AnalyticsMgrBase.Ins.Purchase(info.Data.Name, 1, info.Data.CostAmount * 1.0);
				}
				GainHero(id);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 获得人物
		/// </summary>
		/// <param name="id"></param>
		public void GainHero(int id) {
			if (!Client.User.UserInfo.HeroList.ContainsKey(id)) {
				HeroInfo info = GetHeroInfo(id);
				info.isUnLock = true;
				info.RedPointState = id == DataDef.DefaultHero ? RedPointState.AlreadyShow : RedPointState.ShouldShow;
				Client.User.UserInfo.HeroList.Add(id, info);
				Client.EventMgr.SendEvent(EventEnum.Hero_Unlock, id);
				Client.Hero.SaveData();
			}
		}

		/// <summary>
		/// 人物升级
		/// </summary>
		/// <param name="id"></param>
		public bool UpgradeHero(int id) {
			HeroInfo info = GetHeroInfo(id);
			ItemInfo item = Client.Item.GetItem(info.Data.heroItem.ID);
			if (item.ChangeAmount(-info.Data.LvUpCost.GetValue(info.Level))) {
				if (info != null) {
					info.Level += 1;
					Client.EventMgr.SendEvent(EventEnum.Hero_Upgrade, info.Data.ID);
				}
				Client.Hero.SaveData();
				return true;
			}
			return false;
		}

		/// <summary>
		/// 是否需要显示小红点
		/// </summary>
		/// <returns></returns>
		public bool CheckRedPoint() {
			return Client.User.UserInfo.HeroList.Values.Any(info => info.RedPointState == RedPointState.ShouldShow);
		}

		/// <summary>
		/// 设置小红点为已显示
		/// </summary>
		/// <param name="id"></param>
		public void SetRedPointShowed(int id) {
			HeroInfo info = GetHeroInfo(id);
			if (info.RedPointState == RedPointState.ShouldShow) {
				info.RedPointState = RedPointState.AlreadyShow;
				Client.Hero.SaveData();
			}
		}

		public float GetUpgradeBase(HeroUpgradeType type) {
			switch (type) {
				case HeroUpgradeType.Control:
					return Client.System.GetMiscValue<float>("Hero.InitControlBase");
				case HeroUpgradeType.N2oAdd:
					return Client.System.GetMiscValue<float>("Hero.N2oAddBase");
			}
			return -1;
		}

		public float GetUpgradeMax(HeroUpgradeType type) {
			switch (type) {
				case HeroUpgradeType.Control:
					return Client.System.GetMiscValue<float>("Hero.InitControlMax");
				case HeroUpgradeType.N2oAdd:
					return Client.System.GetMiscValue<float>("Hero.N2oAddMax");
			}
			return -1;
		}
	}


}
