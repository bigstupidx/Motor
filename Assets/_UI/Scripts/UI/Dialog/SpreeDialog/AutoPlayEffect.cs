
using UnityEngine;

namespace GameUI
{
	public class AutoPlayEffect : MonoBehaviour
	{
		public ParticleSystem Ps;
		public float DelayTime = 0.4f;

		private float timer = 0;

		void OnEnable()
		{
			timer = 0;
		}

		void Update()
		{
			timer += Time.unscaledDeltaTime;
			if (timer >= DelayTime)
			{
				Ps.Play();
				timer = 0;
			}
		}
	}

}
