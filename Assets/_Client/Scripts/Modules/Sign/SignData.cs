using GameClient;
using Mono.Data.Sqlite;
using XPlugin.Data.SQLite;

public class SignData {

	public int Day;
	public RewardItemInfo Reward;

	public SignData() {
	}

	public SignData(SqliteDataReader reader) {
		Day = (int)reader.GetValue("Day");
		int id = (int)reader.GetValue("ItemID");
		int amount = (int)reader.GetValue("Amount");
		Reward = new RewardItemInfo(id, amount);
	}
}
