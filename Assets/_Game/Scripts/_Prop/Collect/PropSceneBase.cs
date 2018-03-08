//
// PropSceneBase.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System.Collections;
using UnityEngine;
using LTHUtility;
using XPlugin.Update;

namespace Game {
	public class PropSceneBase : MonoBehaviour {
		[Tooltip("可以被哪些层拾取")]
		public LayerMask GainableLayer;
		[Tooltip("重生时间(0表示不重生)")]
		public float ReviveTime = 4;
		[Tooltip("拾取到的时候特效")]
		public NormalPrefab GainedEffectPrefab;

		protected AudioSource AudioSource;


		private Collider _collider;


		public virtual bool Gain(BikeBase bike) {
			bool canGain = false;
			if (TestManager.Ins != null) {
				canGain = true;
			} else {
				canGain = bike.bikeHealth.IsAlive;
			}
			return canGain;
		}

		public virtual void OnGainedEffectSpawnd(GameObject effect, BikeBase bike) {

		}

		protected virtual void Awake() {
			this._collider = GetComponent<Collider>();
			this._collider.enabled = true;
			this.AudioSource = gameObject.AddComponent<AudioSource>();
			this.AudioSource.spatialBlend = 1;
			this.AudioSource.outputAudioMixerGroup = SfxManager.Ins.SfxGroup;
		}

		protected void PlayAudio(SfxType type) {
			this.AudioSource.PlayOneShot(UResources.Load<AudioClip>(type.ToString()));
		}

		void OnTriggerEnter(Collider other) {
			if (other.attachedRigidbody != null) {
				var layer = LayerUtility.GetLayerMaskFromIndex(other.attachedRigidbody.gameObject.layer);
				if ((layer & this.GainableLayer) != 0) {
					var bike = other.attachedRigidbody.GetComponent<BikeBase>();
					if (bike == null) {
						return;
					}
					bool gained = Gain(bike);
					if (gained) {
						this._collider.enabled = false;
						gameObject.transform.SetAllChildActive(false);
						if (this.GainedEffectPrefab.Prefab != null) {
							// TODO 对象池生成特效
							var effectIns = this.GainedEffectPrefab.Spawn(transform.parent);
							effectIns.transform.position = transform.position;
							effectIns.transform.rotation = transform.rotation;
							OnGainedEffectSpawnd(effectIns, bike);
						}
						if (this.ReviveTime > 0) {
							StartCoroutine(DelayRevive());
						}
					}
				}
			}
		}

		IEnumerator DelayRevive() {
			yield return new WaitForSeconds(this.ReviveTime);
			this._collider.enabled = true;
			gameObject.transform.SetAllChildActive(true);
		}


	}
}

