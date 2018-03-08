using UnityEngine;

[ExecuteInEditMode]
public sealed class ScreenColorMulti : LTHImageEffectBase {
	public static ScreenColorMulti instance;
	public Color color = Color.white;

	void Awake() {
		instance = this;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		base.material.SetColor("_Color", color);
		Graphics.Blit(source, destination, material);
	}
}

