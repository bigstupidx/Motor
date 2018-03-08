using LTHUtility;
using UnityEngine;

namespace Game {
	[System.Serializable]
	public class WheelComponent {
		public Transform WheelPos;
		public Transform WheelRenderer;
		public BoxCollider WheelCrashCollider { get; private set; }
		public Transform AxleRenderer;
		public Transform GroundHitPos;

		[System.NonSerialized]
		public WheelCollider collider;
		[System.NonSerialized]
		public Vector3 startPos;
		[System.NonSerialized]
		public float maxSteer;
		[System.NonSerialized]
		public bool drive;
		[System.NonSerialized]
		public WheelSkidmarks skidmark;

		public void SetUp(Rigidbody rigidbody, bool drive, float maxSteer, BikeControl.BikeSetting bikeSetting) {
			this.drive = drive;
			this.maxSteer = maxSteer;
			this.startPos = this.AxleRenderer.localPosition;

			this.WheelCrashCollider = this.WheelRenderer.gameObject.AddComponent<BoxCollider>();
			var size = this.WheelCrashCollider.size;
			size.y = 0.2f;
			this.WheelCrashCollider.size = size;
			GameObject wheelCol = new GameObject(this.WheelRenderer.name + "WheelCollider");
			wheelCol.transform.SetParent(this.WheelPos, false);
			wheelCol.SetLayer(Layers.Ins.Wheel);
			var wheelCollider = wheelCol.AddComponent<WheelCollider>();
			this.collider = wheelCollider;

			this.collider.suspensionDistance = bikeSetting.SuspensionDistance;
			JointSpring js = this.collider.suspensionSpring;
			js.spring = bikeSetting.Springs;
			js.damper = bikeSetting.Dampers;
			js.targetPosition = bikeSetting.TargetPosition;
			this.collider.suspensionSpring = js;
			this.collider.radius = bikeSetting.WheelRadius;
			this.collider.mass = bikeSetting.WheelWeight;

			WheelFrictionCurve fc = this.collider.forwardFriction;
			fc.extremumSlip = 0.4f;
			fc.asymptoteSlip = 0.8f;
			fc.asymptoteValue = 0.5f;
			fc.stiffness = bikeSetting.StiffMin;
			this.collider.forwardFriction = fc;
			fc = this.collider.sidewaysFriction;
			fc.extremumSlip = 0.2f;
			fc.asymptoteValue = 0.75f;
			fc.asymptoteSlip = 0.5f;
			fc.stiffness = bikeSetting.StiffMin;
			this.collider.sidewaysFriction = fc;


			this.skidmark = wheelCol.AddComponent<WheelSkidmarks>();
			this.skidmark.myRigidbody = rigidbody;
			this.skidmark.SkidmarkMaker = SkidmarkMaker.Ins;
			this.skidmark.wheel_col = wheelCollider;
			this.skidmark.groundHit = this.GroundHitPos;
			this.skidmark.enabled = false;
		}
	}
}