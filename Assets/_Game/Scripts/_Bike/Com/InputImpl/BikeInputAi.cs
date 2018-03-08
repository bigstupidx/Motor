//
// BikeInputAi.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using GameClient;
using RoomBasedClient;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game {
	public class BikeInputAi : BikeInputImplBase {
		[NonSerialized]
		public WaypointManager WaypointManager;
		public WaypointNodeAi NowPoint {
			get {
				var index = racerInfo.ActuallyPoint.Index;
				return (WaypointNodeAi)WaypointManager.WaypointList[index];
			}
		}
		[NonSerialized]
		public Vector3 Target;

		/// <summary>
		/// AI等级
		/// 修改AI等级同时会更新临时AI等级
		/// 修改临时AI等级不影响原AI等级
		/// </summary>
		public int AiLevel {
			get { return _aiLevel; }
			set {
				_aiLevel = SetAiLevel(value);
			}
		}
		private int _aiLevel = 5;
		/// <summary>
		/// 临时AI等级, 当AI与玩家距离过大时，会临时临时提高AI等级以提高追上玩家的概率
		/// </summary>
		private int _tempAiLevel = 1;

		[NonSerialized] public int Turn = 100;
		[NonSerialized] public int Drift = 100;
		[NonSerialized] public int Boost = 100;
		[NonSerialized] public int Attack = 100;
		[NonSerialized] public int UsePropRate = 100;
		[NonSerialized] public float SpeedUp = 1f;
		[NonSerialized] public float SpeedDown = -1f;

		private bool _needBoost;
		private BikeControl _followBike;
		private float _wait = 10;


		private float OriginLimitSpeedNormal;
		private float OriginLimitSpeedBoost;
		void Start() {
			this.OriginLimitSpeedBoost = this.bikeControl.bikeSetting.LimitNormalSpeed;
			this.OriginLimitSpeedBoost = this.bikeControl.bikeSetting.LimitBoostSpeed;
		}

		private void FixedUpdate() {
			if (!racerInfo.IsFinish) {
				if (!RoomClient.OfflineMode) {
					if (!Lobby.Ins.RoomClient.ToRoomClient.MimePlayer.IsMaster) {
						return;
					}
				}
			}

			if (!racerInfo.Running) {
				return;
			}
			bikeInput.Vertical = 1;
			if (!bikeControl.Grounded) {
				bikeInput.Horizontal = 0;
			} else {
				Control();
				AdjustSpeed();

				// 开局一定时间内无动作
				if (racerInfo.RunTime < _wait) return;
				// 落后于玩家的，不互相攻击
				if (racerInfo.Rank < BikeManager.Ins.CurrentBike.racerInfo.Rank - 1) return;
				Fight();
				Action();
			}
		}


		public int SetAiLevel(int level, bool onlyControl = false) {
			_tempAiLevel = level;
			var turn = 35;
			var drift = 20;
			var boost = 20;
			var attack = 0;
			var useProp = 0;
			var speedUp = 1.1f;
			var speedDown = -1f;
			switch (level) {
				case 1:
					turn = 35;
					drift = 20;
					boost = 20;
					attack = 0;
					useProp = 0;
					speedUp = 1.0f;
					speedDown = -1f;
					break;
				case 2:
					turn = 45;
					drift = 30;
					boost = 40;
					attack = 5;
					useProp = 5;
					speedUp = 1.2f;
					speedDown = -0.9f;
					break;
				case 3:
					turn = 50;
					drift = 25;
					boost = 25;
					attack = 10;
					useProp = 10;
					speedUp = 1.3f;
					speedDown = -0.8f;
					break;
				case 4:
					turn = 50;
					drift = 50;
					boost = 50;
					attack = 20;
					useProp = 20;
					speedUp = 1.4f;
					speedDown = -0.7f;
					break;
				case 5:
					turn = 100;
					drift = 100;
					boost = 100;
					attack = 30;
					useProp = 30;
					speedUp = 1.5f;
					speedDown = -0.6f;
					break;
			}
			Turn = turn;
			Drift = drift;
			Boost = boost;
			SpeedUp = speedUp;
			this.SpeedDown = speedDown;
			if (!onlyControl) {
				Attack = attack;
				this.UsePropRate = useProp;
			}
			return _tempAiLevel;
		}

		#region 操控
		private void Control() {
			// 取下一个目标点
			// var dis = Vector3.Distance(transform.position, matchInfo.NowPoint.NextPoint.Center);
			var dis = WaypointMath.DisPoint2Line(transform.position, NowPoint.NextPoint.Left,
				NowPoint.NextPoint.Right);
			if (dis < NowPoint.NextDistance * 0.1f) {
				Target = NowPoint.NextPoint.NextPoint.Position;
			} else {
				Target = NowPoint.NextPoint.Position;
			}
			var randTarget = (AiLevel - 5) * 1.5f;
			Target += new Vector3(Random.Range(-randTarget, randTarget), 0, Random.Range(-randTarget, randTarget));
			// 距离与角度
			dis = Vector3.Distance(transform.position, Target);
			var turnAngle = Vector3.Angle(Target - transform.position, transform.forward);
			// 所处位置前后共4个路径点的角度和
			// matchInfo.NowPoint.PrePoint.Angle + matchInfo.NowPoint.Angle + matchInfo.NowPoint.NextPoint.Angle + matchInfo.NowPoint.NextPoint.NextPoint.Angle;
			// 向目标移动
			var targetLocalPos = transform.InverseTransformPoint(Target);
			var x = targetLocalPos.x * 0.5f;
			var turn = x * turnAngle / 200;
			// 转向
			if (Random.Range(0, 100) < Turn && turnAngle > 9) {
				bikeInput.Horizontal = turn;
			}
			// 刹车
			if (Mathf.Abs(turn) > 2.5f || turnAngle > 20) {
				var driftRate = Random.Range(0, 100);
				if (driftRate < Drift)
					bikeInput.OnDrift();
			}
			// 加速
			if (bikeControl.BoostingEnergy > 20 && Mathf.Abs(turn) < 2f && turnAngle < 16 || dis > WaypointManager.WayWidth) {
				var boostRate = 0;
				if (_needBoost) {
					boostRate = 0;
				} else {
					boostRate = Random.Range(0, 100);
				}
				if (boostRate < Boost) {
					bikeInput.OnBoost();
				}
			}
		}
		#endregion

		#region 近战
		private BikeBase _preTarget;

		private bool Fight() {
			var target = bikeAttack.Target;
			if (target != null && target != _preTarget && target.bikeHealth.IsAlive) {
				_preTarget = target;
				if (Random.Range(0, 100) < Attack) {
					bikeInput.OnAttack(null);
					return true;
				}
			}
			if (target == null) {
				_preTarget = null;
			}
			return false;
		}
		#endregion

		#region 动作 使用道具

		private float _actionTimer;
		private float _actionIntervialTime = 5f;

		private float _startActionTime = 10f;

		private bool Action() {
			if (Time.timeSinceLevelLoad < this._startActionTime) {//开局不做动作
				return false;
			}
			_actionTimer += Time.fixedDeltaTime;
			if (_actionTimer > _actionIntervialTime) {
				_actionTimer = 0;
			} else {
				return false;
			}

			if (Random.Range(0, 100) >= this.UsePropRate) {
				return false;
			}

			PropBase prop = null;
			// 从背包和道具栏获取可用道具
			var isEquiped = false;
			if (bikeProp.PropSlots != null && bikeProp.PropSlots.Count > 0) {
				prop = bikeProp.PropSlots[0];
			} else if (bikeProp.info.EquipedProps.Count > 0) {
				var propInfo = bikeProp.info.EquipedProps[Random.Range(0, bikeProp.info.EquipedProps.Count)];
				if (propInfo.Amount > 0) {
					prop = BikeProp.PropDic[propInfo.PropData.PropType];
					isEquiped = true;
				}
			}
			if (prop == null) {
				return false;
			}
			var use = false;
			switch (prop.Type) {
				case PropType.Energy:
					// 剩余氮气不足时使用
					if (bikeControl.BoostingEnergy < 100f * 0.5f) {
						use = true;
					}
					break;
				case PropType.Missile:
					if (!racerInfo.IsFirst) {
						use = true;
					}
					break;
				case PropType.Shield:
					// 被敌人锁定时使用
					if (bikeAttack.BeLocked) {
						use = true;
					}
					break;
			}
			if (use) {
				if (!isEquiped) {
					bikeProp.Use();
				} else {
					bikeProp.Use(prop.Type);
					bikeProp.info.EquipedProps[0].ChangeAmountWithoutSave(-1);
				}

				// Debug.Log("AI Use prop : " + name + " - " + prop.Type);
			}
			return use;
		}
		#endregion

		#region 调速

		private void AdjustSpeed() {
			if (this._followBike == null) {
				this._followBike = BikeManager.Ins.CurrentBike.bikeControl;
				if (this._followBike == null) {
					return;
				}
			}
			if (bikeControl == this._followBike) {
				return;
			}
			_needBoost = false;
			var disDelta = racerInfo.Distance - this._followBike.racerInfo.Distance;
			var waitDis = AiLevel * 2;
			if (disDelta > waitDis) {//在玩家前面
				SetAiLevel(AiLevel, true);
				SetSpeedUp(this.SpeedDown * 0.5f);
				//				Debug.Log(("减速,当前限速：" + this.bikeControl.bikeSetting.LimitNormalSpeed).Colored(LogColors.red));
			} else {//在玩家后面
				SetAiLevel(5, true);
				bikeControl.BoostingEnergy = 100f;// AI回氮气追赶
				_needBoost = true;
				SetSpeedUp(this.SpeedUp * 0.5f);
				//				Debug.Log(("加速,当前限速：" + this.bikeControl.bikeSetting.LimitNormalSpeed).Colored(LogColors.green));
			}
		}

		private void SetSpeedUp(float increse) {
			var limitMin = this.OriginLimitSpeedBoost - (150 * (1 / AiLevel));
			var limitMax = this.OriginLimitSpeedBoost + AiLevel * 10 - 70;
			bikeControl.bikeSetting.LimitNormalSpeed = Mathf.Clamp(bikeControl.bikeSetting.LimitNormalSpeed + increse, limitMin, limitMax);
			bikeControl.bikeSetting.LimitBoostSpeed = Mathf.Clamp(bikeControl.bikeSetting.LimitBoostSpeed + increse, limitMin, limitMax);
			this.bikeControl.Rigidbody.velocity *= increse > 0 ? 1.0000001f : 0.999999f;
		}

		#endregion

		#region 调试

		private void DebugLog(string str) {
			if (gameObject.layer != LayerMask.NameToLayer("15_Player")) {
				return;
			}
			Debug.Log(str);
		}

		private void OnDrawGizmos() {
			if (!enabled) {
				return;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, Target);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position + transform.forward * 10, transform.position);
		}

		#endregion
	}
}