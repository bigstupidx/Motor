using GameClient;

namespace GameUI {
	public class HeroRedPoint : RedPoint {
		public override bool CheckState() {
//			return Client.Hero.CheckRedPoint();
			return false;
		}
	}

}

