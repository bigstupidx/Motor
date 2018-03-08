
using System.Collections.Generic;
using System.Text;
using XPlugin.Data.Json;

namespace GameClient {
	public enum RedPointState {
		NotYetShow = 0,
		ShouldShow = 1,
		AlreadyShow = 2
	}

	public class BikeInfo {
		[JsonIgnore]
		public BikeData Data;
		public bool isUnLock = false;
		public Dictionary<BikeUpgradeType, int> UpgradeItemLv = new Dictionary<BikeUpgradeType, int>();
		public RedPointState RedPointState = RedPointState.NotYetShow;

		/// <summary>
		/// 车辆是否全部属性满级
		/// </summary>
		[JsonIgnore]
		public bool IsAllUpgradeMax {
			get {
				bool ret = true;
				foreach (var i in this.UpgradeItemLv.Values) {
					if (i < Client.Bike.LevelMax) {
						ret = false;
						break;
					}
				}
				return ret;
			}
		}

		public BikeInfo() {
		}

		public BikeInfo(BikeData data, int limitNormalSpeedLv = 0, int limitBoostLv = 0, int powerLv = 0, int driftReduceLv = 0) {
			this.Data = data;
			this.UpgradeItemLv = new Dictionary<BikeUpgradeType, int>() {
				{BikeUpgradeType.LimitNormalSpeed, limitNormalSpeedLv },
				{BikeUpgradeType.LimitBoostSpeed, limitBoostLv},
				{BikeUpgradeType.Power, powerLv},
				{BikeUpgradeType.DriftReduce, driftReduceLv},
			};
		}

		public void InitData(BikeData data, bool needInitUpgradItemLv = true) {
			this.Data = data;
			if (needInitUpgradItemLv) {
				this.UpgradeItemLv = new Dictionary<BikeUpgradeType, int>()
				{
					{BikeUpgradeType.LimitNormalSpeed, 0},
					{BikeUpgradeType.LimitBoostSpeed, 0},
					{BikeUpgradeType.Power, 0},
					{BikeUpgradeType.DriftReduce, 0},
				};
			}
		}

		public JObject ToJson() {
			JObject ret = new JObject();
			ret["ID"] = this.Data.ID;
			ret["DriftReduce"] = this.UpgradeItemLv[BikeUpgradeType.DriftReduce];
			ret["LimitBoostSpeed"] = this.UpgradeItemLv[BikeUpgradeType.LimitBoostSpeed];
			ret["LimitNormalSpeed"] = this.UpgradeItemLv[BikeUpgradeType.LimitNormalSpeed];
			ret["Power"] = this.UpgradeItemLv[BikeUpgradeType.Power];
			return ret;
		}

		public BikeInfo(JObject json) :
			this(Client.Bike[json["ID"].AsInt()],
				json["LimitNormalSpeed"].AsInt(),
				json["LimitBoostSpeed"].AsInt(),
				json["Power"].AsInt(),
				json["DriftReduce"].AsInt()) {
		}

	}
}
