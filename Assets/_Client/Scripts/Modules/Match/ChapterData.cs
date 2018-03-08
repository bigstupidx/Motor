
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ChapterData {
		public int ID;
		public int Index;
		public string Name;
		public string Icon;
		public string Description;

		public int UnlockStarCount;

		/// <summary>
		/// 货币
		/// </summary>
		public ItemData Currency;

		/// <summary>
		/// 货币数量
		/// </summary>
		public int CurrencyAmount;

		public List<RewardItemInfo> RewardList;//完美通关奖励

		public IconData LevelBg;
		public UILevelLineData UILine;
		
		public Dictionary<int, MatchData> Matches = new Dictionary<int, MatchData>();

		public ChapterData(SqliteDataReader reader) {
			ID = (int)reader.GetValue("ID");
			Index = (int)reader.GetValue("ChapterIndex");
			Name = (string)reader.GetValue("Name");
			Icon = (string)reader.GetValue("Icon");
			Description = (string)reader.GetValue("Description");
			UnlockStarCount = (int) reader.GetValue("UnlockStarCount");
			this.Currency = Client.Item[(int)reader.GetValue("CurrencyID")];
			this.CurrencyAmount = (int)reader.GetValue("CurrencyAmount");

			string str = (string)reader.GetValue("RewardList");
			JArray json = JArray.Parse(str);
			RewardList = new List<RewardItemInfo>();
			for (int i = 0; i < json.Count; i++)
			{
				int id = json[i][0].AsInt();
				int amount = json[i][1].AsInt();
				RewardList.Add(new RewardItemInfo(id, amount));
			}

			LevelBg = Client.Icon[(int)reader.GetValue("LevelBg")];
			var s = (string) reader.GetValue("LevelLine");
			if (!string.IsNullOrEmpty(s))
			{
				var lineJson = JArray.Parse(s);
				UILine = new UILevelLineData(lineJson);
			}

			

		}
	}
}
