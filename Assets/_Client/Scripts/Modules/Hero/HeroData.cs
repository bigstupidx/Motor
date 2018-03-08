
using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.SQLite;

namespace GameClient {

	public enum HeroUpgradeType {
		N2oAdd, Control
	}

	public class HeroData : ItemData {
		public int Sort;
		public string Prefab;

		/// <summary>
		/// 购买所需货币
		/// </summary>
		public ItemData CostItem;

		/// <summary>
		/// 售价
		/// </summary>
		public int CostAmount;

		public Dictionary<HeroUpgradeType, FloatLevelData> UpgradeDatas = new Dictionary<HeroUpgradeType, FloatLevelData>();

		/// <summary>
		/// 特殊能力功能名称
		/// </summary>
		public string[] AbilityFuncName;
		/// <summary>
		/// 特殊能力描述
		/// </summary>
		public string AbilityDesc;
		/// <summary>
		/// 特殊能力数值
		/// </summary>
		public FloatLevelData[] AbilityValue;

		/// <summary>
		///升级所需花费
		/// </summary>
		public ItemData heroItem;
		public IntLevelData LvUpCost;

		public float GetAbilityValue(int index, int lv) {
			if (index > this.AbilityValue.Length) {
				throw new ArgumentOutOfRangeException("index", (LString.GAMECLIENT_HERODATA_GETABILITYVALUE).ToLocalized() + index + "/" + this.AbilityValue.Length);
			}
			return this.AbilityValue[index].GetValue(lv);
		}

		public HeroData() : base() {
		}

		public HeroData(SqliteDataReader reader) : base(reader) {
			this.Sort = (int)reader.GetValue("Sort");
			Prefab = (string)reader.GetValue("Prefab");

			char[] c = "|".ToCharArray();
			char[] underline = "_".ToCharArray();

			this.UpgradeDatas = new Dictionary<HeroUpgradeType, FloatLevelData>();

			string[] str = ((string)reader.GetValue("N2oAdd")).Split(c);
			this.UpgradeDatas.Add(HeroUpgradeType.N2oAdd, new FloatLevelData(str));

			str = ((string)reader.GetValue("Control")).Split(c);
			this.UpgradeDatas.Add(HeroUpgradeType.Control, new FloatLevelData(str));


			str = ((string)reader.GetValue("AbilityFuncName")).Split(c);
			AbilityFuncName = new string[str.Length];
			for (var j = 0; j < str.Length; j++) {
				AbilityFuncName[j] = str[j];
			}
			this.AbilityDesc = (string)reader.GetValue("AbilityDesc");
			str = ((string)reader.GetValue("AbilityValue")).Split(underline);
			this.AbilityValue = new FloatLevelData[str.Length];
			int i = 0;
			foreach (var s in str) {
				var v = s.Split(c);
				this.AbilityValue[i] = new FloatLevelData(v);
				i++;
			}
			str = ((string)reader.GetValue("LvUpCost")).Split(c);
			this.heroItem = Client.Item[int.Parse(str[0])];
			this.LvUpCost = new IntLevelData(new string[] { str[1], str[2] });

			str = ((string)reader.GetValue("Cost")).Split(c);
			this.CostItem = Client.Item[int.Parse(str[0])];
			this.CostAmount = int.Parse(str[1]);

		}

	}
}
