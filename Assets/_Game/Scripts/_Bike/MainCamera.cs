using UnityEngine;

namespace Game {
	public class MainCamera :Singleton<MainCamera> {

		public BikeCamera BikeCamera;

		protected override void Awake() {
			base.Awake();
			this.BikeCamera = GetComponent<BikeCamera>();
		}

	}
}