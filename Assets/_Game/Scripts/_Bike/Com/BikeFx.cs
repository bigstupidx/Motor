using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
	[System.Serializable]
	public class AiDisabledFxPrefab : PrefabWithSpawnPos {
		public bool DisableOnAi;
		public bool DisableOnLowDevice;
	}

	public class BikeFx : BikeBase {
		public class WheelParticle {
			public FadeEffectBase Effect;
		}

		[Tooltip("刹车/漂移粒子")]
		public NormalPrefab BrakeEffectPrefab;
		private WheelParticle[] _driftParticle;

		[Tooltip("刹车灯")]
		public AiDisabledFxPrefab BrakeLightPrefab;
		private FadeEffectBase _brakeLightEffect;

		[Tooltip("氮气加速特效")]
		[Reorderable]
		public AiDisabledFxPrefab[] BoostEffectPrefab;
		private List<FadeEffectBase> _boostEffects;

		void Start() {
			bool isAi = gameObject.CompareTag(Tags.Ins.Enemy);

			//生成刹车灯
			if (!this.BrakeLightPrefab.DisableOnAi) {
				if (!this.BrakeLightPrefab.DisableOnLowDevice||DeviceLevel.Score>20) {
					this._brakeLightEffect = this.BrakeLightPrefab.Spawn().GetComponent<FadeEffectBase>();
					this._brakeLightEffect.FadeOut(0);
				}
			}

			//生成刹车/漂移特效
			this._driftParticle = new WheelParticle[] { new WheelParticle(), new WheelParticle(), };
			this._driftParticle[0].Effect = this.BrakeEffectPrefab.Spawn(bikeControl.Wheels[0].GroundHitPos).GetComponent<FadeEffectBase>();
			this._driftParticle[1].Effect = this.BrakeEffectPrefab.Spawn(bikeControl.Wheels[1].GroundHitPos).GetComponent<FadeEffectBase>();
			foreach (var p in this._driftParticle) {
				p.Effect.FadeOut(0f);
			}
			//注册漂移事件
			if (DeviceLevel.Score > 20 || info.IsPlayer) {
				this.bikeControl.OnDrift += drifting => {
					if (drifting) {
						for (int i = 0; i < this._driftParticle.Length; i++) {
							if (i == 0 && this.bikeControl.EnableWheelie) {
								continue;
							}
							var p = this._driftParticle[i];
							p.Effect.FadeIn(0f);
						}
						if (this._brakeLightEffect != null) {
							this._brakeLightEffect.FadeIn(0);
						}
					} else {
						foreach (var p in this._driftParticle) {
							p.Effect.FadeOut(0f);
						}
						if (this._brakeLightEffect != null) {
							this._brakeLightEffect.FadeOut(0);
						}
					}
				};
			}

			//生成加速特效
			this._boostEffects = new List<FadeEffectBase>();
			foreach (var prefab in this.BoostEffectPrefab) {
				if (prefab.DisableOnAi) {//有一个风的特效不需要在ai上显示，节约性能
					if (isAi) {
						continue;
					}
				}
				if (prefab.DisableOnLowDevice) {
					if (DeviceLevel.Score < 20) {
						continue;
					}
				}
				var ins = prefab.Spawn().GetComponent<FadeEffectBase>();
				this._boostEffects.Add(ins);
				ins.FadeOut(0f);
			}

			//注册加速事件
			if (DeviceLevel.Score > 20 || info.IsPlayer) {
				this.bikeControl.OnBoost += boosting => {
					if (boosting) {
						foreach (var f in this._boostEffects) {
							f.FadeIn(0f);
						}
					} else {
						foreach (var f in this._boostEffects) {
							f.FadeOut(0f);
						}
					}
				};
			}

		}

		void Update() {
			if (!bikeControl.Drifting) {
				if (this._brakeLightEffect != null) {
					if (this.bikeInput.Vertical <= 0 || this.bikeControl.Speed <= 10f) {
						this._brakeLightEffect.FadeIn(0);
					} else {
						this._brakeLightEffect.FadeOut(0);
					}
				}
			}
		}

	}
}