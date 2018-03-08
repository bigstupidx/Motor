using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LTHScreenExploreIn : LTHImageEffectBase {
	[System.NonSerialized]
	public static LTHScreenExploreIn instance;
	public Vector2 center = new Vector2(0.5f, 0.5f);
	[Range(-10f, 10f)]
	public float R = 0;
	[Range(0f, 10f)]
	public float P = 0;
	[Range(-0.5f, 0.5f)]
	public float Q = 0;

	void Awake() {
		instance = this;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		material.SetVector("_RPQ", new Vector4(R, P, Q));
		material.SetVector("_Center", new Vector4(center.x, center.y));
		Graphics.Blit(source, destination, material);
	}

}