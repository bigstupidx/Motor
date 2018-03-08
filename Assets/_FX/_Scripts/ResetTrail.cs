using UnityEngine;
using System.Collections;

public class ResetTrail : MonoBehaviour {

	public TrailRenderer Trail;

	void Reset() {
		this.Trail = GetComponent<TrailRenderer>();
	}

	void Awake() {
		if (this.Trail == null) {
			this.Trail = GetComponent<TrailRenderer>();
		}
	}

	void OnEnable() {
		this.Trail.Clear();
	}

}
