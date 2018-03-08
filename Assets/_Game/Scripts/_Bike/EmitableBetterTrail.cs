using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Game {
	public class EmitableBetterTrail : FadeEffectBase {

		public TrailRenderer_Base TrailBase;

		void Awake() {
			if (this.TrailBase == null) {
				this.TrailBase = GetComponent<TrailRenderer_Base>();
			}
		}

		void Reset() {
			this.TrailBase = GetComponent<TrailRenderer_Base>();
		}

		public override void FadeIn(float time) {
			this.TrailBase.Emit = true;
		}

		public override void FadeOut(float time) {
			this.TrailBase.Emit = false;
		}
	}
}