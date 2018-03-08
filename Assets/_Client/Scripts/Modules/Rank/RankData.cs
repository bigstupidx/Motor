using System;
using XPlugin.Data.Json;
namespace GameClient
{
	public class RankData
	{
		public RegionEnum Region;
		public int PlayerID;
		public int Rank;
		public string NickName;
		public HeroData Hero;
		public BikeData Bike;
		public float RunTime;

		public RankData(){
			
		}

		public RankData(JObject item){
			Rank = item ["ranking"].AsInt ();
			PlayerID = item ["player_id"].AsInt ();
			NickName = PlayerID == Client.User.UserInfo.Setting.UserId
						? Client.User.UserInfo.Setting.Nickname
						: (item ["nickname"].IsValid ? item ["nickname"].AsString () : (LString.ISNETWORKCONNECTED).ToLocalized());
			Hero = Client.Hero [item ["hero_id"].AsInt ()];
			Bike = Client.Bike [item ["bike_id"].AsInt ()];
			RunTime = item ["record_time"].AsFloat ();
			Region = (RegionEnum) Enum.Parse(typeof(RegionEnum),item["country"].AsString(),true);
		}
	}


}
