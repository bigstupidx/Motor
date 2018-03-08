using UnityEngine;

namespace Game {
	public class FadeEffectGroup :FadeEffectBase {
		public FadeEffectBase[] Effects;


		public override void FadeIn(float time) {
			foreach (var f in this.Effects) {
				f.FadeIn(time);
			}
		}

		public override void FadeOut(float time) {
			foreach (var f in this.Effects) {
				f.FadeOut(time);
			}
		}
	}
}