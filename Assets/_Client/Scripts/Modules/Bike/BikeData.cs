
using System.Collections.Generic;
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class BikeData :ItemData {
		public int Sort;
		public string Prefab;

		/// <summary>
		/// 货币ID
		/// </summary>
		public ItemData Cost;

		/// <summary>
		/// 售价
		/// </summary>
		public int CostAmount;

		public Dictionary<BikeUpgradeType, BikeUpgradeItemData> UpgradeItemDatas = new Dictionary<BikeUpgradeType, BikeUpgradeItemData>();

		/// <summary>
		/// 特殊能力功能名称
		/// </summary>
		public string[] AbilityFuncName;
		public string AbilityDesc;
		public float[] AbilityValues;

		public ItemData LvUpgradeCost;

		public BikeRank Rank;
        public ItemData MaxCost;
        public int MaxCostNum;
		public BikeData() :base(){
		}

		public BikeData(SqliteDataReader reader):base(reader) {
			this.Sort = (int)reader.GetValue("Sort");
			Prefab = (string)reader.GetValue("Prefab");
			char[] c = "|".ToCharArray();

			string[] str;
			str = ((string)reader.GetValue("LimitNormalSpeed")).Split(c);
			this.UpgradeItemDatas.Add(BikeUpgradeType.LimitNormalSpeed, new BikeUpgradeItemData(str));

			str = ((string)reader.GetValue("LimitBoostSpeed")).Split(c);
			this.UpgradeItemDatas.Add(BikeUpgradeType.LimitBoostSpeed, new BikeUpgradeItemData(str));

			str = ((string)reader.GetValue("Power")).Split(c);
			this.UpgradeItemDatas.Add(BikeUpgradeType.Power, new BikeUpgradeItemData(str));

			str = ((string)reader.GetValue("DriftReduce")).Split(c);
			this.UpgradeItemDatas.Add(BikeUpgradeType.DriftReduce, new BikeUpgradeItemData(str));

			str = ((string)reader.GetValue("Cost")).Split(c);
			Cost = Client.Item[int.Parse(str[0])];
			this.CostAmount = int.Parse(str[1]);

			str = ((string)reader.GetValue("AbilityFuncName")).Split(c);
		    AbilityFuncName = new string[str.Length];
			for (var j = 0; j < str.Length; j++)
			{
				AbilityFuncName[j] = str[j];
			}
            this.AbilityDesc = (string)reader.GetValue("AbilityDesc");
            str = ((string)reader.GetValue("AbilityValue")).Split(c);
            this.AbilityValues=new float[str.Length];
			for (int i = 0; i < str.Length; i++) {
				this.AbilityValues[i] = float.Parse(str[i]);
			}
			this.LvUpgradeCost = Client.Item[(int) reader.GetValue("LvUpgradeCost")];
			this.Rank = (BikeRank) reader.GetValue("Rank");
            str = ((string)reader.GetValue("MaxCost")).Split(c);
            MaxCost = Client.Item[int.Parse(str[0])];
            MaxCostNum = int.Parse(str[1]);
        }
	}
}
