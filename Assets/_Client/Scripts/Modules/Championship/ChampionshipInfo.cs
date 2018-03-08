using XPlugin.Data.Json;

namespace GameClient {
	public class ChampionshipInfo : MatchInfo {
		[JsonIgnore]
		public ChampionshipData ChampionshipData;

		[JsonIgnore]
		public long RemainTime {
			get { return ChampionshipData.FinishTime - Client.System.TimeOnServer; }
		}

		private float _bestTime = 0f;
		public float BestTime {
			get { return _bestTime; }
			set {
				if (_bestTime == 0) {
					_bestTime = value;
				}
				if (value < _bestTime) {
					_bestTime = value;
				}
			}
		}

		public int Rank = -1;//当前排名

		/// <summary>
		/// 领取过奖励
		/// </summary>
		public bool IsGetReward = false;
	}

}

