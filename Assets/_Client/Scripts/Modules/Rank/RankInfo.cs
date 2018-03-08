using System.Collections.Generic;
using XPlugin.Data.Json;
namespace GameClient
{
	public class RankInfo
	{
		public List<RankData> ListItems;
		public List<RankData> LocalListItems;
		public RankData SelfData;
		public RankData LocalSelfData;
		public int ID;
		public string Name;
		public int sort;

		public RankInfo(JArray data){
			ID = data [0].AsInt ();
			sort = data [1].AsInt ();
			Name = data [2].AsString ();
			ListItems = new List<RankData> ();
			var dataArray = data [3].OptArray ();
			foreach(JObject item in dataArray){
				ListItems.Add (new RankData (item));
			}
			if(!data[4].IsNull){
				SelfData = new RankData (data [4].AsObject());
			}else{
				SelfData = null;
			}

			LocalListItems = new List<RankData>();
			LocalSelfData = null;

		}

		public void SetLocalData(List<RankData> list, RankData self)
		{
			LocalListItems = list;
			LocalSelfData = self;
		}
	}

}
