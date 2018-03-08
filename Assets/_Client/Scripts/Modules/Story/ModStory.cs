
using System.Collections.Generic;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModStory : ClientModule {
		protected Dictionary<int, StoryData> StoryDataList = new Dictionary<int, StoryData>();

		public StoryData this[int id] {
			get {
				StoryData data;
				StoryDataList.TryGetValue(id, out data);
				return data;
			}
		}

		public List<StoryData> GetStoryByMatchID(int id) {
			List<StoryData> list = new List<StoryData>();
			foreach (var data in StoryDataList.Values) {
				if (data.MatchID == id) {
					list.Add(data);
				}
			}
			list.Sort((p1, p2) => p1.IndexInMatch - p2.IndexInMatch);
			return list;
		}

		public override void InitData(DbAccess db) {
			var reader = db.ReadFullTable("Story");
			while (reader.Read()) {
				StoryData data = new StoryData(reader);
				StoryDataList.Add(data.ID, data);
			}
		}

		public override void ResetData() {
			StoryDataList.Clear();
		}
	}
}

