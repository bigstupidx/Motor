using GameUI;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.SQLite;
using XPlugin.Update;

namespace GameClient {
	public class IconData {
		public int ID;
		public string AtlasName;
		public string SpriteName;

		public IconData(SqliteDataReader reader) {
			ID = (int)reader.GetValue("ID");
			AtlasName = (string)reader.GetValue("AtlasName");
			SpriteName = (string)reader.GetValue("SpriteName");
		}

		public Sprite Sprite {
			get { return AtlasManager.GetSprite(this.AtlasName, this.SpriteName); }
		}

		public Texture Texture {
			get { return UResources.Load<Texture>(this.SpriteName); }
		}
	}
}