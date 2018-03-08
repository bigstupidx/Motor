using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Luminance : LTHImageEffectBase {
	public static Luminance ins;
	public float saturation = 1f;
	public float R = 0.22f;
	public float G = 0.707f;
	public float B = 0.071f;

	void Awake() {
		ins = this;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		material.SetFloat("_Saturation", saturation);
		material.SetFloat("_R", R);
		material.SetFloat("_G", G);
		material.SetFloat("_B", B);
		Graphics.Blit(source, destination, material);
	}
}
