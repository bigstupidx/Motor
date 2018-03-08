using System;
using GameClient;
using LTHUtility;
using UnityEngine;

namespace Game {
	public class BikeDriverModel : MonoBehaviour {

		public int DriverPosIndex = 0;

		public Transform WeaponPoint_L;
		public Transform WeaponPoint_R;
		public Transform HeadPoint;

		[NonSerialized]
		public NormalPrefab ShapeshiftPrefab;

		[System.NonSerialized]
		public WeaponBase WeaponL;

		[System.NonSerialized]
		public WeaponBase WeaponR;
		[System.NonSerialized]
		public PlayerInfo Info;

		public void Init(PlayerInfo info) {
			this.Info = info;
		}

		public void SpawnWeapon(GameObject prefab) {
			this.WeaponL = Instantiate(prefab).GetComponent<WeaponBase>();
			this.WeaponL.transform.SetParent(this.WeaponPoint_L, false);
			this.WeaponL.transform.localPosition = Vector3.zero;
			this.WeaponL.transform.localEulerAngles = Vector3.zero;
			this.WeaponL.Hide();

			this.WeaponR = Instantiate(prefab).GetComponent<WeaponBase>();
			this.WeaponR.transform.SetParent(this.WeaponPoint_R, false);
			this.WeaponR.transform.localPosition = Vector3.zero;
			this.WeaponR.transform.localEulerAngles = Vector3.zero;
			this.WeaponR.Hide();
		}

		public void AttackReceiver() {
			if (Info == null || Info.Bike == null) {
				return;
			}
			Info.Bike.bikeAttack.DoAttack();
		}

#if UNITY_EDITOR
		/// <summary>
		/// 自动创建游戏用人物预制，Aniamtor和变身需手动设置
		/// </summary>
		[ContextMenu("Create Driver Model Hum")]
		public void CreateDriverHum() {
			DriverPosIndex = 1;
			// 左右手
			var handL = transform.FindInAllChildFuzzy("Bip001 L Hand");
			var wl = handL.FindOrCreate("WeaponPoint_L");
			wl.localRotation = Quaternion.Euler(0, -180, 0);
			WeaponPoint_L = wl;
			wl.gameObject.GetOrAddComponent<CubeGizmos>().Size = Vector3.one * 0.2f;
			var handR = transform.FindInAllChildFuzzy("Bip001 R Hand");
			var wr = handR.FindOrCreate("WeaponPoint_R");
			WeaponPoint_R = wr;
			wr.gameObject.GetOrAddComponent<CubeGizmos>().Size = Vector3.one * 0.2f;
			// 头部
			var head = transform.FindInAllChildFuzzy("Bip001 HeadNub");
			HeadPoint = head;
			// 刚体
			var rigidBody = gameObject.GetOrAddComponent<Rigidbody>();
			rigidBody.angularDrag = 0.05f;
			rigidBody.isKinematic = true;
			rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			// 碰撞
			/*var collider = gameObject.GetOrAddComponent<CapsuleCollider>();
			collider.center = new Vector3(0, 0.98f, 0);
			collider.radius = 0.3f;
			collider.height = 1.8f;*/
			// 布娃娃
			var doll = gameObject.GetOrAddComponent<Ragdoll>();
			doll.AutoAssign();
			doll.MainRigidbody = rigidBody;
		}
#endif
	}
}