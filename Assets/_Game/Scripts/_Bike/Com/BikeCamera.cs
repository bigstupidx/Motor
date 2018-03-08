using UnityEngine;
using LTHUtility;
using System.Collections.Generic;

namespace Game {
	public enum CameraMode {
		Fixed,
		Drift,
	}

	[System.Serializable]
	public struct CameraArgs {
		public float distance;
		public float height;
		public float Angle;

		public float DriftT;

		public static CameraArgs MoveToward(CameraArgs from, CameraArgs to, float t) {
			return new CameraArgs() {
				distance = Mathf.MoveTowards(from.distance, to.distance, t),
				Angle = Mathf.MoveTowards(from.Angle, to.Angle, t),
				height = Mathf.MoveTowards(from.height, to.height, t),
				DriftT = Mathf.MoveTowards(from.DriftT, to.DriftT, t),
			};
		}
	}

	public class BikeCamera : Singleton<BikeCamera> {
		public float CrashAngle = 60;
		public float smooth;
		private float yVelocity;

		public CameraMode mode { get; set; }
		public Transform Target { get; set; }
		public BikeControl Bike { get; set; }
		public Camera cam { get; private set; }
		public GlassBroken glassBroken;
		public CameraArgs DriftArg;
		public CameraArgs FixedArg;

		private float t = T;
		private const float T = 2;


		private CameraArgs _nowCameraArgs;
		private CameraArgs _targetCameraArgs;

		Vector3 xoffsetVec = Vector3.zero;

		protected override void Awake() {
			base.Awake();
			this.cam = GetComponent<Camera>();
			if (GameClient.Client.Ins != null) {
				SetCameraMode(GameClient.Client.User.UserInfo.Setting.CamaerMode);
			} else {
				this._targetCameraArgs = this.DriftArg;
			}
		}

		public void SetCameraMode(CameraMode m) {
			this.mode = m;
			GameClient.Client.User.UserInfo.Setting.CamaerMode = m;
			switch (m) {
				case CameraMode.Drift:
					this._targetCameraArgs = this.DriftArg;
					break;
				case CameraMode.Fixed:
					this._targetCameraArgs = this.FixedArg;
					break;
			}
		}

		public void SetTarget(BikeControl target) {
			this.Target = target.transform;
			this.Bike = target;
			this.transform.position = this.Target.position;
			this.transform.rotation = this.Target.rotation;
			this.t = 1000000;
			this._nowCameraArgs = this._targetCameraArgs;
			this.t = T;
		}

		public void LateUpdate() {
			if (this.Target == null) {
				return;
			}

			if (this.Bike.Boosting) {
				cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 90f, 1f * Time.deltaTime);
			} else {
				if (this.Bike.LimitSpeed != 0) {
					cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, this.Bike.Speed / this.Bike.LimitSpeed + 60.0f, 1f * Time.deltaTime);
				}
			}
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 70, 90.0f);

			this._nowCameraArgs = CameraArgs.MoveToward(this._nowCameraArgs, this._targetCameraArgs, 4f * Time.deltaTime);

			if (!this.Bike.Crashed) {
				var targetYAngle = Mathf.Lerp(this.Target.eulerAngles.y, this.Bike.bikeSetting.MainBody.transform.eulerAngles.y,
					this._nowCameraArgs.DriftT);
				var yAngle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetYAngle, ref this.yVelocity, this.smooth);
				if (float.IsNaN(yVelocity)) {
					yVelocity = 0f;
				}
				if (float.IsNaN(yAngle)) {
					yAngle = 0f;
				}

				var zangle = Mathf.LerpAngle(this.transform.eulerAngles.z, this.Bike._steer2 * 0.7f * this.Bike.SpeedDivLimit, t * Time.deltaTime);
				if (float.IsNaN(zangle)) {
					zangle = 0f;
				}
				zangle = Mathf.LerpAngle(0f, zangle, this._nowCameraArgs.DriftT);
				this.transform.eulerAngles = new Vector3(this._nowCameraArgs.Angle, yAngle, zangle);

				var direction = this.transform.rotation * -Vector3.forward;


				Vector3 targetXOffset = Vector3.zero;
				if (Bike.Drifting) {
					var xoffset = this.Bike._steer2 * 0.03f;
					targetXOffset = new Vector3(xoffset, 0f, 0f);
					targetXOffset = this.transform.rotation * targetXOffset;
					targetXOffset = Vector3.Lerp(Vector3.zero, targetXOffset, this._nowCameraArgs.DriftT);
				}
				this.xoffsetVec = Vector3.MoveTowards(this.xoffsetVec, targetXOffset, t * Time.deltaTime);
				var targetDistance = AdjustLineOfSight(this.Target.position + new Vector3(0, this._nowCameraArgs.height, 0), direction);
				this.transform.position =
					this.Target.position + xoffsetVec + new Vector3(0, this._nowCameraArgs.height, 0) + direction * targetDistance
					 ;
			} else {
				var yAngle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y,
										this.Target.eulerAngles.y,
											ref this.yVelocity, this.smooth);
				this.transform.eulerAngles = MathUtility.MoveTowardsAngle(this.transform.eulerAngles,
					new Vector3(this.CrashAngle, yAngle, 0.0f), 100 * Time.deltaTime);

				var direction = transform.rotation * -Vector3.forward;

				var targetDistance = AdjustLineOfSight(this.Target.position + new Vector3(0, this._nowCameraArgs.height, 0), direction);
				this.transform.position = this.Target.position + new Vector3(0f, this._nowCameraArgs.height, 0) + direction * targetDistance;

				//				Vector3 look = Target.position - transform.position;
				//				transform.rotation = Quaternion.LookRotation(look);
			}
			glassBroken.enabled = this.Bike.Crashed;

		}

		float AdjustLineOfSight(Vector3 target, Vector3 direction) {
			//			RaycastHit hit;
			//			if (Physics.Raycast(target, direction, out hit, distance, lineOfSightMask.value))
			//				return hit.distance;
			//			else
			return this._nowCameraArgs.distance;
		}


	}
}
