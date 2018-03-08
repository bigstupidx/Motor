using System;
using GameClient;
using LTHUtility;
using UnityEngine;

namespace Game {
	public enum BikeDeathType {
		Crash,
		Beated,
		Eliminated
	}

	public class BikeHealth : BikeBase {
		[NonSerialized]
		[Range(0f, 100f)]
		public float Life;

		public bool IsAlive {
			get {
				return Life > 0;
			}
		}

		/// <summary>
		/// 复活时间
		/// </summary>
		[System.NonSerialized]
		public float ReviveTime = 3.5f;
		/// <summary>
		/// 近战防御概率(0-100)
		/// </summary>
		public float AttackDef {
			get { return _attackDef; }
			set { _attackDef = Mathf.Clamp(value, 0, 100); }
		}
		private float _attackDef = 0;
		/// <summary>
		/// 道具防御概率(0-100)
		/// </summary>
		public float PropDef {
			get { return _propDef; }
			set { _propDef = Mathf.Clamp(value, 0, 100); }
		}
		private float _propDef = 0;


		public int CrashCount { get; private set; }

		public Action OnDeath = delegate { };
		public Action OnRevive = delegate { };
		public Action<float> OnHurt = delegate { };
		public Action<float> OnCure = delegate { };

		void Start() {
			Life = 100;
			CrashCount = 0;
			bikeState.Fsm.OnCrashIn += type => {
				Life = 0;
				CrashCount++;
				bikeControl.Boosting = false;
				bikeBuff.StopAll();
				// 关闭碰撞
				gameObject.SetLayerRecursion(Layers.Ins.Blink);
				bikeDriver.Driver.gameObject.SetLayerRecursion(Layers.Ins.Blink);
				// 不同死亡方式的处理
				switch ((BikeDeathType)type) {
					case BikeDeathType.Crash:
					case BikeDeathType.Beated:
						this.DelayInvoke(Revive, ReviveTime);
						break;
					case BikeDeathType.Eliminated:
						this.DelayInvoke(() => {
							gameObject.SetActive(false);
							bikeDriver.Driver.gameObject.SetActive(false);
						}, 6f);
						break;
				}
			};
			bikeState.Fsm.OnCrashOut += o => {
				Life = 100;
				bikeBuff.buffInvincible.Start(5);
				this.DelayInvoke(() => {
					if (gameObject.CompareTag(Tags.Ins.Player)) {
						gameObject.SetLayerRecursion(Layers.Ins.Player);
					} else {
						gameObject.SetLayerRecursion(Layers.Ins.Enemy);
					}
				}, 5);
			};
		}

		public bool Damage(float power) {
			if (!IsAlive) {
				return false;
			}
			if (bikeBuff.buffInvincible.isAffect) {
				return false;
			}
			if (bikeBuff.buffBlink.isAffect) {
				return false;
			}

			Life -= power;
			if (Life <= 0) {
				Life = 0;
				return true;
			}
			return false;
		}


		[ContextMenu("Die")]
		public void Die() {
			bikeState.Fsm.processEvent(BikeFSM.Event.Crash, BikeDeathType.Beated);
			OnDeath();
		}

		public void Revive() {
			if (IsAlive) {
				return;
			}
			if (GameModeBase.Ins.Match.Data.GameMode == GameMode.Elimination ||
				GameModeBase.Ins.Match.Data.GameMode == GameMode.EliminationProp) {
				if (racerInfo.IsFinish) {
					return;
				}
			}
			racerInfo.DoReset();
			OnRevive();
		}
	}
}

