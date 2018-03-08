using UnityEngine;

namespace GameUI {
	public class BG : MonoBehaviour { 

	void OnUISpawned() {
			if (ModelShow.Ins != null) {
				ModelShow.Ins.SetActive(false);
			}
		}

	}
}