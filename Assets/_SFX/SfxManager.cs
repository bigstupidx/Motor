using System;
using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using XPlugin.Update;

public class SfxManager : Singleton<SfxManager> {

	public AudioMixer Mixer;
	public AudioMixerGroup MusicGroup;
	public AudioMixerGroup SfxGroup;

	public AudioListener Listener { get; private set; }

	public AudioSource MusicSource { get; private set; }
	public AudioSource SfxSource { get; private set; }

	protected override void Awake() {
		base.Awake();
		Refresh();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode) {
		Refresh();
	}

	void Refresh() {
		this.Listener = FindObjectOfType<AudioListener>();
		if (this.Listener != null) {
			var audiosources = this.Listener.gameObject.GetComponents<AudioSource>();
			foreach (var audiosource in audiosources) {
				Destroy(audiosource);
			}

			this.MusicSource = this.Listener.gameObject.AddComponent<AudioSource>();
			this.SfxSource = this.Listener.gameObject.AddComponent<AudioSource>();
			this.MusicSource.outputAudioMixerGroup = this.MusicGroup;
			this.SfxSource.outputAudioMixerGroup = this.SfxGroup;
		}

	}

	public void PlayVoice(string clipName)
	{
		var clip = UResources.Load<AudioClip>(clipName);
		SfxSource.Stop();
		Ins.DelayInvoke(() =>
		{
			PlayOneShot(clip);
		}, 0.1f);
	}

	public void PlayOneShot(SfxType type) {
		PlayOneShot(UResources.Load<AudioClip>(type.ToString()));
	}

	public void PlayOneShot(AudioClip clip, float volumeScale = 1f) {
		if (clip == null) throw new ArgumentNullException("clip");
		this.SfxSource.PlayOneShot(clip, volumeScale);
	}

	public void Play(AudioClip clip, bool loop) {
		this.MusicSource.clip = clip;
		this.MusicSource.loop = loop;
		this.MusicSource.Play();
	}

	public void SetMusicVolume(float volume) {
		this.Mixer.SetFloat("MusicVol", Mathf.Lerp(-80, 0, volume));
	}

	public void SetSfxVolume(float volume) {
		this.Mixer.SetFloat("SfxVol", Mathf.Lerp(-80, 0, volume));
	}

	private Coroutine _pauseCoro;
	public void SetPause(bool pause) {
		if (this._pauseCoro != null) {
			StopCoroutine(this._pauseCoro);
		}
		this._pauseCoro = StartCoroutine(PauseCoro(pause));
	}

	IEnumerator PauseCoro(bool pause) {
		float current;
		if (pause) {
			current = Client.User.UserInfo.Setting.SfxVolume;
		} else {
			current = 0;
		}
		while (true) {
			yield return null;
			if (pause) {
				current -= 0.02f;
				this.SetSfxVolume(current);
				if (current <= 0) {
					yield break;
				}
			} else {
				current += 0.02f;
				this.SetSfxVolume(current);
				if (current >= Client.User.UserInfo.Setting.SfxVolume) {
					yield break;
				}
			}
		}
	}





}
