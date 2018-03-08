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
using System.Collections.Generic;

public class MFExplosionPostFX : LTHImageEffectBase {
	struct S_WaveEmitter {
		public Vector2 m_Center;
		public float m_Amplitude;
		public float m_Frequency;
		public float m_Speed;
		public float m_DistAtt;
		public float m_StartTime;
		public float m_Duration;
		public int m_SlotIdx;
		public Color m_color;
	};


	[System.Serializable]
	public struct S_WaveParams {
		public float m_Amplitude;
		public float m_Freq;
		public float m_Speed;
		public float m_Duration;
		public float m_Radius;
		public float m_Delay;
		public Color m_WaveColor;

		public S_WaveParams(float mAmplitude = 0.3f, float mFreq = 20f, float mSpeed = 1.4f, float mDuration = 1.5f, float mRadius = 1.0f, float mDelay = 0f) {
			m_Amplitude = mAmplitude;
			m_Freq = mFreq;
			m_Speed = mSpeed;
			m_Duration = mDuration;
			m_Radius = mRadius;
			m_Delay = mDelay;
			m_WaveColor = Color.white;
		}
	};

	int m_ScreenGridXRes = 30;
	int m_ScreenGridYRes = 25;
	Mesh m_Mesh;
	int m_MaxWaves = 4; // limted by shader, don't change unless you know what you are doing


	List<S_WaveEmitter> m_ActiveWaves = new List<S_WaveEmitter>();
	Stack<int> m_FreeWaveEmitterSlots = new Stack<int>();

	public static MFExplosionPostFX ins;
	void Awake() {
		ins = this;
		DoInit();
		enabled = false;
	}


	protected void OnDestroy() {
		ins = null;
	}

	void LateUpdate() {
		UpdateEmitters();
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (m_Mesh == null) {
			Graphics.Blit(source, destination);
			Debug.LogError("Explosion PostFX subsystem not initialized correctly");
			return;
		}

		//		RenderTexture prevRT 		= Camera.current.targetTexture;
		RenderTexture prevActiveRT = RenderTexture.active;

		//		Camera.current.targetTexture = destination;
		RenderTexture.active = destination;

		Vector4 uvOffs = Vector4.zero;

		uvOffs.x = 0.5f / source.width;
		uvOffs.y = 0.5f / source.height;
		uvOffs.z = 1;
		uvOffs.w = (float)source.height / source.width;

		for (int i = 0; i < m_MaxWaves; i++) {
			SetWaveShaderParams(i, Vector4.zero, Vector4.zero);
		}

		material.mainTexture = source;
		material.SetVector("_UVOffsAndAspectScale", uvOffs);

		foreach (S_WaveEmitter currWave in m_ActiveWaves) {
			SetupWaveShaderParams(currWave);
			material.SetColor("_WaveColor", currWave.m_color);
		}

		if (material.SetPass(0)) {
			Graphics.DrawMeshNow(m_Mesh, Matrix4x4.identity);
		} else {
			Debug.LogError("Unable to set material pass");
		}

		RenderTexture.active = prevActiveRT;
		//		Camera.current.targetTexture	= prevRT;
	}

	void DoInit() {
		InitMeshes();
		for (int i = 0; i < m_MaxWaves; i++) {
			m_FreeWaveEmitterSlots.Push(i);
		}
	}

	void InitMeshes() {
		m_Mesh = new Mesh();
		int numVerts = m_ScreenGridXRes * m_ScreenGridYRes;
		int numTris = (m_ScreenGridXRes - 1) * (m_ScreenGridYRes - 1) * 2;
		Vector3[] verts = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts]; // we fill UVs even if it is not used by shader to make Unity happy
		int[] tris = new int[numTris * 3];
		for (int y = 0; y < m_ScreenGridYRes; y++) {
			for (int x = 0; x < m_ScreenGridXRes; x++) {
				int idx = y * m_ScreenGridXRes + x;

				verts[idx].x = (float)x / (m_ScreenGridXRes - 1);
				verts[idx].y = (float)y / (m_ScreenGridYRes - 1);
				verts[idx].z = 0;

				uv[idx].x = verts[idx].x;
				uv[idx].y = verts[idx].y;
			}
		}

		int currIdx = 0;
		for (int y = 0; y < m_ScreenGridYRes - 1; y++) {
			for (int x = 0; x < m_ScreenGridXRes - 1; x++) {
				// 0   1
				// +---+
				// |   |
				// +---+
				// 3   2

				int i0 = x + y * m_ScreenGridXRes;
				int i1 = (x + 1) + y * m_ScreenGridXRes;
				int i2 = (x + 1) + (y + 1) * m_ScreenGridXRes;
				int i3 = x + (y + 1) * m_ScreenGridXRes;

				tris[currIdx++] = i3;
				tris[currIdx++] = i1;
				tris[currIdx++] = i0;

				tris[currIdx++] = i3;
				tris[currIdx++] = i2;
				tris[currIdx++] = i1;
			}
		}
		m_Mesh.vertices = verts;
		m_Mesh.uv = uv;
		m_Mesh.triangles = tris;
	}

	void UpdateEmitters() {
		float currTime = Time.timeSinceLevelLoad;

		for (int i = m_ActiveWaves.Count - 1; i >= 0; i--) {
			if ((currTime - m_ActiveWaves[i].m_StartTime) > m_ActiveWaves[i].m_Duration) {
				m_FreeWaveEmitterSlots.Push(m_ActiveWaves[i].m_SlotIdx);
				m_ActiveWaves.RemoveAt(i);
				if (m_ActiveWaves.Count == 0) {
					enabled = false;
				}
			}
		}
	}

	void SetWaveShaderParams(int slotIdx, Vector4 paramSet0, Vector4 paramSet1) {
		switch (slotIdx) {
			case 0: {
					material.SetVector("_Wave0ParamSet0", paramSet0);
					material.SetVector("_Wave0ParamSet1", paramSet1);
				}
				break;

			case 1: {
					material.SetVector("_Wave1ParamSet0", paramSet0);
					material.SetVector("_Wave1ParamSet1", paramSet1);
				}
				break;

			case 2: {
					material.SetVector("_Wave2ParamSet0", paramSet0);
					material.SetVector("_Wave2ParamSet1", paramSet1);
				}
				break;

			case 3: {
					material.SetVector("_Wave3ParamSet0", paramSet0);
					material.SetVector("_Wave3ParamSet1", paramSet1);
				}
				break;

			default:
				break;
		}
	}

	void SetupWaveShaderParams(S_WaveEmitter emitter) {
		//
		// paramsSet0.xy	- wave center (normalized coords)
		// paramsSet0.z		- wave amplitude
		// paramsSet0.w		- wave frequency
		//
		// paramsSet1.x		- wave distance attenuation
		// paramsSet1.y		- wave speed
		// paramsSet1.z		- wave start time
		//

		//		const float MIN_ATT = 0.001f;

		Vector4 paramSet0 = Vector4.zero;
		Vector4 paramSet1 = Vector4.zero;

		paramSet0.x = emitter.m_Center.x;
		paramSet0.y = emitter.m_Center.y;
		paramSet0.z = emitter.m_Amplitude;
		paramSet0.w = emitter.m_Frequency;

		paramSet1.x = emitter.m_DistAtt;
		paramSet1.y = emitter.m_Speed;
		paramSet1.z = emitter.m_StartTime;
		paramSet1.w = 1; // (1 - MIN_ATT) / (MIN_ATT * emitter.m_Duration * emitter.m_Duration);

		SetWaveShaderParams(emitter.m_SlotIdx, paramSet0, paramSet1);
	}

	public void EmitGrenadeExplosionWave(Vector2 normScreenPos, S_WaveParams waveParams) {
		if (m_FreeWaveEmitterSlots.Count == 0) {
			Debug.LogWarning("Out of free wave-emitter slots");
			return;
		}
		enabled = true;
		int slotIdx = m_FreeWaveEmitterSlots.Pop();

		S_WaveEmitter emitter = new S_WaveEmitter();

		emitter.m_Center = normScreenPos;
		emitter.m_Amplitude = waveParams.m_Amplitude;
		emitter.m_Frequency = waveParams.m_Freq;
		emitter.m_Speed = waveParams.m_Speed;
		emitter.m_StartTime = Time.timeSinceLevelLoad + waveParams.m_Delay;
		emitter.m_Duration = waveParams.m_Duration + waveParams.m_Delay;
		emitter.m_DistAtt = 1.0f / waveParams.m_Radius;
		emitter.m_SlotIdx = slotIdx;
		emitter.m_color = waveParams.m_WaveColor;

		m_ActiveWaves.Add(emitter);
	}

}
