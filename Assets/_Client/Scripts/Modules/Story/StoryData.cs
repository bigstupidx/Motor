
using GameUI;
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;
namespace GameClient {
	public class StoryData {

		public int ID;
		public int MatchID;
		public int IndexInMatch;
		public IconData Icon;
		public string Msg;
		public bool IsLeft;

		public StoryData() {
		}

		public StoryData(SqliteDataReader reader) {
			ID = (int)reader.GetValue("ID");
			MatchID = (int)reader.GetValue("MatchID");
			IndexInMatch = (int)reader.GetValue("IndexInMatch");
			Icon = Client.Icon[(int)reader.GetValue("Icon")];
			Msg = (string)reader.GetValue("Msg");
			IsLeft = (bool)reader.GetValue("IsLeft");
		}
	}


}
