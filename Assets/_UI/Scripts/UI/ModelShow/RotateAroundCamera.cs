using UnityEngine;

namespace GameUI {
	public class RotateAroundCamera : Singleton<RotateAroundCamera> {
		public Transform Target;
		public Vector3 Offset;

		public float SpeedX = 10.0f;
		public float SpeedLimitX = 300.0f;
		public float SpeedY = 10.0f;
		public float SpeedLimitY = 150.0f;
		public float Damping = 0.95f;
		public float limitAngleMin = 5.0f;
		public float limitAngleMax = 60.0f;
		private float _vx = 0.0f;
		private float _vy = 0.0f;


		private Vector3 touchPos = Vector3.zero;

		void Update() {
			if (this.Target == null) {
				return;
			}

			if (Input.GetMouseButton(0)) {
				if (touchPos == Vector3.zero) {
					touchPos = Input.mousePosition;
				} else {
					Vector3 nowTouch = Input.mousePosition;
					this._vx = (nowTouch.x - touchPos.x) * SpeedX;
					this._vy = (nowTouch.y - touchPos.y) * -SpeedY;
					touchPos = nowTouch;

					if (Mathf.Abs(this._vx) > SpeedLimitX) {
						this._vx = Mathf.Sign(this._vx) * SpeedLimitX;
					}
					if (Mathf.Abs(this._vy) > SpeedLimitY) {
						this._vy = Mathf.Sign(this._vy) * SpeedLimitY;
					}
				}
			} else {
				touchPos = Vector3.zero;
				_vx *= Damping;
				_vy *= Damping;
			}

			var targetPos = this.Target.position + this.Offset;

			Vector3 dir = this.transform.position - targetPos;
			dir.Normalize();
			if (this._vy < 0 && dir.y < Mathf.Sin(limitAngleMin * Mathf.PI / 180)) {
				this._vy = 0;
			} else if (this._vy > 0 && dir.y > Mathf.Sin(limitAngleMax * Mathf.PI / 180)) {
				this._vy = 0;
			}

			this.transform.RotateAround(targetPos, this.Target.transform.up, this._vx * Time.deltaTime);
			this.transform.RotateAround(targetPos, this.transform.right, this._vy * Time.deltaTime);

			Quaternion lookRotation = Quaternion.LookRotation(targetPos - this.transform.position);
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, 5 * Time.deltaTime);
		}
	}
}