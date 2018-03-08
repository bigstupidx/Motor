//
// By using or accessing the source codes or any other information of the Game SHADOWGUN: DeadZone ("Game"),
// you ("You" or "Licensee") agree to be bound by all the terms and conditions of SHADOWGUN: DeadZone Public
// License Agreement (the "PLA") starting the day you access the "Game" under the Terms of the "PLA".
//
// You can review the most current version of the "PLA" at any time at: http://madfingergames.com/pla/deadzone
//
// If you don't agree to all the terms and conditions of the "PLA", you shouldn't, and aren't permitted
// to use or access the source codes or any other information of the "Game" supplied by MADFINGER Games, a.s.
//

using UnityEngine;

[ExecuteInEditMode]
public class MFColorCorrectionEffectSimple : LTHImageEffectBase {
	public float R_offs = 0;
	public float G_offs = 0;
	public float B_offs = 0;


	// Called by camera to apply image effect
	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (material) {
			material.shader = shader;
			material.SetVector("_ColorBias", new Vector4(R_offs, G_offs, B_offs, 0));
		}
		Graphics.Blit(source, destination, material);
	}
}
