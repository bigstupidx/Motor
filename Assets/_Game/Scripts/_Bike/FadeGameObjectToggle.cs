using UnityEngine;

namespace Game {
	public class FadeGameObjectToggle : FadeEffectBase {
		public override void FadeIn(float time) {
			gameObject.SetActive(true);
		}

		public override void FadeOut(float time) {
			gameObject.SetActive(false);
		}
	}
}