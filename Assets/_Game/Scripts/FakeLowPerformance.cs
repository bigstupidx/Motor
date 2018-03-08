using UnityEngine;

namespace Game {
	public class FakeLowPerformance : MonoBehaviour {
		public bool OnlyEditorMode = true;

		public int Count = 10000;

		public float a { get; set; }

		void OnEnable() {
			if (this.OnlyEditorMode && !Application.isEditor) {
				enabled = false;
			}
		}

		void Update() {
			for (int i = 0; i < this.Count; i++) {
				a = i * i + i - i;
				a = i * i + i - i;
				a = i * i + i - i;
				a = i * i + i - i;
			}
		}

	}
}