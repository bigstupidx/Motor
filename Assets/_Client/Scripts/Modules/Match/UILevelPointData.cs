
using XPlugin.Data.Json;

namespace GameClient 
{
	public class UILevelPointData {
		
		public float PosX;
		public float PosY;

		public UILevelPointData(float x, float y)
		{
			PosX = x;
			PosY = y;
		}

		public UILevelPointData(JArray json)
		{
			PosX = json[0].AsFloat();
			PosY = json[1].AsFloat();
		}
	}

}
