using UnityEngine;

namespace Game {
	public class EmitableParticle : FadeEffectBase {

		public ParticleSystem Particle { get; set; }

		void Awake() {
			this.Particle = GetComponent<ParticleSystem>();
		}

		public override void FadeIn(float time) {
			var emission = Particle.emission;
			emission.enabled = true;
		}

		public override void FadeOut(float time) {
			var emission = Particle.emission;
			emission.enabled = false;
		}
	}
}