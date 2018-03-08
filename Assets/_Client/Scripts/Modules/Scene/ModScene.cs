using System.Collections.Generic;
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModScene :ClientModule{

		public Dictionary<int,SceneData> SceneDatas=new Dictionary<int, SceneData>();

		public SceneData this[int id] {
			get {
				SceneData data;
				this.SceneDatas.TryGetValue(id, out data);
				return data;
			}
		}

		public override void InitData(DbAccess db) {
			base.InitData(db);
			SqliteDataReader reader = db.ReadFullTable("Scene");
			while (reader.Read()) {
				SceneData data = new SceneData(reader);
				this.SceneDatas.Add(data.ID, data);
			}
		}
	}
}