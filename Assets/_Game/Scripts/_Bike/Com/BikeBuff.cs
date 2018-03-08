using UnityEngine;
using System.Collections.Generic;

namespace Game {
	public enum BikeMoutPointType {
		Bike,
		Driver
	}

	public class BikeBuff : BikeBase {
		[Tooltip("缩放因子：生成在角色身上的buff特效会根据这个值进行缩放")]
		public float scaleFactor = 1f;

		public BuffInvincible buffInvincible;
		public BuffVertigo buffVertigo;
		public BuffFloat buffFloat;
		public BuffBlink buffBlink;

		public List<BuffBase> Buffs;
		//		public Dictionary<BikeMoutPointType, Transform> MoutPoints; 

		public override void Awake() {
			base.Awake();
			InitBuff();
		}

		private void InitBuff() {
			Buffs = new List<BuffBase>();

			buffInvincible = new BuffInvincible() { bike = this };
			Buffs.Add(buffInvincible);
			buffVertigo = new BuffVertigo() { bike = this };
			Buffs.Add(buffVertigo);
			buffFloat = new BuffFloat() { bike = this };
			Buffs.Add(buffFloat);
			buffBlink = new BuffBlink() { bike = this };
			Buffs.Add(buffBlink);
		}

		private void InitMountPoint() {
			//			MoutPoints = new Dictionary<BikeMoutPointType, Transform>();
			//			MoutPoints.Add(BikeMoutPointType.Bike, bikeControl.transform);
			//			MoutPoints.Add(BikeMoutPointType.Driver, bikeDriver.DriverPos[bikeDriver.BikeDriverModel.DriverPosIndex]);
		}

		void Start() {
			InitMountPoint();
		}

		void Update() {
			if (!bikeHealth.IsAlive) {
				return;
			}
			for (int i = 0; i < this.Buffs.Count; i++) {
				var buff = this.Buffs[i];
				buff.Update();
			}
		}

		void FixedUpdate() {
			if (!bikeHealth.IsAlive) {
				return;
			}
			for (int i = 0; i < this.Buffs.Count; i++) {
				var buff = this.Buffs[i];
				buff.FixedUpdate();
			}
		}

		public void StopAll() {
			foreach (var buff in Buffs) {
				buff.Stop();
			}
		}
	}
}

