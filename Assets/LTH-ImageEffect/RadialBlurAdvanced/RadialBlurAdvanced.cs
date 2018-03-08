using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RadialBlurAdvanced : LTHImageEffectBase {
	public float Strength = -0.05f;

	public Vector2 center = new Vector2(0.5f, 0.5f);

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		material.SetFloat("_Strength", this.Strength);
		material.SetFloat("_CenterX", this.center.x);
		material.SetFloat("_CenterY", this.center.y);
		Graphics.Blit(source, destination, material);
	}
}
