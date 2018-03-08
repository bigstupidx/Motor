using UnityEngine;

namespace GameUI {
	public class UICamera :Singleton<UICamera> {
		public Camera Camera { get; private set; }

		protected override void Awake() {
			base.Awake();
			this.Camera = GetComponent<Camera>();
		}
	}
}