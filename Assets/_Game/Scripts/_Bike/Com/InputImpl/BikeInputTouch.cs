using GameClient;
using UnityEngine;

namespace Game {
	public class BikeInputTouch : BikeInputKeyboard {
		public static BikeInputTouch Ins { get; private set; }
		[System.NonSerialized]
		public bool TurnLeft;
		[System.NonSerialized]
		public bool TurnRight;

		public override void Awake() {
			base.Awake();
			Ins = this;
		}

		protected virtual void OnDestroy() {
			Ins = null;
		}

		void Update() {
			bikeInput.Horizontal = 0f;
			bikeInput.Vertical = Input.GetAxis("Vertical") >= 0 ? 1f : -1f;
			KeyboardUpdate();
			if (TurnLeft) {
				bikeInput.Horizontal = -1;
			} else if (TurnRight) {
				bikeInput.Horizontal = 1;
			}
		}


	}
}