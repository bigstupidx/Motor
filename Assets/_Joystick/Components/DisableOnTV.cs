using UnityEngine;

namespace Joystick {
	public class DisableOnTV : MonoBehaviour {

		void OnEnable() {
			if (FocusManager.TVMode) {
				this.gameObject.SetActive(false);
			}
		}

	}
}