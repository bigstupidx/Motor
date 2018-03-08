using UnityEngine;

[ExecuteInEditMode]
public class DirtLens : LTHImageEffectBase {
	public float intensity = 1.0f;
	public float intensity2 = 1.0f;
	public Texture2D texture = null;
	public Color Color;
	public Transform Light;


	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (this.Light != null) {

			this.intensity2 = -Vector3.Dot(transform.forward, this.Light.forward);
			this.intensity2 = Mathf.Clamp01(this.intensity2);
			if (this.intensity2 <= 0.1f) {
				Graphics.Blit(source, destination);
				return;
			}
		}
		material.SetFloat("_Intensity", this.intensity);
		material.SetFloat("_Intensity2", this.intensity2);

		material.SetTexture("_Lens", this.texture);
		material.SetColor("_Color", this.Color);
		Graphics.Blit(source, destination, material);


	}
}
