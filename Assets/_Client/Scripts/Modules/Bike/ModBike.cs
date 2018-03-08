
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModBike : ClientModule {

		protected Dictionary<int, BikeData> BikeDataList = new Dictionary<int, BikeData>();


		public List<BikeData> GetSortedDatas() {
			List<BikeData> ret = new List<BikeData>(this.BikeDataList.Values);
			ret.Sort((l, r) => {
				return l.Sort - r.Sort;
			});
			return ret;
		}

		public BikeData Random() {
			return BikeDataList.Random();
		}

		/// <summary>
		/// 获取指定赛车数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public BikeData this[int id] {
			get {
				BikeData data;
				BikeDataList.TryGetValue(id, out data);
				return data;
			}
		}

		/// <summary>
		/// 车辆最高等级
		/// </summary>
		public int LevelMax {
			get { return Client.System.GetMiscValue<int>("Bike.LevelMax"); }
		}

		public float GetUpgradeBase(BikeUpgradeType type) {
			switch (type) {
				case BikeUpgradeType.DriftReduce:
					return Client.System.GetMiscValue<float>("Bike.DriftReduceBase");
				case BikeUpgradeType.LimitNormalSpeed:
					return Client.System.GetMiscValue<float>("Bike.NormalSpeedBase");
				case BikeUpgradeType.Power:
					return Client.System.GetMiscValue<float>("Bike.PowerBase");
				case BikeUpgradeType.LimitBoostSpeed:
					return Client.System.GetMiscValue<float>("Bike.BoostSpeedBase");
			}
			return -1;
		}

		public float GetUpgradeMax(BikeUpgradeType type) {
			switch (type) {
				case BikeUpgradeType.DriftReduce:
					return Client.System.GetMiscValue<float>("Bike.DriftReduceMax");
				case BikeUpgradeType.LimitNormalSpeed:
					return Client.System.GetMiscValue<float>("Bike.NormalSpeedMax");
				case BikeUpgradeType.Power:
					return Client.System.GetMiscValue<float>("Bike.PowerMax");
				case BikeUpgradeType.LimitBoostSpeed:
					return Client.System.GetMiscValue<float>("Bike.BoostSpeedMax");
			}
			return -1;
		}

		#region base

		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ExecuteQuery("select * from Bike left join Item on Item.ID == Bike.ID");
			while (reader.Read())
			{
				BikeData data = new BikeData(reader);
				BikeDataList.Add(data.ID, data);
			}

		}

		public override void InitInfo(string s) {
			if (string.IsNullOrEmpty(s))
			{
				return;
			}
			var bikeList = JsonMapper.ToObject<Dictionary<int, BikeInfo>>(s);
			//			Debug.Log(JsonMapper.ToJson(bikeList));
			Client.User.UserInfo.BikeList = bikeList;
			foreach (var e in Client.User.UserInfo.BikeList)
			{
				//				Debug.Log("==>already own bike:" + e.Key);
				e.Value.InitData(Client.Bike[e.Key], false);
			}
		}

		public override void ResetData() {
			BikeDataList.Clear();
		}

		public override string ToJson(UserInfo info) {
			if (info == null)
			{
				return JsonMapper.ToJson(Client.User.UserInfo.BikeList);
			}

			return JsonMapper.ToJson(info.BikeList);
		}
		#endregion

		/// <summary>
		/// 获取车辆信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public BikeInfo GetBikeInfo(int id) {
			BikeInfo info = null;
			if (Client.User.UserInfo.BikeList.ContainsKey(id)) {
				info = Client.User.UserInfo.BikeList[id];
			} else {
				info = new BikeInfo(this[id]);
			}
			return info;
		}

		/// <summary>
		/// 是否已经拥有某辆车
		/// </summary>
		public bool IsAlreadyHave(int id) {
			return Client.User.UserInfo.BikeList.ContainsKey(id);
		}

		/// <summary>
		/// 购买车辆
		/// </summary>
		/// <param name="id"></param>
		public bool BuyBike(int id) {
			BikeInfo info = GetBikeInfo(id);
			ItemInfo cost = Client.Item.GetItem(info.Data.Cost.ID);
			if (cost.ChangeAmount(-info.Data.CostAmount)) {
				if (cost.Data.Type == ItemType.Diamond)
				{
					 AnalyticsMgrBase.Ins.Purchase(info.Data.Name, 1, info.Data.CostAmount * 1.0);
				}
				GainBike(id);
				return true;
			} else {
				return false;
			}

		}

		/// <summary>
		/// 获得车辆
		/// </summary>
		/// <param name="id"></param>
		public void GainBike(int id) {
			if (!Client.User.UserInfo.BikeList.ContainsKey(id))
			{
				BikeInfo info = GetBikeInfo(id);
				info.isUnLock = true;
				info.RedPointState = id == DataDef.DefaultBike? RedPointState.AlreadyShow : RedPointState.ShouldShow;
				Client.User.UserInfo.BikeList.Add(id, info);
				Client.EventMgr.SendEvent(EventEnum.Bike_Unlock, id);
				Client.Bike.SaveData();
			}
		}

		/// <summary>
		/// 车辆属性升级
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		public bool UpgradeBike(int id, BikeUpgradeType type) {
			BikeInfo info = GetBikeInfo(id);
			ItemInfo currency = Client.Item.GetItem(info.Data.LvUpgradeCost.ID);
			if (currency.ChangeAmount(-info.Data.UpgradeItemDatas[type].GetCost(info.UpgradeItemLv[type]))) {
				info.UpgradeItemLv[type]++;
				Client.EventMgr.SendEvent(EventEnum.Bike_Upgrade, info.Data.ID, (int)type);
				Client.Bike.SaveData();
				return true;
			} else {
				return false;
			}
		}

        /// <summary>
        /// 一键车辆属性升级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public bool UpgradeMaxBike(int id)
        {
            BikeInfo info = GetBikeInfo(id);
            ItemInfo currency = Client.Item.GetItem(info.Data.MaxCost.ID);
            if (currency.ChangeAmount(-info.Data.MaxCostNum))
            {
                for(int i = 0;i<= (int)BikeUpgradeType.Power; i++)
                {
                    int lv = info.UpgradeItemLv[(BikeUpgradeType)i];
                    for (; lv < Client.Bike.LevelMax;)
                    {
                        info.UpgradeItemLv[(BikeUpgradeType)i]++;
                        Client.EventMgr.SendEvent(EventEnum.Bike_Upgrade, info.Data.ID, i);
                        lv = info.UpgradeItemLv[(BikeUpgradeType)i];
                    }
                }
                Client.Bike.SaveData();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否已经升级满了
        /// </summary>
        /// <returns></returns>
        public bool IsMaxBike(int id)
        {
            BikeInfo info = GetBikeInfo(id);
            for (int i = 0; i <= (int)BikeUpgradeType.Power; i++)
            {
                int lv = info.UpgradeItemLv[(BikeUpgradeType)i];
                if(lv< Client.Bike.LevelMax)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取车辆评分
        /// </summary>
        /// <returns></returns>
        public int GetBikePowerSocre(int id){
            float total = 0;
            BikeInfo info = GetBikeInfo(id);
            BikeUpgradeItemData NoramlData = info.Data.UpgradeItemDatas[BikeUpgradeType.LimitNormalSpeed];
            BikeUpgradeItemData BoostData = info.Data.UpgradeItemDatas[BikeUpgradeType.LimitBoostSpeed];
            BikeUpgradeItemData PowerData = info.Data.UpgradeItemDatas[BikeUpgradeType.Power];
            BikeUpgradeItemData DirftData = info.Data.UpgradeItemDatas[BikeUpgradeType.DriftReduce];
            float noraml = NoramlData.GetValue(info.UpgradeItemLv[BikeUpgradeType.LimitNormalSpeed]);
            float boost = BoostData.GetValue(info.UpgradeItemLv[BikeUpgradeType.LimitBoostSpeed]);
            float power = PowerData.GetValue(info.UpgradeItemLv[BikeUpgradeType.Power]);
            float dirft = DirftData.GetValue(info.UpgradeItemLv[BikeUpgradeType.DriftReduce]);
            total += Client.System.GetMiscValue<float>("Speed.Weight") * noraml;
            total += Client.System.GetMiscValue<float>("N2o.Weight") * boost;
            total += Client.System.GetMiscValue<float>("Accelerate.Weight") * power;
            total += Client.System.GetMiscValue<float>("Operation.Weight") * dirft;
            return (int)total;
		}

		/// <summary>
		/// 是否需要显示小红点
		/// </summary>
		/// <returns></returns>
		public bool CheckRedPoint() {
			return Client.User.UserInfo.BikeList.Values.Any(info => info.RedPointState == RedPointState.ShouldShow);
		}

		/// <summary>
		/// 设置小红点为已显示
		/// </summary>
		/// <param name="id"></param>
		public void SetRedPointShowed(int id) {
			BikeInfo info = GetBikeInfo(id);
			if (info.RedPointState == RedPointState.ShouldShow)
			{
				info.RedPointState = RedPointState.AlreadyShow;
				Client.Bike.SaveData();
			}
		}

	}


}
