using UnityEngine;
using XPlugin.Update;


namespace Game {
	[CreateAssetMenu]
	public class Layers : ScriptableObject {
		private static Layers _ins;
		public static Layers Ins {
			get {
				if (_ins == null) {
					_ins = UResources.Load<Layers>("Layers");
				}
				return _ins;
			}
		}

		public LayerMask Wheel;

		public LayerMask Cliff;
		public LayerMask Wall;
		public LayerMask T4M;

		public LayerMask Road;
		public LayerMask Player;
		public LayerMask Enemy;
		public LayerMask Blink;
		public LayerMask Accelerate;

		public LayerMask Over;


	}
}
