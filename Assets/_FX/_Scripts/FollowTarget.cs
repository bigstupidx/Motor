using UnityEngine;

public class FollowTarget : MonoBehaviour {

	public Vector3 Offset;
	public Transform Target;

	// Update is called once per frame
	void Update() {
		if (Target != null) {
			transform.position = Target.position + Offset;
		}
	}
}
