using UnityEngine;

[ExecuteInEditMode]
public class GlassBroken : LTHImageEffectBase {

	[Range(0, 1)]
	public float Power;
	public Texture2D GlassTex;

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		material.SetTexture("_GlassTex", GlassTex);
		material.SetFloat("_Power", Power);
		Graphics.Blit(src, dst, material);
	}
}
