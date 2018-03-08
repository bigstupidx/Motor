using System.Collections;
using UnityEngine;

namespace Game {
	[System.Serializable]
	public class BikeAudioClip {

		public AudioClip Clip;
		public float Volume = 1;
		public bool Loop;
		public bool PlayOnAwake;
		public float FadeDelta = 1;

		public AudioSource Source { private set; get; }

		private float _targetVolume;

		public void Init(MonoBehaviour parent) {
			var o = new GameObject("audio " + this.Clip.name);
			o.transform.SetParent(parent.transform, false);
			this.Source = o.AddComponent<AudioSource>();
			if (SfxManager.Ins != null) {
				this.Source.outputAudioMixerGroup = SfxManager.Ins.SfxGroup;
			}
			this.Source.playOnAwake = this.PlayOnAwake;
			this.Source.spatialBlend = 1f;
			this.Source.clip = this.Clip;
			this.Source.loop = this.Loop;
			//			this.Source.rolloffMode=AudioRolloffMode.Linear;
			this.Source.minDistance = 5f;
			this.Source.maxDistance = 100f;
			this._targetVolume = this.Volume;
			if (this.PlayOnAwake) {
				this.Source.Play();
			}
			parent.StartCoroutine(Update());
		}

		IEnumerator Update() {
			while (true) {
				if (this.Source.isPlaying) {
					Source.volume = Mathf.MoveTowards(Source.volume, this._targetVolume, this.FadeDelta);
//					if (Source.volume <= 0.02f) {
//						Source.Stop();
//					}
				}
				yield return null;
			}
		}

		public void Play() {
			this._targetVolume = this.Volume;
			this.Source.volume = 0;
			this.Source.Play();
		}

		public void Stop() {
			this._targetVolume = 0f;
		}

	}
}