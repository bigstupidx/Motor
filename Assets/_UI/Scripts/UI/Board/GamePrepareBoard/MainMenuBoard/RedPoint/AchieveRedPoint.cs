using GameClient;

namespace GameUI {
	public class AchieveRedPoint : RedPoint {
		public override bool CheckState() {
			return Client.Task.HaveReward();
		}
	}
}

