using UnityEngine;
using System.Collections;

public class ShowOverDrawHolder : MonoBehaviour {
	public MonoBehaviour showOverDrawCom;

#if UNITY_EDITOR
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.O)) {
			showOverDrawCom.enabled = !showOverDrawCom.enabled;
		}
	}
#endif
}
