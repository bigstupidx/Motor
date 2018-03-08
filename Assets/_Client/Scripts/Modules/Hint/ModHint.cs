
using System.Collections.Generic;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModHint : ClientModule {

		private Dictionary<int, string> hintList;

		public override void InitData(DbAccess db) {
			hintList = new Dictionary<int, string>();
			var reader = db.ReadFullTable("Hint");
			while (reader.Read()) {
				int id = (int)reader.GetValue("ID");
				string content = (string)reader.GetValue("Content");
				hintList.Add(id, content);
			}

		}

		public string GetHint() {
			return hintList.Random();
		}
	}


}
