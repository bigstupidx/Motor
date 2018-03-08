using System;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class BuyItemData {
		public int ID;
		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 热销标志
		/// </summary>
		public bool IsHot;

		/// <summary>
		/// 图标
		/// </summary>
		public IconData Icon;

		public int PayCode;

		public BuyItemData() { }

		public BuyItemData(SqliteDataReader reader) {
			ID = (int)reader.GetValue("ID");
			Name = (string)reader.GetValue("Name");
			IsHot = (bool)reader.GetValue("IsHot");
			Icon = Client.Icon[(int)reader.GetValue("Icon")];
			PayCode = (int)reader.GetValue("PayCode");
		}


		public void Pay(Action<bool> onDone) {
			if (SDKPay.Ins != null) {
				SDKPay.Ins.Pay(this, onDone);
			} else {
				Debug.LogError("==>SDKPay未挂载");
			}
		}

		public virtual SDKPayInfo GetPayInfo() {
			return null;
		}
	}


}