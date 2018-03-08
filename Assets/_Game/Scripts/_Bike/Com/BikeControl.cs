using System;
using GameClient;
using LTHUtility;
using ThinksquirrelSoftware.Utilities;
using UnityEngine;

namespace Game {
	public class BikeControl : BikeBase {

		[System.Serializable]
		public class BikeSetting {
			public PrefabWithSpawnPos BikePrefab;
			public PrefabWithSpawnPos BikePrefab_Low;
			[System.NonSerialized]
			public Transform MainBody;
			[System.NonSerialized]
			public Transform Steer;

			[System.NonSerialized]
			public Transform LeftHandIK;
			[System.NonSerialized]
			public Transform RightHandIK;
			[System.NonSerialized]
			public Transform LeftFootIK;
			[System.NonSerialized]
			public Transform RightFootIK;

			//刚体参数
			[System.NonSerialized]
			public float Mass = 3500;
			[System.NonSerialized]
			public Vector3 centreOfMass = new Vector3(0.0f, 0f, 0.0f);

			//轮胎参数
			[System.NonSerialized]
			public float WheelRadius = 0.30f; // the radius of the wheels
			[System.NonSerialized]
			public float WheelWeight = 3500.0f; // the weight of a wheel
			[System.NonSerialized]
			public float SuspensionDistance = 0.6f;
			[System.NonSerialized]
			public float WheelDistance = 0.2f;
			[System.NonSerialized]
			public float Springs = 95000.0f;
			[System.NonSerialized]
			public float Dampers = 10000.0f;
			[System.NonSerialized]
			public float TargetPosition = 0.8f;

			[System.NonSerialized]
			public float MaxSteerAngle = 35.0f;
			[System.NonSerialized]
			public float MaxTurn = 1.5f;

			//前轮抬起参数
			[System.NonSerialized]
			public float maxWheelie = 40.0f;
			[System.NonSerialized]
			public float speedWheelie = 40.0f;

			//马力参数
			public float bikePower = 200;
			public float BoostPower { get { return 2 * this.bikePower; } }
			[System.NonSerialized]
			public float BackwardPower = -5000;
			[System.NonSerialized]
			public float brakePower = 50000;

			//轮胎硬度和漂移参数
			[System.NonSerialized]
			public float StiffMin = 1f; // 低速时候的轮胎硬度

			public float StiffMax {// 告诉时候的轮胎硬度
				get { return 0.017875f * this.LimitNormalSpeed - 2.0f; }
			}
			// 漂移时候的轮胎硬度
			public float DriftStiffMax {
				get { return 0.023075f * this.LimitBoostSpeed - 2.5f; }
			}
			[System.NonSerialized]
			public float DriftRotationFactor = -0.7f;
			public float DriftReduceSpeed = 0.8f;

			//最高速参数
			[System.NonSerialized]
			public float LimitBackwardSpeed = 15.0f;
			public float LimitNormalSpeed = 170.0f;
			public float LimitBoostSpeed = 200f;

		}

#if UNITY_EDITOR
		[SerializeField]
		[HeavyDutyInspector.Button("Spawn Bike", "__SpawnBike", true)]
		private bool _button_SpawnBike;

		void __SpawnBike() {
			this.bikeSetting.BikePrefab.Spawn();
		}
#endif

		[System.NonSerialized]
		public WheelComponent[] Wheels;
		[System.NonSerialized]
		public Transform MaincColliderPos;
		public BikeSetting bikeSetting;

		public Transform BikeModel { private set; get; }
		[System.NonSerialized]
		public bool ActiveControl;

		private Quaternion _steerOriginRotation;

		public bool Grounded { get; private set; }
		public float Speed { get; set; }
		public float DisPerFrame { get { return Speed * Time.deltaTime / 3.6f; } }

		public float DriftDis { get; private set; }
		public float DriftTime { get; private set; }
		public float BoostingDis { get; private set; }
		public float BoostingTime { get; private set; }

		public float SpeedWithNeg {
			get { return Speed * (this.Backward ? -1 : 1); }
		}

		public float SpeedDivLimit {
			get { return this.Speed / this.LimitSpeed; }
		}

		public bool Crashed { get; set; }

		public float _inputSteer { get; set; } //-1 1
		private float _inputAccel = 0.0f; // -1 1

		public float _steer2 { get; set; }

		private Quaternion MainBodyRotation = Quaternion.identity;

		[System.NonSerialized]
		public bool Backward = false;

		private float _curTorque = 0f;

		private bool _drifting;

		public bool Drifting {
			get { return this._drifting; }
			private set {
				if (this._drifting == value) {
					return;
				}
				this._drifting = value;
				for (int i = 0; i < this.Wheels.Length; i++) {
					if (this.EnableWheelie && i == 0) {
						continue;
					}
					this.Wheels[i].skidmark.ForceSkidmarks = this._drifting;
				}
				//				if (this._drifting && this.Boosting) {
				//					this.Boosting = false;
				//				}
				this.OnDrift(this._drifting);
			}
		}

		private bool _boosting;

		public bool Boosting {
			get { return this._boosting; }
			set {
				this._boosting = value;
				if (this._boosting && this.Drifting) {
					this.Drifting = false;
				}
				this.OnBoost(this._boosting);
			}
		}

		public bool BosstingWithoutEnergy { get; set; }
		public float BoostingWithoutEnergyTime = 3f;
		private float _boostingWithoutEnergyTimer;
		private bool _boostingWithoutEnergyTiming = false;
		[System.NonSerialized]
		public float EnergyRecoverSpeedDrift = 6f;
		public float EnergyRecoverSpeedFloat = 0f;
		public float EnergyUseSpeed = 8f;

		private float _boostingEnergy = 30;//提供一点初始氮气

		public float BoostingEnergyMax = 100f;
		public float BoostingEnergy {
			get { return this._boostingEnergy; }
			set { this._boostingEnergy = Mathf.Clamp(value, 0, BoostingEnergyMax); }
		}

		private float _maxWheelie = 30; //前轮离地角度
		private float _wheelie = 0;
		private bool _enableWheelie;

		public bool EnableWheelie {
			get { return this._enableWheelie; }
			set { this._enableWheelie = value; }
		}

		private float _motorRotation;
		private float _driftRotation;
		private Quaternion _deltaRotation2;


		private float _currentStiff;
		[System.NonSerialized]
		public Rigidbody Rigidbody;

		[System.NonSerialized]
		public float LimitSpeed;
		private float _moveForwardTime;
		private float _onAirTime;


		/// <summary>
		/// 漂移或停止漂移
		/// </summary>
		public System.Action<bool> OnDrift = delegate { };

		/// <summary>
		/// 加速或关闭加速
		/// </summary>
		public System.Action<bool> OnBoost = delegate { };

		/// <summary>
		/// 着地
		/// </summary>
		public System.Action<float> OnGround = delegate { };

		public override void Awake() {
			base.Awake();
			this.Rigidbody = this.gameObject.GetOrAddComponent<Rigidbody>();
			this.Rigidbody.mass = this.bikeSetting.Mass;
		}

		public override void Init(PlayerInfo info) {
			base.Init(info);
			this.Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			//			if (info.IsPlayer) {
			this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			//			} else {
			//				this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			//			}

			if (info.IsPlayer) {
				if (DeviceLevel.Score < 20) {
					this.BikeModel = this.bikeSetting.BikePrefab_Low.Spawn().transform;
				} else {
					this.BikeModel = this.bikeSetting.BikePrefab.Spawn().transform;
				}
			} else {
				this.BikeModel = this.bikeSetting.BikePrefab_Low.Spawn().transform;
			}

			this.BikeModel.gameObject.SetLyaerRecursionByIndex(this.gameObject.layer);

			this.bikeSetting.MainBody = this.transform.FindInAllChild("main");

			this.Wheels = new WheelComponent[2] { new WheelComponent(), new WheelComponent() };
			this.Wheels[0].WheelPos = this.transform.FindInAllChild("f_wheel_pos");
			this.Wheels[1].WheelPos = this.transform.FindInAllChild("b_wheel_pos");
			this.MaincColliderPos = this.transform.FindInAllChild("MainCollider");

			this.Wheels[0].WheelRenderer = BikeModel.FindInAllChild("f_wheel");
			this.Wheels[0].AxleRenderer = BikeModel.FindInAllChild("f_axle");
			this.Wheels[0].GroundHitPos = BikeModel.FindInAllChild("f_hit");

			this.Wheels[1].WheelRenderer = BikeModel.FindInAllChild("b_wheel");
			this.Wheels[1].AxleRenderer = BikeModel.FindInAllChild("b_axle");
			this.Wheels[1].GroundHitPos = BikeModel.FindInAllChild("b_hit");

			this.bikeSetting.Steer = BikeModel.FindInAllChild("steer");
			this.bikeSetting.LeftHandIK = BikeModel.FindInAllChild("l_hand_ik");
			this.bikeSetting.RightHandIK = BikeModel.FindInAllChild("r_hand_ik");
			this.bikeSetting.LeftFootIK = BikeModel.FindInAllChild("l_foot_ik");
			this.bikeSetting.RightFootIK = BikeModel.FindInAllChild("r_foot_ik");


			var bikeInfo = info.ChoosedBike;
			var heroInfo = info.ChoosedHero;
			BoostingEnergy = BoostingEnergyMax * (_boostingEnergy / 100f);
			EnergyRecoverSpeedDrift *= (1 + heroInfo.Data.UpgradeDatas[HeroUpgradeType.N2oAdd].GetValue(heroInfo.Level) / 100f);

			BikeUpgradeType type = BikeUpgradeType.LimitNormalSpeed;
			int lv = bikeInfo.UpgradeItemLv[type];
			float value = bikeInfo.Data.UpgradeItemDatas[type].GetValue(lv);
			this.bikeSetting.LimitNormalSpeed = value;//* 0.2f;

			type = BikeUpgradeType.LimitBoostSpeed;
			lv = bikeInfo.UpgradeItemLv[type];
			value = bikeInfo.Data.UpgradeItemDatas[type].GetValue(lv);
			this.bikeSetting.LimitBoostSpeed = value;//* 0.4f;

			type = BikeUpgradeType.Power;
			lv = bikeInfo.UpgradeItemLv[type];
			value = bikeInfo.Data.UpgradeItemDatas[type].GetValue(lv);
			this.bikeSetting.bikePower = value * 90f;

			type = BikeUpgradeType.DriftReduce;
			lv = bikeInfo.UpgradeItemLv[type];
			value = bikeInfo.Data.UpgradeItemDatas[type].GetValue(lv);
			this.bikeSetting.DriftReduceSpeed = (700f + value) / 1000f;
		}

		private void Start() {
			this.Wheels[0].SetUp(this.Rigidbody, true, this.bikeSetting.MaxSteerAngle, this.bikeSetting);
			this.Wheels[1].SetUp(this.Rigidbody, false, 0f, this.bikeSetting);

			if (this.bikeSetting.Steer) {
				this._steerOriginRotation = this.bikeSetting.Steer.localRotation;
			}

			this.LimitSpeed = this.bikeSetting.LimitNormalSpeed;
			this.bikeInput.OnBoost += () => {
				if (!this.Boosting
					&& !this.Backward
					&& this.Speed > 5.0f
					&& this.BoostingEnergy > 1f) {
					this.Boosting = true;
					this.BosstingWithoutEnergy = false;
				}
			};
			this.bikeInput.OnBosstWithoutEnergy += () => {
				if (!this.Backward) {
					this.Boosting = true;
					this.BosstingWithoutEnergy = true;
					this._boostingWithoutEnergyTiming = false;
				}
			};
			this.bikeInput.OnBosstWithoutEnergyTiming += () => {
				if (this.Boosting
					&& this.BosstingWithoutEnergy
					&& !this.Backward) {
					this._boostingWithoutEnergyTiming = true;
				}
			};
			this.bikeInput.OnDrift += () => {
				if (this.Grounded) {// 空中不能开始漂移
					this._moveForwardTime = 0f;
					this.Drifting = true;
				}
			};

			this.bikeState.Fsm.OnCrashIn += o => {
				if (this.Boosting) {
					this.Boosting = false;
				}
			};
		}

		private void Update() {
			if (info.IsPlayer) {//为了性能
				if (this.bikeSetting.Steer) {
					var steerAngle = this.Wheels[0].collider.steerAngle;
					steerAngle = steerAngle * (this.Drifting ? 3.5f : 1.5f);
					steerAngle = Mathf.Clamp(steerAngle, -this.Wheels[0].maxSteer, this.Wheels[0].maxSteer);
					var newSteer = this._steerOriginRotation * Quaternion.Euler(0, steerAngle, 0);
					this.bikeSetting.Steer.localRotation = Quaternion.Lerp(this.bikeSetting.Steer.localRotation, newSteer,
						10 * Time.deltaTime);
				}
			}

			if (this.Grounded) {
				this._onAirTime = 0f;
				if (Drifting) {// 漂移回复能量
					this.BoostingEnergy = Mathf.MoveTowards(this.BoostingEnergy, this.BoostingEnergyMax, Time.deltaTime * EnergyRecoverSpeedDrift);
				}
			} else {
				this._onAirTime += Time.deltaTime;
				if (this._onAirTime > 0.8f) {
					this.Drifting = false;
					// 浮空回复能量
					this.BoostingEnergy = Mathf.MoveTowards(this.BoostingEnergy, this.BoostingEnergyMax, Time.deltaTime * EnergyRecoverSpeedFloat);
				}
			}

			this._steer2 = Mathf.LerpAngle(this._steer2, this._inputSteer * -this.bikeSetting.MaxSteerAngle, Time.deltaTime * 10.0f);
			this._motorRotation = Mathf.LerpAngle(this._motorRotation,
				this._steer2 * this.bikeSetting.MaxTurn * (Mathf.Clamp(this.Speed / this.bikeSetting.LimitNormalSpeed, 0.0f, 1.0f)),
				Time.deltaTime * 3f);

			if (!this.Crashed) {
				var _flipRotate = (this.transform.eulerAngles.z > 90 && this.transform.eulerAngles.z < 270) ? 180.0f : 0.0f;
				this._deltaRotation2 = Quaternion.Euler(0, 0, _flipRotate - this.transform.localEulerAngles.z);
				this.Rigidbody.MoveRotation(this.Rigidbody.rotation * this._deltaRotation2);

				//				if (this.Boosting) {
				//					Wheelie += bikeSetting.speedWheelie * Time.deltaTime / (this.Speed / 150f);
				//				} else {
				//					Wheelie = Mathf.MoveTowards(Wheelie, 0, (bikeSetting.speedWheelie * 2) * Time.deltaTime * 1.3f);
				//				}
				//				Wheelie = Mathf.Clamp(Wheelie, 0, bikeSetting.maxWheelie);
				//模拟漂移时候的车身旋转
				float targetDriftRotation = this.Drifting
					? this._steer2 * this.bikeSetting.DriftRotationFactor * (this.Speed / this.LimitSpeed)
					: 0f;
				//				float targetDriftRotation = this.Drifting ? -this._motorRotation : 0f;
				float delta = Mathf.Abs(this._driftRotation) < Mathf.Abs(targetDriftRotation) ? 40f : 50f;
				this._driftRotation = Mathf.MoveTowards(this._driftRotation, targetDriftRotation, delta * Time.deltaTime);
				//				driftRotation = Mathf.LerpAngle(this.bikeSetting.MainBody.localRotation.y, driftRotation, 2f * Time.deltaTime);
				if (EnableWheelie) {
					this._wheelie = Mathf.MoveTowards(this._wheelie, this._maxWheelie, 70 * Time.deltaTime);
				} else {
					this._wheelie = Mathf.MoveTowards(this._wheelie, 0f, 90 * Time.deltaTime);
				}
				this._wheelie = Mathf.Clamp(this._wheelie, 0f, this._maxWheelie);
				var deltaRotation1 = Quaternion.Euler(-this._wheelie, this._driftRotation,
					_flipRotate - this.transform.localEulerAngles.z + (this._motorRotation));
				this.MainBodyRotation = deltaRotation1;

			} else {
				this.MainBodyRotation = Quaternion.identity;
				this._wheelie = 0;
			}

			this.Speed = this.Rigidbody.velocity.magnitude * 3.6f;

			this._inputAccel = 0.0f;
			if (this.ActiveControl) {
				if (!this.Crashed) {
					this._inputSteer = Mathf.MoveTowards(this._inputSteer, this.bikeInput.Horizontal, 0.1f);
					this._inputAccel = this.bikeInput.Vertical;
					if (Mathf.Abs(this._inputSteer) < 0.2f) {
						if (Mathf.Abs(this._inputSteer) < 0.1f) {
							this._moveForwardTime += Time.deltaTime * 2;
						} else {
							this._moveForwardTime += Time.deltaTime;
						}
						if (this._moveForwardTime > 0.7f) {
							//如果朝正前方开行超过一定时间，则停止漂移
							this.Drifting = false;
						}
					} else {
						this._moveForwardTime = 0f;
					}
				} else {
					this._inputSteer = 0;
					this.Drifting = false;
				}
			} else {
				this._inputSteer = 0f;
				this.Drifting = false;
			}

			if (!this.Backward) {
				if (this.Boosting) {
					this.LimitSpeed = this.bikeSetting.LimitBoostSpeed;
				} else {
					this.LimitSpeed = this.bikeSetting.LimitNormalSpeed;
				}
			} else {
				this.LimitSpeed = this.bikeSetting.LimitBackwardSpeed;
			}


			float maxStiff = this.Drifting ? this.bikeSetting.DriftStiffMax : this.bikeSetting.StiffMax;
			var targetStiff = maxStiff * (this.Speed / this.LimitSpeed);
			targetStiff = Mathf.Clamp(targetStiff, this.bikeSetting.StiffMin, maxStiff);
			this._currentStiff = targetStiff; // Mathf.Lerp(this._currentStiff, targetStiff, 10f*Time.deltaTime);

			//处理加速
			if (this._inputAccel < 0) {
				if (this.Boosting) {
					this.Boosting = false;
				}
				if (this.Speed < 0.3f) {
					this.Backward = true;
				}
				this._curTorque = this.bikeSetting.BackwardPower;
			} else {
				this.Backward = false;
				if (this.Boosting && !this.BosstingWithoutEnergy) {
					// 能量消耗
					this.BoostingEnergy = Mathf.MoveTowards(this.BoostingEnergy, 0.0f, Time.deltaTime * EnergyUseSpeed);
					this._curTorque = this.bikeSetting.BoostPower;
					if (this.BoostingEnergy <= 0) {
						//能量耗尽，关闭加速
						this.Boosting = false;
					}
				} else {
					this._curTorque = this.bikeSetting.bikePower;
				}
				this.EnableWheelie = this.BosstingWithoutEnergy;
				// 加速带
				if (this.BosstingWithoutEnergy) {
					if (this._boostingWithoutEnergyTiming) {
						this._boostingWithoutEnergyTimer += Time.deltaTime;
					}
					// 结束
					if (this._boostingWithoutEnergyTimer >= this.BoostingWithoutEnergyTime || !this.Boosting) {
						// Debug.Log(this.name + " Boosting End");
						this._boostingWithoutEnergyTimer = 0;
						this._boostingWithoutEnergyTiming = false;
						this.BosstingWithoutEnergy = false;
						this.Boosting = false;
					}
				}
			}

			bool dontDealAxle = false;
			if (!info.IsPlayer) {
				if (Client.Ins != null && Client.User.UserInfo.Setting.GameQuality == GameQuality.Low) {
					dontDealAxle = true;
				}
			}
			var mass = 1000f + this.Speed / this.LimitSpeed * this.bikeSetting.WheelWeight;
			foreach (var w in this.Wheels) {
				var col = w.collider;
				w.collider.mass = mass;
				//处理悬吊
				if (dontDealAxle) {//为了性能
					continue;
				}
				Vector3 axlePos = w.AxleRenderer.localPosition;
				WheelHit hit;
				if (col.GetGroundHit(out hit)) {
					axlePos.y -= Vector3.Dot(w.AxleRenderer.position - hit.point, this.transform.TransformDirection(0, 1, 0)) -
								 (col.radius);
					axlePos.y = Mathf.Clamp(axlePos.y, w.startPos.y - this.bikeSetting.WheelDistance / 2f,
						w.startPos.y + this.bikeSetting.WheelDistance / 2f);
					//	axlePos.y = Mathf.Lerp(axlePos.y, axlePosy, 50.5f * Time.deltaTime);
					if (w.skidmark.SkidmarkMaker != null) {
						w.skidmark.enabled = true;
					}
				} else {
					axlePos.y = w.startPos.y - this.bikeSetting.WheelDistance / 2f;
					w.skidmark.enabled = false;
				}
				w.AxleRenderer.localPosition = Vector3.Lerp(w.AxleRenderer.localPosition, axlePos, 10f * Time.deltaTime);
			}

			if (info.IsPlayer && Client.Ins != null) {
				// 漂移统计
				if (this.Drifting) {
					DriftDis += DisPerFrame;
					DriftTime += Time.deltaTime;
					Client.EventMgr.GameDrifting(DisPerFrame, Time.deltaTime);
				} else {
					if (DriftDis >= 1f) {
						Client.EventMgr.GameDrift(DriftDis, DriftTime);
						DriftDis = 0;
						DriftTime = 0;
					}
				}
				// 加速统计
				if (this.Boosting && !this.BosstingWithoutEnergy) {
					BoostingDis += DisPerFrame;
					BoostingTime += Time.deltaTime;
					Client.EventMgr.GameBoosting(DisPerFrame, Time.deltaTime);
				} else {
					if (BoostingDis >= 1f) {
						Client.EventMgr.GameBoost(BoostingDis, BoostingTime);
						BoostingDis = 0;
						BoostingTime = 0;
					}
				}
			}
		} //update()

		private void FixedUpdate() {
			this.bikeSetting.MainBody.localRotation = this.MainBodyRotation;
			this.Rigidbody.drag = 0f;
			if (!this.Crashed) {
				this.Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
				this.Rigidbody.centerOfMass = this.bikeSetting.centreOfMass;
			} else {
				this.Rigidbody.constraints = RigidbodyConstraints.None;
				this.Rigidbody.centerOfMass = new Vector3(0.1f, 0.3f, 0f); //重心偏移一点让车子摔倒
			}
			foreach (WheelComponent w in this.Wheels) {
				WheelCollider col = w.collider;
				if (this.Crashed) {
					w.collider.enabled = false;
					w.WheelCrashCollider.enabled = true;
				} else {
					w.collider.enabled = true;
					w.WheelCrashCollider.enabled = false;
				}
				if (this._inputAccel < 0f) {
					if (!this.Backward) {
						col.motorTorque = 0f;
						col.brakeTorque = this.bikeSetting.brakePower;
					} else {
						col.brakeTorque = 0;
					}
				} else {
					col.brakeTorque = 0;
				}
				//根据速度修改侧向摩擦力的硬度，让车辆更容易过弯
				var fc = col.sidewaysFriction;
				fc.stiffness = this._currentStiff;
				col.sidewaysFriction = fc;
				//处理车轮旋转
				Vector3 pos;
				Quaternion rot;
				col.GetWorldPose(out pos, out rot);
				w.WheelRenderer.localRotation = Quaternion.Euler(rot.eulerAngles.x, 0.0f, 0.0f);
			}

			//设置车轮马力
			foreach (WheelComponent w in this.Wheels) {
				WheelCollider col = w.collider;
				if (w.drive) {
					if (col.isGrounded && this._inputAccel != 0) {
						if (this.Speed < this.LimitSpeed) {
							col.motorTorque = this._curTorque;
							if (this.Drifting) {
								//&& w == this.wheels[0]) {//如果是漂移，让车速降下来
								if (this.Speed > this.LimitSpeed * this.bikeSetting.DriftReduceSpeed) {
									col.motorTorque = col.motorTorque * 0f;
									col.brakeTorque = 10000;
									this.Rigidbody.drag = 1f;
								}
							}
						} else {
							this.Rigidbody.drag = 1f; //车子已超速
							col.motorTorque = 0;
							col.brakeTorque = 10000;
						}
					} else {
						col.motorTorque = 0;
						col.brakeTorque = 1000;
					}
				}
				if (col.isGrounded) {
					float factor = this.Drifting ? 10f : 3f;
					float steerAngle = Mathf.Clamp(this.Speed / w.maxSteer, 1.0f, w.maxSteer / factor);
					col.steerAngle = this._inputSteer * (w.maxSteer / steerAngle);
					//					WheelHit hit;
					//					col.GetGroundHit(out hit);
					//					this.Rigidbody.AddForce(Vector3.Normalize(-hit.normal)*1000);
				} else {
					col.steerAngle = 0f;
				}
			}

			var ground = this.Wheels[0].collider.isGrounded && this.Wheels[1].collider.isGrounded;
			if (this.Grounded != ground) {
				if (ground) {
					this.OnGround(this._onAirTime);
				}
			}
			this.Grounded = ground;
			if (!this.Grounded) {
				this.Rigidbody.AddForce(0, -30000, 0);
				//this.Rigidbody.drag = .1f;
				//为了让车子在空中不会随意转动，导致下落时车身和速度方向不一致，轮胎漂移
				if (this.Speed > 0.1f && !Crashed) {
					this.Rigidbody.centerOfMass = new Vector3(0, 0.2f, 0);
					this.Rigidbody.angularDrag = 1.0f;
					// 速度方向与正下方角度过小时不扭转
					if (this.Rigidbody.velocity.sqrMagnitude > 0.01f) {
						this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(this.Rigidbody.velocity),
							10f * Time.deltaTime);
					}
				}
				if (Crashed) {
					this.Rigidbody.angularDrag = 0.0f;
				}
			} else {
				if (this._inputAccel < 0f && !this.Backward) {
					this.Rigidbody.drag = 1f; //加大阻力让车子更快停下
				}
				if (!this.Crashed) {
					this.Rigidbody.angularDrag = 10.0f;
				} else {
					this.Rigidbody.angularDrag = 0.0f;
				}
			}
		}

		private CameraShakeArgs _cameraShakeArgs = new CameraShakeArgs();

		public Action<Collision> OnImpact = delegate { };
		public Action<Collision, bool> OnRub = delegate { };
		public Action<Collision> OnRubStay = delegate { };

		private void OnCollisionEnter(Collision collision) {
			if (collision.gameObject.layer == Layers.Ins.T4M.GetLayerIndex()) {
				racerInfo.DoDrop();
				return;
			}

			if (collision.gameObject.layer == Layers.Ins.Wall.GetLayerIndex() ||
				collision.gameObject.layer == Layers.Ins.Player.GetLayerIndex()) {
				if (this.Drifting) {
					this.Drifting = false;
				}
				if (this.Speed > this.LimitSpeed * this.bikeSetting.DriftReduceSpeed * this.bikeSetting.DriftReduceSpeed) {//贴墙的速度受操控因素影响
					this.Rigidbody.velocity *= 0.9f;
				}
				this.Rigidbody.drag = 10f;
			}
			if (collision.gameObject.layer == Layers.Ins.Wall.GetLayerIndex()) {
				if (Client.Ins != null && Client.User.UserInfo.Setting.GameQuality == GameQuality.High) {
					if (info.IsPlayer) {
						var factor = collision.relativeVelocity.magnitude;
						factor *= 0.1f;
						_cameraShakeArgs.NumberOfShakes = 1;
						_cameraShakeArgs.ShakeAmount = new Vector3(0.5f, 0.5f, 0f) * factor;//不影响z轴，因为proflare就是贴着近裁切
																							//					_cameraShakeArgs.RotationAmount = new Vector3(0f, 0f, 0.2f) * factor;//不影响xy轴，因为proflare就是贴着近裁切
						_cameraShakeArgs.Speed = 10f * factor;
						_cameraShakeArgs.Decay = 0.1f * factor;
						CameraShake.instance.DoShake(_cameraShakeArgs);
					}
				}
				OnImpact(collision);
				OnRub(collision, true);
			}
		}

		private void OnTriggerEnter(Collider collider) {
			if (collider.gameObject.layer == Layers.Ins.Cliff.GetLayerIndex()) {
				racerInfo.DoDrop();
			}
		}

		private void OnCollisionStay(Collision collision) {
			if (collision.gameObject.layer == Layers.Ins.Wall.GetLayerIndex()) {
				if (info.IsPlayer) {
					this.OnRubStay(collision);
				}
			}

			if (collision.collider.gameObject.layer == Layers.Ins.Wall.GetLayerIndex() ||
				collision.collider.gameObject.layer == Layers.Ins.Player.GetLayerIndex()) {
				if (this.Drifting) {
					this.Drifting = false;
				}
				float limitWallSpeed = this.bikeSetting.DriftReduceSpeed / 2;
				if (this.Speed > this.LimitSpeed * limitWallSpeed) {//贴墙的速度受操控因素影响
					this.Rigidbody.velocity *= 0.8f;
				}
				this.Rigidbody.drag = 10f;
			}
		}

		private void OnCollisionExit(Collision collisionInfo) {
			if (collisionInfo.gameObject.layer == Layers.Ins.Wall.GetLayerIndex()) {
				OnRub(collisionInfo, false);
			}
		}

		/// <summary>
		/// 清除车辆物理运动数据
		/// </summary>
		/// <param name="bike"></param>
		public void ClearBikeRigidbody() {
			Rigidbody.velocity = Vector3.zero;
			Rigidbody.angularVelocity = Vector3.zero;
			Rigidbody.drag = 0;
			Rigidbody.angularDrag = 0;
		}
	}
}