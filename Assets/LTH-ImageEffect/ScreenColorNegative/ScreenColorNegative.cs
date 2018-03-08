using UnityEngine;

[ExecuteInEditMode]
public sealed class ScreenColorNegative : LTHImageEffectBase {
	public static ScreenColorNegative instance;
	[Range(0f, 1f)]
	public float m = 1f;

	void Awake() {
		instance = this;
		enabled = false;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		base.material.SetFloat("_M", m);
		Graphics.Blit(source, destination, material);
	}
}

