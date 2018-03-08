using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LTHImageEffectBase : MonoBehaviour {
	/// Provides a shader property that is set in the inspector
	/// and a material instantiated from the shader
	public Shader shader;

	private Material m_Material;

#if PERFORMANCE_CONTROL
	[Range(0, 100)]
	public int PerformanceScore = 20;

#endif


	protected virtual void Start() {
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}

		// Disable the image effect if the shader can't
		// run on the users graphics card
		if (!shader || !shader.isSupported) {
			enabled = false;
		}
	}


	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		}
	}


	protected virtual void OnEnable() {
#if PERFORMANCE_CONTROL
		if (DeviceLevel.Score < this.PerformanceScore) {
			enabled = false;
		}
#endif
	}

	protected virtual void OnDisable() {
		if (m_Material) {
			DestroyImmediate(m_Material);
		}
	}
}
