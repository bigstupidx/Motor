using System.Collections.Generic;
using XPlugin.Data.Json;

namespace GameClient {
	public class ChapterInfo {
		[JsonIgnore]
		public ChapterData Data;

		public bool IsRewarded = false;

		public Dictionary<int, MatchInfo> matchInfoList = new Dictionary<int, MatchInfo>();

		public ChapterInfo()
		{
			
		}

		public ChapterInfo(ChapterData data) {
			this.Data = data;
			matchInfoList = new Dictionary<int, MatchInfo>();
			foreach (var matchData in data.Matches) {
				this.matchInfoList.Add(matchData.Key,new MatchInfo(matchData.Value));
            }
		}


		public List<MatchInfo> GetSortedMatches() {
			List<MatchInfo> ret = new List<MatchInfo>(this.matchInfoList.Values);
			ret.Sort((l, r) => {
				return l.Data.Index - r.Data.Index;
			});

			return ret;
		}

		public bool IsGetAllStars()
		{
			foreach (var info in matchInfoList.Values)
			{
				if (info.GetStarCount() != 3)
				{
					return false;
				}
			}
			return true;
		}

		public int TotleGetStars(){
			int count = 0;
			foreach (var info in matchInfoList.Values) {
				if(info.IsUnlocked()){
					count += info.GetStarCount ();
				}
			}
			return count;
		}

		public int TotleStars(){
			int count = 0;
			foreach (var info in matchInfoList.Values) {
				count += info.Data.LevelTasks.Length;
			}
			return count;
		}
	}
}