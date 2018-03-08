
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class PropData : ItemData {
		public int Sort;
		public PropType PropType;

		/// <summary>
		/// 货币ID
		/// </summary>
		public ItemData Currency;

		/// <summary>
		/// 货币数量
		/// </summary>
		public int CurrencyAmount;


		public float CDTime;

		public PropData() {
		}

		public PropData(SqliteDataReader reader) : base(reader) {
			this.Sort = (int)reader.GetValue("Sort");
			this.PropType = (PropType) reader.GetValue("PropType");
			var cd = reader.GetValue("CDTime");
            CDTime = (float)cd;

			this.Currency = Client.Item[(int)reader.GetValue("CurrencyID")];
			CurrencyAmount = (int)reader.GetValue("CurrencyAmount");
		}
	}


}
