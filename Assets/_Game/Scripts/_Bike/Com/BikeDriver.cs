using GameClient;
using UnityEngine;

namespace Game {

	public class BikeDriver : BikeBase {

		public Transform[] DriverPos;
		public NormalPrefab DriverPrefab;

		public Animator DriverAnimator { get; private set; }
		public Transform Driver { get; private set; }
		public BikeDriverIk DriverIk { get; private set; }
		public Ragdoll PlayeRagdoll { get; private set; }
		public BikeDriverModel BikeDriverModel { get; private set; }

		public Rigidbody DriverRigidbody { get; private set; }

		public NormalPrefab Weapon;

		private float _steer = 0.0f;

		public override void Init(PlayerInfo playerInfo) {
			base.Init(playerInfo);
			// 加载车手
			var driverName = "";
			if (playerInfo.IsPlayer) {
				driverName = playerInfo.ChoosedHero.Data.Prefab + "_hum";
			} else {
				driverName = playerInfo.ChoosedHero.Data.Prefab + "_ai";
			}
			this.DriverPrefab._prefab = null;
			this.DriverPrefab.ResourcePath = driverName;
			this.Weapon._prefab = null;
			//武器
			var weaponPrefab = playerInfo.ChoosedWeapon.WeaponData.Prefab;
			this.Weapon.ResourcePath = weaponPrefab;

			CreateDriver(playerInfo);
		}

		public void CreateDriver(PlayerInfo playerInfo) {

			if (this.DriverPrefab.Prefab == null) {
				this.DriverPrefab.ResourcePath = "human_hum";
			}
			this.Driver = this.DriverPrefab.Spawn().transform;
			this.DriverAnimator = this.Driver.GetComponent<Animator>();
			RegisterAttackFinish();

			this.DriverRigidbody = this.Driver.GetComponent<Rigidbody>();
			this.DriverRigidbody.isKinematic = true;
			this.PlayeRagdoll = this.Driver.GetComponent<Ragdoll>();
			this.PlayeRagdoll.Enable(false);

			this.BikeDriverModel = this.Driver.GetComponent<BikeDriverModel>();
			this.BikeDriverModel.Init(playerInfo);
			this.Driver.transform.SetParent(this.DriverPos[this.BikeDriverModel.DriverPosIndex], false);
			this.Driver.transform.ResetLocal();
			if (playerInfo != null && playerInfo.ChoosedWeapon.WeaponData.Type!=WeaponType.Foot) {
				this.BikeDriverModel.SpawnWeapon(this.Weapon.Prefab);
			}
			this.DriverIk = this.Driver.gameObject.AddComponent<BikeDriverIk>();
			this.DriverIk.Init(this, this.DriverAnimator);
		}

		public void RegisterAttackFinish() {
			var behaviours = this.DriverAnimator.GetBehaviours<AttackStateLeave>();
			foreach (var behaviour in behaviours) {
				behaviour.OnFinish += () => {
					this.bikeState.ProcessEvent(BikeFSM.Event.Attack_finish);
				};
			}
		}

		void Start() {
			if (this.Driver == null) {
				CreateDriver(null);
			}
			// 武器攻击
			this.bikeState.Fsm.OnAttackIn += o => {
				if (this.DriverAnimator == null) {//联网模式下奇怪的错误
					return;
				}
				bool left = (bool)o;
				if (left) {
					this.DriverIk.LeftHandIK = 0f;
					if (this.BikeDriverModel.WeaponL != null) {
						this.BikeDriverModel.WeaponL.Show();
					}
				} else {
					this.DriverIk.RightHandIK = 0f;
					if (this.BikeDriverModel.WeaponL != null) {
						this.BikeDriverModel.WeaponR.Show();
					}
				}

				if (info.ChoosedWeapon.WeaponData.Type == WeaponType.Stick) {
					this.DriverAnimator.SetBool("attack_right", !left);
					this.DriverAnimator.SetTrigger("attack");
					this.DriverAnimator.Play("attack_hand_left");
				} else if (info.ChoosedWeapon.WeaponData.Type == WeaponType.Foot) {
					this.DriverAnimator.SetBool("kick_right", !left);
					this.DriverAnimator.SetTrigger("kick");
					this.DriverAnimator.Play("attack_kick_left");
				}
			};
			this.bikeState.Fsm.OnAttackOut += o => {
				this.DriverIk.LeftHandIK = 1f;
				this.DriverIk.RightHandIK = 1f;
				if (this.BikeDriverModel.WeaponL != null) {
					this.BikeDriverModel.WeaponL.Hide();
				}
				if (this.BikeDriverModel.WeaponR != null) {
					this.BikeDriverModel.WeaponR.Hide();
				}
			};

			// 撞毁
			this.bikeState.Fsm.OnCrashIn += o => {
				this.DriverAnimator.enabled = false;
				this.Driver.SetParent(null);

				this.DriverRigidbody.isKinematic = false;
				//this.SpringManager.enabled = true;
				//				this.DriverCollider.enabled = true;
				this.DriverRigidbody.velocity = this.bikeControl.Rigidbody.velocity * 1.1f + Vector3.up * 2;
				this.DriverRigidbody.angularVelocity = this.bikeControl.Rigidbody.angularVelocity;
				this.DriverRigidbody.constraints = RigidbodyConstraints.None;
				this.DriverRigidbody.drag = 0.5f;
				this.DriverRigidbody.angularDrag = 1.5f;

				this.PlayeRagdoll.Enable(true);
				this.PlayeRagdoll.MainRigidbody.velocity = this.bikeControl.Rigidbody.velocity * 2 + Vector3.up * 5;
				this.PlayeRagdoll.MainRigidbody.velocity = Vector3.ClampMagnitude(this.PlayeRagdoll.MainRigidbody.velocity, 50);//速度太快关节会断开
				this.bikeControl.Rigidbody.velocity /= 4f;
			};

			this.bikeState.Fsm.OnCrashOut += o => {

			};
		}


		void Update() {
			this._steer = this.bikeControl._inputSteer;
			//			if (bikeControl.Speed > 50 && this._grounded) {
			//			} else {
			//				this._steer = Mathf.MoveTowards(this._steer, 0.0f, Time.deltaTime * 10.0f);
			//			}
			if (this.bikeDriver.DriverPos[this.BikeDriverModel.DriverPosIndex].gameObject.activeSelf) {
				this.DriverAnimator.SetFloat("speed", this.bikeControl.SpeedWithNeg);
				this.DriverAnimator.SetFloat("steer", this._steer);
			}
		}

	}
}