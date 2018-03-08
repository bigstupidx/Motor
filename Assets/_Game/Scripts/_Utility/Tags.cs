using HeavyDutyInspector;
using UnityEngine;
using XPlugin.Update;

namespace Game {
	[CreateAssetMenu]
	public class Tags : ScriptableObject {

		private static Tags _ins;
		public static Tags Ins {
			get {
				if (_ins == null) {
					_ins = UResources.Load<Tags>("Tags");
				}
				return _ins;
			}
		}

		[Tag]
		public string Player;
		[Tag]
		public string Enemy;
		[Tag]
		public string Bike;
	}
}