using System;
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;

namespace GameClient {
	public enum WeaponType {
		/// <summary>
		/// 脚
		/// </summary>
		Foot=1,
		/// <summary>
		/// 棍棒型武器
		/// </summary>
		Stick=2
	}

	public class WeaponData :ItemData{
		public int Sort;
		public string Prefab;

		/// <summary>
		/// 货币
		/// </summary>
		public ItemData Currency;
		public int CurrencyAmount;

		/// <summary>
		/// 攻击范围
		/// </summary>
		public float AtkRange;

		/// <summary>
		/// 特殊能力功能名称
		/// </summary>
		public string[] AbilityFuncName;
		public string AbilityDesc;
		public float[] AbilityValues;

		/// <summary>
		/// 是否消耗
		/// </summary>
		public bool Consum;

		public WeaponType Type;


		public WeaponData() {
		}

		public WeaponData(SqliteDataReader reader) : base(reader) {
			this.Sort = (int) reader.GetValue("Sort");
			this.Prefab = (string) reader.GetValue("Prefab");
			this.Currency = Client.Item[(int) reader.GetValue("CurrencyID")];
			this.CurrencyAmount = (int) reader.GetValue("CurrencyAmount");
			this.AtkRange = (float) reader.GetValue("AtkRange");
			this.AbilityDesc = (string) reader.GetValue("AbilityDesc");

			char[] c = "|".ToCharArray();
			this.AbilityDesc = (string)reader.GetValue("AbilityDesc");
			string[] str;
			str = ((string)reader.GetValue("AbilityFuncName")).Split(c);
			AbilityFuncName = new string[str.Length];
			for (var j = 0; j < str.Length; j++)
			{
				AbilityFuncName[j] = str[j];
			}
			str = ((string)reader.GetValue("AbilityValue")).Split(c);
			this.AbilityValues = new float[str.Length];
			for (int i = 0; i < str.Length; i++) {
				this.AbilityValues[i] = float.Parse(str[i]);
			}
			this.Consum = (bool) reader.GetValue("Consum");
			this.Type = (WeaponType) reader.GetValue("Type");
		}
	}
}