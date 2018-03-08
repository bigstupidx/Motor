using UnityEngine;

namespace Game {
	public class FadeFlare : FadeEffectBase {

		public LensFlare Flare { get; set; }

		private float originBrightness;

		private float _targetBrightness;
		public float delta = 0.05f;

		void Awake() {
			this.Flare = GetComponent<LensFlare>();
			this.originBrightness = this.Flare.brightness;
			this._targetBrightness = this.originBrightness;
		}

		void Update() {
			this.Flare.brightness = Mathf.MoveTowards(this.Flare.brightness, this._targetBrightness, this.delta);
		}


		public override void FadeIn(float time) {
			this._targetBrightness = this.originBrightness;
		}

		public override void FadeOut(float time) {
			this._targetBrightness = 0f;
		}
	}
}