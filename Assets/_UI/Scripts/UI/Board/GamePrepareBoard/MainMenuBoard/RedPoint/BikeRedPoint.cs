using GameClient;

namespace GameUI
{
	public class BikeRedPoint : RedPoint {
		public override bool CheckState()
		{
//			return Client.Bike.CheckRedPoint();
			return false;
		}
	}

}

