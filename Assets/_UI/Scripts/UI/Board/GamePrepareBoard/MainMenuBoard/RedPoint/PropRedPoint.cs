using GameClient;

namespace GameUI {
	public class PropRedPoint : RedPoint {
		public override bool CheckState() {
//			return Client.Prop.CheckRedPoint();
			return false;
		}
	}

}

