using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game;
using GameClient;
using RoomBasedClient;
using RoomServerModel;

public class BikeNetwork : RoomView {

	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Vector3 currentVelocity;
	private float updateTime = 0;

	public float FinishTime = -1;

	public BikeBase Bike { get; private set; }

	void Awake() {
		Bike = GetComponent<BikeBase>();
	}


	void Start() {
		if (RoomClient.OfflineMode) {
			enabled = false;
			return;
		}

		if (this.IsControledByMe) {
			Bike.bikeInput.OnDrift += () => {
				this.Rpc(BroadcastType.Others, "__OnDrift");
			};
			Bike.bikeInput.OnBoost += () => {
				this.Rpc(BroadcastType.Others, "__OnBoost");
			};

			Bike.bikeInput.OnAttack += left => {
				this.Rpc(BroadcastType.Others, "__OnAttack", left);
			};

			Bike.racerInfo.OnFinish += bike => {
				this.Rpc(BroadcastType.All, "__OnFinish", bike.racerInfo.RunTime);
			};
		}
	}

	public override void OnInstantiate(object[] customData) {
		base.OnInstantiate(customData);
		bool isPlayer = (bool)customData[0];
		PlayerInfo info = (PlayerInfo)customData[1];
		BikeManager.Ins.ApplyBikeInfo(info, Bike);
		if (!isPlayer) {
			BikeManager.Ins.SetBikeAsEnemy(Bike);
		}
	}

	public bool MoveByNetwork = true;

	//	public void Update() {
	public void FixedUpdate() {
		if (this.FinishTime > 0) {
			Bike.racerInfo.RunTime = this.FinishTime;
		}
		if (!IsControledByMe) {
			if (this.MoveByNetwork) {
				//			Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);
				var dis = Vector3.Distance(transform.position, this.correctPlayerPos);
				if (dis > 30f) {
					transform.position = correctPlayerPos;
				} else if (dis > 15f) {
					transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 8f);
				} else {
					transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 4f);
				}
				//				transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 3000);
				transform.rotation = this.correctPlayerRot;
				Bike.bikeControl.Rigidbody.velocity = currentVelocity;

				if (Vector3.Distance(transform.position, this.correctPlayerPos) < 1) {
					this.MoveByNetwork = false;
				}
			}
		}
	}

	public void __OnAttack(bool? isLeft) {
		Bike.bikeInput.OnAttack(isLeft);
	}

	public void __OnDrift() {
		Bike.bikeInput.OnDrift();
	}

	public void __OnBoost() {
		Bike.bikeInput.OnBoost();
	}

	public void __OnFinish(float time) {
		this.FinishTime = time;
	}

	public override void SendUpdate(List<object> toSend) {
		base.SendUpdate(toSend);
		if (IsControledByMe) {
			toSend.Add(Bike.bikeInput.Horizontal);
			toSend.Add(Bike.bikeInput.Vertical);
			toSend.Add(transform.position);
			toSend.Add(transform.rotation);
			toSend.Add(Bike.bikeControl.Rigidbody.velocity);
			//			toSend.Add(bikeControl.Rigidbody.angularVelocity);

		}
	}

	public override void RecieveUpdate(List<object> recieve) {
		base.RecieveUpdate(recieve);
		Bike.bikeInput.Horizontal = (float)recieve[0];
		Bike.bikeInput.Vertical = (float)recieve[1];
		correctPlayerPos = (Vector3)recieve[2];
		correctPlayerRot = (Quaternion)recieve[3];
		currentVelocity = (Vector3)recieve[4];
		updateTime = Time.time;
		this.MoveByNetwork = true;
	}

}
