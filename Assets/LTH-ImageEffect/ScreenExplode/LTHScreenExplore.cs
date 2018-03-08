using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class LTHScreenExplore : LTHImageEffectBase
{
		public int screenWidth = 30;
		public int screenHeight = 25;
		public float amplitude = 0;
		[Range(0,5)]
		public float range = 0.1f;
		public float rangeMulti=1f;
		public Vector2 center = new Vector2 (0.5f, 0.5f);
		[Range(0,0.3f)]
		public float power = 10;
		private AnimationCurve curve;
		private float time = 1f;
		private float timer = 0;
		[System.NonSerialized]
		public static LTHScreenExplore instance;

		public static void Explore(AnimationCurve curve,Vector2 center,float time,float rangeMulti=1){
				instance.Exp (curve,center,time,rangeMulti);
		}

		public void Exp(AnimationCurve curve,Vector2 center,float time,float rangeMulti=1){
				this.rangeMulti = rangeMulti;
				this.time = time;
				this.timer = 0f;
				this.enabled = true;
				this.center = center;
				this.curve = curve;

		}

		void Awake ()
		{
				instance = this;	
		}




		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{		
				if(Application.isPlaying){
						timer += Time.deltaTime;
						range =curve.Evaluate (timer / time);

						if (timer >= time) {
								timer = 0f;
								enabled = false;
						}
				}
				range *= rangeMulti;

				RenderTexture.active = destination;

				source.filterMode = FilterMode.Point;

				material.SetTexture ("_MainTex", source);

				material.SetFloat ("power", power);
				material.SetVector ("center", new Vector4 (center.x, center.y, amplitude, range));


				float m_width = 1.0f / (float)screenWidth;
				float m_height = 1.0f / (float)screenHeight;

				GL.PushMatrix ();
				GL.LoadOrtho ();

				material.SetPass (0);

				GL.Begin (GL.QUADS);

				for (int i = 0; i < screenWidth; i++) {
						for (int j = 0; j < screenHeight; j++) {  
								{
										GL.MultiTexCoord2 (0, m_width * (i + 1), m_height * j);
										GL.Vertex3 (m_width * (i + 1), m_height * j, -1.0f); // BR

										GL.MultiTexCoord2 (0, m_width * i, m_height * j);
										GL.Vertex3 (m_width * i, m_height * j, -1.0f); // BL


										GL.MultiTexCoord2 (0, m_width * i, m_height * (j + 1));
										GL.Vertex3 (m_width * i, m_height * (j + 1), -1.0f); // TL


										GL.MultiTexCoord2 (0, m_width * (i + 1), m_height * (j + 1));
										GL.Vertex3 (m_width * (i + 1), m_height * (j + 1), -1.0f); // TR 

								}

						}
				}

				GL.End ();
				GL.PopMatrix ();
		}
	
}