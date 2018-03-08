//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;

namespace Game {
	/// <summary>
	/// 减速带
	/// </summary>
	[RequireComponent(typeof(BoxCollider))]
	public class DecelerateField : MonoBehaviour {
		private float _speedLimit;

		void Reset() {
			GetComponent<BoxCollider>().isTrigger = true;
		}

		public void Reverse() {
			transform.RotateAround(transform.position, transform.up, 180);
		}

		void OnTriggerEnter(Collider other) {
			if (other.attachedRigidbody != null) {
				var bike = other.attachedRigidbody.GetComponent<BikeBase>();
				if (bike != null && !bike.bikeBuff.buffInvincible.isAffect) // && !bike.bikeShapeshift.IsShapeshifted)
				{
					// Debug.Log(bike.name + " Decelerate Start");
					_speedLimit = bike.bikeControl.Speed * 0.7f;
					bike.bikeControl.OnBoost(false);
				}
			}
		}

		void OnTriggerStay(Collider other) {
			if (other.attachedRigidbody != null) {
				var bike = other.attachedRigidbody.GetComponent<BikeBase>();
				if (bike != null && !bike.bikeBuff.buffInvincible.isAffect) // && !bike.bikeShapeshift.IsShapeshifted)
				{
					if (bike.bikeControl.Speed > _speedLimit) {
						// Debug.Log(bike.name + " Decelerate Speed Down");
						bike.bikeControl.Rigidbody.velocity = Vector3.MoveTowards(bike.bikeControl.Rigidbody.velocity, Vector3.zero, Time.deltaTime * bike.bikeControl.Speed);
					}
				}
			}
		}

		void OnTriggerExit(Collider other) {
			if (other.attachedRigidbody != null) {
				var bike = other.attachedRigidbody.GetComponent<BikeBase>();
				if (bike != null) {
					// Debug.Log(bike.name + " Decelerate Leave");
				}
			}
		}
	}

}
