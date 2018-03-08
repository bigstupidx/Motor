using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ItemData {
		/// <summary>
		/// 物品ID
		/// </summary>
		public int ID;

		/// <summary>
		/// 物品名称
		/// </summary>
		public string Name;

		public IconData Icon;

		/// <summary>
		/// 物品描述
		/// </summary>
		public string Description;

		/// <summary>
		/// 物品类型
		/// </summary>
		public ItemType Type;

		public ItemData() {
		}

		public ItemData(SqliteDataReader reader) {
			this.ID = (int)reader.GetValue("ID");
			this.Name = (string)reader.GetValue("Name");
			this.Icon = Client.Icon[(int)reader.GetValue("Icon")];
			this.Description = (string)reader.GetValue("Description");
			this.Type = (ItemType) reader.GetValue("ItemType");

			Client.Item.CacheData(this);
		}


	}
}
