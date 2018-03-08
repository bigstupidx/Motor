using GameClient;

namespace GameUI {
	public class WeaponRedPoint : RedPoint {
		public override bool CheckState() {
			//			return Client.Weapon.CheckRedPoint();
			return false;
		}
	}

}

