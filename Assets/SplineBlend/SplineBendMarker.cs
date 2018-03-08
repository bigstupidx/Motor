using System;
using UnityEngine;
public class SplineBendMarker : MonoBehaviour {

	public enum MarkerType {
		Smooth,
		Transform,
		Beizer,
		BeizerCorner,
		Corner
	}

	public SplineBend splineScript;

	public int num;
	public MarkerType type;

	public Vector3 position;

	public Vector3 up;

	public Vector3 prewHandle;

	public Vector3 nextHandle;

	public float dist;

	public float percent;

	public Vector3[] subPoints;

	public float[] subPointPercents;

	public float[] subPointFactors;

	public float[] subPointMustPercents;
	public bool expandWithScale;

	public Vector3 oldPos;

	public Vector3 oldScale;

	public Quaternion oldRot;
	public SplineBendMarker() {
		this.subPoints = new Vector3[10];
		this.subPointPercents = new float[10];
		this.subPointFactors = new float[10];
		this.subPointMustPercents = new float[10];
	}
	public void Init(SplineBend script, int mnum) {
		this.splineScript = script;
		this.num = mnum;
		this.gameObject.name = "Marker" + mnum;
		this.transform.SetSiblingIndex(mnum);
		this.up = this.transform.up;
		this.position = script.transform.InverseTransformPoint(this.transform.position);
		SplineBendMarker next = null;
		SplineBendMarker last = null;
		if (this.num > 0) {
			last = this.splineScript.markers[this.num - 1];
		}
		if (this.num < this.splineScript.markers.Length - 1) {
			next = this.splineScript.markers[this.num + 1];
		}
		if (last) {
			this.dist = last.dist + SplineBend.GetBeizerLength(last, this);
		} else {
			this.dist = (float)0;
		}
		if (this.splineScript.closed && this.num == this.splineScript.markers.Length - 1) {
			next = this.splineScript.markers[this.splineScript.markers.Length - 2];
			next = this.splineScript.markers[1];
		}

		if (next) {
			if (this.subPoints == null) {
				this.subPoints = new Vector3[10];
			}
			float num = 1f / (float)(this.subPoints.Length - 1);
			for (int i = 0; i < this.subPoints.Length; i++) {
				this.subPoints[i] = SplineBend.AlignPoint(this, next, num * (float)i, new Vector3((float)0, (float)0, (float)0));
			}
			float num2 = (float)0;
			this.subPointPercents[0] = (float)0;
			float num3 = 1f / (float)(this.subPoints.Length - 1);
			for (int i = 1; i < this.subPoints.Length; i++) {
				this.subPointPercents[i] = num2 + (this.subPoints[i - 1] - this.subPoints[i]).magnitude;
				num2 = this.subPointPercents[i];
				this.subPointMustPercents[i] = num3 * (float)i;
			}
			for (int i = 1; i < this.subPoints.Length; i++) {
				this.subPointPercents[i] = this.subPointPercents[i] / num2;
			}
			for (int i = 0; i < this.subPoints.Length - 1; i++) {
				this.subPointFactors[i] = num3 / (this.subPointPercents[i + 1] - this.subPointPercents[i]);
			}
		}
		Vector3 vector = new Vector3((float)0, (float)0, (float)0);
		if (next) {
			vector = script.transform.InverseTransformPoint(next.transform.position);
		}
		MarkerType markerType = this.type;
		if (markerType == MarkerType.Smooth) {
			if (!next) {
				this.prewHandle = (last.position - this.position) * 0.333f;
				this.nextHandle = -this.prewHandle * 0.99f;
			} else if (!last) {
				this.nextHandle = (vector - this.position) * 0.333f;
				this.prewHandle = -this.nextHandle * 0.99f;
			} else {
				this.nextHandle = Vector3.Slerp(-(last.position - this.position) * 0.333f, (vector - this.position) * 0.333f, 0.5f);
				this.prewHandle = Vector3.Slerp((last.position - this.position) * 0.333f, -(vector - this.position) * 0.333f, 0.5f);
			}
		} else if (markerType == MarkerType.Transform) {
			if (last) {
				float magnitude = (this.position - last.position).magnitude;
				this.prewHandle = -this.transform.forward * this.transform.localScale.z * magnitude * 0.4f;
			}
			if (next) {
				float magnitude2 = (this.position - vector).magnitude;
				this.nextHandle = this.transform.forward * this.transform.localScale.z * magnitude2 * 0.4f;
			}
		} else if (markerType == MarkerType.Corner) {
			if (last) {
				this.prewHandle = (last.position - this.position) * 0.333f;
			} else {
				this.prewHandle = new Vector3((float)0, (float)0, (float)0);
			}
			if (next) {
				this.nextHandle = (vector - this.position) * 0.333f;
			} else {
				this.nextHandle = new Vector3((float)0, (float)0, (float)0);
			}
		}
		if ((this.nextHandle - this.prewHandle).sqrMagnitude < 0.01f) {
			this.nextHandle += new Vector3(0.001f, (float)0, (float)0);
		}
	}
}
