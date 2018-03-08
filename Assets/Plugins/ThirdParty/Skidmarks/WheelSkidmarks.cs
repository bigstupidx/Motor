using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WheelSkidmarks : MonoBehaviour {

	public Rigidbody myRigidbody { get; set; }
	public SkidmarkMaker SkidmarkMaker { get; set; }
	public WheelCollider wheel_col { get; set; }
	public Transform groundHit { get; set; }

	private float startSlipValue = 0.4f;
	private int lastSkidmark = -1;
	private bool _forceSkidmarks;

	public bool ForceSkidmarks {
		get { return this._forceSkidmarks; }
		set {
			this._forceSkidmarks = value;
			this.lastSkidmark = -1;
		}
	}

	void OnEnable() {
		this.lastSkidmark = -1;
	}
	//This has to be in fixed update or it wont get time to make skidmesh fully.
	void LateUpdate() {
		if (this.groundHit == null) {
			return;
		}

		if (this.SkidmarkMaker == null) {
			enabled = false;
			return;
		}

		WheelHit GroundHit; //variable to store hit data
		if (wheel_col.GetGroundHit(out GroundHit)) {
			var wheelSlipAmount = Mathf.Abs(GroundHit.sidewaysSlip);
			if (ForceSkidmarks) {
				Vector3 skidPoint = this.groundHit.position;// + 0f * (this.myRigidbody.velocity) * Time.deltaTime;
				lastSkidmark = this.SkidmarkMaker.AddSkidMark(skidPoint, GroundHit.normal, wheelSlipAmount / 1.0f + 0.5f, lastSkidmark);
			} else {
				if (wheelSlipAmount > startSlipValue) {//if sideways slip is more than desired value
					Vector3 skidPoint = this.groundHit.position;// + 0f * (this.myRigidbody.velocity) * Time.deltaTime;
					lastSkidmark = this.SkidmarkMaker.AddSkidMark(skidPoint, GroundHit.normal, wheelSlipAmount / 1.0f + 0.5f, lastSkidmark);
				} else {
					lastSkidmark = -1;
				}
			}
		} else {
			this.lastSkidmark = -1;
		}



	}

}
