using UnityEngine;
using System.Collections;

public class MFExplosionTester : MonoBehaviour {


	public MFExplosionPostFX.S_WaveParams Params=new MFExplosionPostFX.S_WaveParams(0.3f);

	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Vector2 pos = Input.mousePosition;
			pos.x /= Screen.width;
			pos.y /= Screen.height;
			MFExplosionPostFX.ins.EmitGrenadeExplosionWave(pos, Params);
		}
	}
}
