
using System.Collections.Generic;

namespace GameClient
{
	public enum LimitBikeType
	{
		ID = 0,//具体车辆
		Rank = 1//等级
	}

	public class ChampionshipData
	{
		public int Id;
		public int Sort;
		public List<ChampionshipRewardData> GameReward;//比赛奖励
		public List<ChampionshipRewardData> RankReward;//排行榜奖励
		public long StartTime;//开始时间
		public long FinishTime;//结束时间
		public IconData Icon;
		public string Description;
		public LimitBikeType LimitBikeType;
		public string LimitBikeRank;//限制车辆等级
	}


}
