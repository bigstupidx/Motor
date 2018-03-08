using GameClient;
using UnityEngine;
using XPlugin.Update;

namespace GameUI {
	public class DriverShow : MonoBehaviour {
		public Vector3 RotOffset;
		public Vector3 HeroBoardRotOffset;

		private Animator animator;
		private AudioSource _audio;

		void Awake() {
			animator = GetComponentInChildren<Animator>();
			this._audio = gameObject.AddComponent<AudioSource>();
			this._audio.outputAudioMixerGroup = SfxManager.Ins.SfxGroup;
		}

		void OnEnable() {
			if (HeroBoard.Ins != null && HeroBoard.Ins.gameObject.activeSelf) {
				this.transform.localEulerAngles = HeroBoardRotOffset;
			} else {
				this.transform.localEulerAngles = RotOffset;
			}
		}


		public void PlayIdleAnim() {
			animator.Play("idle01");
		}

		public void PlayChooseAudio(int id) {
			string prefab = Client.Hero[id].Prefab;
			this._audio.Stop();
			this._audio.PlayOneShot(UResources.Load<AudioClip>(prefab + "/Choose01"));
		}

		public void PlayTalkAudio(int id) {
			string prefab = Client.Hero[id].Prefab;
			this._audio.Stop();
			int random = Random.Range(1, 4);
			this._audio.PlayOneShot(UResources.Load<AudioClip>(prefab + "/Talk0" + random));
		}

		public void PlayUpgradeAudio(int id) {
			string prefab = Client.Hero[id].Prefab;
			this._audio.Stop();
			this._audio.PlayOneShot(UResources.Load<AudioClip>(prefab + "/Upgrade01"));
		}

		//TODO 在游戏胜利和失败的时候调用胜利和失败
		public static void PlayLoseAudio(int id) {//胜利和失败在游戏中调用，这里用static
			string prefab = Client.Hero[id].Prefab;
			//SfxManager.Ins.SfxSource.PlayOneShot(UResources.Load<AudioClip>(prefab + "/Lose01"));
		}

		public static void PlayWinAudio(int id) {
			string prefab = Client.Hero[id].Prefab;
			//SfxManager.Ins.SfxSource.PlayOneShot(UResources.Load<AudioClip>(prefab + "/Win01"));
		}
	}
}