using UnityEngine;
using System.Collections;

namespace XPlugin {

	/// <summary>
	/// 
	///		1|--------------|2
	///		 |				|
	///		 |				|
	///		 |				|
	///		0|--------------|3
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class PotSprite : MonoBehaviour {
		[SerializeField]
		private Texture _texture;

		public Texture Texture {
			get { return this._texture; }
			set {
				if (this._texture == value) {
					this._texture = value;
					this._needUpdate = true;
				}
			}
		}
		public Color Color = Color.white;

		[SerializeField]
		private Vector2 _size = new Vector2(100, 100);

		public Vector2 Size {
			get { return _size; }
			set {
				if (this._size != value) {
					this._size = value;
					this._needUpdate = true;
				}
			}
		}

		public bool _transparent = false;

		public bool Transparent {
			get { return this._transparent; }
			set {
				if (this._transparent != value) {
					this._needUpdate = true;
					this._transparent = value;
				}
			}
		}

		private bool _needUpdate;

		private Mesh mesh;

		private Vector3[] vertices;
		private int[] triangles;
		private Color[] colors;

		private RectTransform trans;
		private Vector2[] uv;

		private Material _material;

		void Start() {
			if (this.Transparent) {
				this._material = new Material(Shader.Find("Unlit/Sprite/Transparent"));
			} else {
				this._material = new Material(Shader.Find("Unlit/Sprite/Opaque"));
			}
			MeshRenderer renderer = GetComponent<MeshRenderer>();
			if (renderer == null) {
				renderer = gameObject.AddComponent<MeshRenderer>();
			}
			renderer.sharedMaterial = this._material;

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if (meshFilter == null) {
				meshFilter = gameObject.AddComponent<MeshFilter>();
			}
			this.mesh = new Mesh();
			meshFilter.mesh = this.mesh;
			this.Size = this._size;


			this.vertices = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, };
			this.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
			this.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), };
			this.colors = new Color[] { this.Color, this.Color, this.Color, this.Color, };
			this._needUpdate = true;
		}

#if UNITY_EDITOR

		void OnEnable() {
			Start();
		}

		void Update() {
			this._needUpdate = true;
		}
#endif

		void LateUpdate() {
			if (this._needUpdate) {
				float hx = Size.x*0.01f / 2f;
				float hy = Size.y *0.01f/ 2f;
				Vector3 lb = new Vector3(-hx, -hy);
				Vector3 lt = new Vector3(-hx, hy);
				Vector3 rt = new Vector3(hx, hy);
				Vector3 rb = new Vector3(hx, -hy);
				this.vertices[0] = lb;
				this.vertices[1] = lt;
				this.vertices[2] = rt;
				this.vertices[3] = rb;

				for (int i = 0; i < this.colors.Length; i++) {
					this.colors[i] = this.Color;
				}

				this.mesh.vertices = this.vertices;
				this.mesh.triangles = this.triangles;
				this.mesh.uv = this.uv;
				this.mesh.colors = this.colors;

				if (this.Transparent) {
					this._material.shader = Shader.Find("Unlit/Sprite/Transparent");
				} else {
					this._material.shader = Shader.Find("Unlit/Sprite/Opaque");
				}

				this._material.mainTexture = this.Texture;
			}
		}


	}
}
