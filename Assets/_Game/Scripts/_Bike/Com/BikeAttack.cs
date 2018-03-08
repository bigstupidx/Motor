//
// BikeAttack.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using GameClient;
using RoomBasedClient;
using RoomServerModel;
using UnityEngine;

namespace Game {
	public class BikeAttack : BikeBase {
		/// <summary>
		/// 近战攻击距离
		/// </summary>
		[NonSerialized]
		public float AttackRange = 4f;
		/// <summary>
		/// 远程道具锁定距离
		/// </summary>
		[NonSerialized]
		public float AttackLockRange = 200f;
		// 被锁定状态
		public bool BeLocked { get; private set; }
		// 近战目标
		[NonSerialized]
		public BikeBase Target;

		/// <summary>
		/// 击杀敌人
		/// </summary>
		public Action<BikeBase> OnKillEnemy = delegate { };
		/// <summary>
		/// 被XX锁定
		/// </summary>
		public Action<BikeBase, PropType> OnBeLocked = delegate { };
		/// <summary>
		/// 被锁定取消
		/// </summary>
		public Action OnBeLockedCancel = delegate { };

		/// <summary>
		/// 攻击方向
		/// </summary>
		private bool _attackLeft;

		// 攻击冷却，防止开局连续击杀
		private float _attackTimer;
		private float _attackIntervial = 1f;

		void Start() {
			BeLocked = false;

			// 武器攻击
			bikeState.Fsm.OnAttackIn += o => {
			};
			this.bikeInput.OnAttack += (isLeft) => {
				if (_attackTimer > _attackIntervial) {
					_attackTimer = 0;
				} else {
					return;
				}
				if (isLeft == null) {
					if (Target != null) {
						// 判断目标左右
						var target = Target.transform.position;
						var disL = Vector3.Distance(bikeDriver.BikeDriverModel.WeaponPoint_L.position, target);
						var disR = Vector3.Distance(bikeDriver.BikeDriverModel.WeaponPoint_R.position, target);
						_attackLeft = disL < disR;
					}
				} else {
					_attackLeft = (bool)isLeft;
				}
				this.bikeState.ProcessEvent(BikeFSM.Event.Attack, _attackLeft);
			};

			OnBeLocked += (bike, type) => {
				BeLocked = true;
				// Debug.Log(gameObject.name + " 被 " + bike.gameObject.name + " 的 " + type + " 锁定了。");
			};
			OnBeLockedCancel += () => {
				BeLocked = false;
			};
			racerInfo.OnReset += bike => {
				OnBeLockedCancel();
			};
		}


		public bool AutoAttack = true;
		void Update() {
			if (BikeManager.Ins == null) {
				return;
			}

			_attackTimer += Time.deltaTime;

			if (Time.frameCount % 2 == 0) {
				return;
			}

			if (!RoomClient.OfflineMode) {
				if (!bikeNetwork.IsControledByMe) {
					return;
				}
			}
			if (!this.AutoAttack) {
				return;
			}

			// 更新近战目标
			Target = BikeManager.Ins.FindNearstEneny(this, AttackRange);
			// 近战
#if !MANUAL_ATTACK
			if (Target != null && Target.bikeHealth.IsAlive) {
				if (gameObject.CompareTag(Tags.Ins.Player)) {
					bikeInput.OnAttack(null);
				}
			}
#endif
		}

		/// <summary>
		/// OnAttack触发攻击动作后，由动画触发真正的攻击效果
		/// </summary>
		public void DoAttack() {

			if (!RoomClient.OfflineMode) {
				var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
				if (!room.MinePlayer.IsMaster) {
					return;
				}
			}

			var weaponTrans = _attackLeft ? bikeDriver.BikeDriverModel.WeaponPoint_L : bikeDriver.BikeDriverModel.WeaponPoint_R;
			Target = FindAttackTarget(weaponTrans, AttackRange);
			if (Target == null) {
				return;
			}

			if (Target.bikeHealth.IsAlive && Target.bikeControl.ActiveControl) {
				if (UnityEngine.Random.Range(0, 100) < Target.bikeHealth.AttackDef) {
					return;
				}
				var result = Target.bikeHealth.Damage(100);
				if (result) {
					bikeNetwork.Rpc(BroadcastType.All, "__RpcDoAttack",this.Target.bikeNetwork);
				}
			}
		}

		void __RpcDoAttack(BikeNetwork target) {
			target.Bike.bikeHealth.Die();
			if (gameObject.CompareTag(Tags.Ins.Player)) {
				Client.EventMgr.SendEvent(EventEnum.Game_KillEnemy, 1);
			}
			OnKillEnemy(this);
		}

		public BikeBase FindAttackTarget(Transform trans, float attackRange) {
			BikeBase ret = null;
			var minDis = Mathf.Infinity;
			for (var i = 0; i < BikeManager.Ins.Bikes.Count; i++) {
				var enemy = BikeManager.Ins.Bikes[i];
				if (enemy.bikeControl == this.bikeControl || !enemy.bikeHealth.IsAlive) {
					continue;
				}
				var dis = Vector3.Distance(trans.position, enemy.transform.position);
				if (dis < minDis) {
					minDis = dis;
					ret = enemy;
				}
			}
			if (minDis <= attackRange) {
				return ret;
			}
			return null;
		}

		void OnDrawGizmosSelected() {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, AttackRange);
		}
	}
}

