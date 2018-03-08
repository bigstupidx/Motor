using System;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class IAPData : BuyItemData {

		public IAPType Type;

		public ItemData Item;
		/// <summary>
		/// 数量
		/// </summary>
		public int Amount;
		/// <summary>
		/// 返利
		/// </summary>
		public int Rebate;
		/// <summary>
		/// 货币
		/// </summary>
		public ItemData Currency;
		/// <summary>
		/// 价格
		/// </summary>
		public int CurrencyAmount;

		public IAPData(SqliteDataReader reader) : base(reader) {
			Type = (IAPType)reader.GetValue("IAPType");
			Item = Client.Item[(int)reader.GetValue("Item")];
			Amount = (int)reader.GetValue("Amount");
			this.Currency = Client.Item[(int)reader.GetValue("CurrencyID")];
			CurrencyAmount = (int)reader.GetValue("CurrencyAmount");
			Rebate = (int)reader.GetValue("Rebate");
		}

		public override SDKPayInfo GetPayInfo() {
			return new SDKPayInfo() {
				Id = this.ID,
				Amount = 1,
				Desc = this.Name,
				Name = this.Name,
				Price = this.CurrencyAmount
			};
		}
	}

}

