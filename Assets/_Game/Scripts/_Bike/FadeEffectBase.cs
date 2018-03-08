using UnityEngine;

namespace Game {
	public abstract class FadeEffectBase :MonoBehaviour {

		public abstract void FadeIn(float time);
		public abstract void FadeOut(float time);
	}
}