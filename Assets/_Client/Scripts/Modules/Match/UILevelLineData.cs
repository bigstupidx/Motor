using System.Collections.Generic;
using XPlugin.Data.Json;

namespace GameClient {
	public class UILevelLineData {
		public UILevelPointData[] PointList;

		public UILevelLineData(JArray json) {
			PointList = new UILevelPointData[json.Count];
			for (int i = 0; i < json.Count; i++) {
				PointList[i] = new UILevelPointData(json[i].AsArray());
			}
		}
	}

}

