using System.Collections.Generic;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModIcon : ClientModule {

		public Dictionary<int, IconData> IconList = new Dictionary<int, IconData>();

		public IconData this[int id] {
			get {
				IconData data;
				this.IconList.TryGetValue(id, out data);
				return data;
			}
		}

		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ReadFullTable("Icon");
			while (reader.Read()) {
				IconData data = new IconData(reader);
				this.IconList.Add(data.ID, data);
			}
		}

		public override void ResetData() {
			this.IconList.Clear();
		}
	}
}