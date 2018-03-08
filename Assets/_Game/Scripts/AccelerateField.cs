//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using GameClient;
using UnityEngine;

namespace Game {
	/// <summary>
	/// 加速带
	/// </summary>
	[RequireComponent(typeof(BoxCollider))]
	public class AccelerateField : MonoBehaviour
	{
		void Reset()
		{
			GetComponent<BoxCollider>().isTrigger = true;
		}

		public void Reverse() {
			transform.RotateAround(transform.position, transform.up, 180);
		}

		void OnTriggerEnter(Collider other) {
			if (other.attachedRigidbody != null) {
				var bike = other.attachedRigidbody.GetComponent<BikeBase>();
				if (bike != null) {// && !bike.bikeShapeshift.IsShapeshifted) {
					// Debug.Log(bike.name + " Boosting Start");
					bike.bikeInput.OnBosstWithoutEnergy();
				}
			}
		}

		void OnTriggerExit(Collider other) {
			if (other.attachedRigidbody != null) {
				var bike = other.attachedRigidbody.GetComponent<BikeBase>();
				if (bike != null) {
					// Debug.Log(bike.name + " Boosting Timing");
					bike.bikeInput.OnBosstWithoutEnergyTiming();
					if (bike.gameObject.CompareTag(Tags.Ins.Player))
					{
						Client.EventMgr.SendEvent(EventEnum.Game_PassAccelerateField, 1);
					}
				}
			}
		}

	}
}

