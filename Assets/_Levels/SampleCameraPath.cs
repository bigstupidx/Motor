using UnityEngine;
using System.Collections;

public class SampleCameraPath : MonoBehaviour {
	[Range(0,1)]
	public float t;

	public WaypointManager WaypointManager;

	public Vector3 Offset=new Vector3(0,2,0);

	void OnValidate() {
		if (this.WaypointManager == null) {
			return;
		}

		float totalDis = 0;
		foreach (var node in this.WaypointManager.WaypointList) {
			totalDis += node.NextDistance;
		}
		float needDis = t*totalDis;
		float tmpDis=0;
		foreach (var node in this.WaypointManager.WaypointList) {
			tmpDis += node.NextDistance;
			if (needDis <= tmpDis) {
				float remainDis = tmpDis - needDis;
				this.transform.position = node.transform.position;
				this.transform.position += node.transform.forward*-remainDis;
				this.transform.position += this.Offset;
				this.transform.rotation = node.transform.rotation;
				break;
			}
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
