namespace GameClient {
	public class ChallengeInfo : MatchInfo {
		public long LastTime {
			get { return _lastTime; }
			set {
				_lastTime = value;
			}
		}

		private long _lastTime = 0;
		public ChallengeInfo() {

		}
		public ChallengeInfo(ChallengeData data) : base(data) {
			this.MatchMode = MatchMode.Challenge;
		}

		public int hasPlay = 0;

		public bool OutOfTimes() {
			return ((ChallengeData)Data).MatchTimes > hasPlay;
		}

		public override void SetStoryPlayed() {
//			base.SetStoryPlayed();
//			Client.Challenge.SaveData();
		}
	}
}
