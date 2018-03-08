using UnityEngine;
using System.Collections;

public class ResetParticle : MonoBehaviour {

	public ParticleSystem Particle;
	public bool WithChildren = true;

	void Reset() {
		this.Particle = GetComponent<ParticleSystem>();
	}

	void Awake() {
		if (this.Particle == null) {
			this.Particle = GetComponent<ParticleSystem>();
		}
	}

	void OnEnable() {
		this.Particle.Clear(this.WithChildren);
	}

}
