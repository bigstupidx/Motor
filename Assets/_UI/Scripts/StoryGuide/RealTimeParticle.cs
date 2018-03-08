using System;
using UnityEngine;

public class RealTimeParticle : MonoBehaviour
{
	private ParticleSystem _particle;

	void Awake ()
	{
		_particle = GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if (_particle != null)
		{
			if (Math.Abs(Time.timeScale) < 1e-6)
			{
				_particle.Simulate(Time.unscaledDeltaTime, false, false);
				_particle.Play();
			}
		}
	}
}
