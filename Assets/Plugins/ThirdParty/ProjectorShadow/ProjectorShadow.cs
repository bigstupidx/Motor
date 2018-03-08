namespace ProjectorShadow {

	using UnityEngine;

	public class ProjectorShadow : MonoBehaviour {

		public static ProjectorShadow Ins { private set; get; }

		public RenderTextureFormat Format;
		public Vector2 Size = new Vector2(512, 512);

		public Material Material;

		public Transform Target;
		public Vector3 Offset;


		private Camera shadowRendererCamera;
		private Projector _projector;

		void Awake() {
			Ins = this;
			this._projector = GetComponent<Projector>();
			RenderTexture texture = new RenderTexture((int)this.Size.x, (int)this.Size.y, 0, this.Format);

			this.shadowRendererCamera = GetComponent<Camera>();
			//			this.shadowRendererCamera.enabled = false;
			this.shadowRendererCamera.targetTexture = texture;

			this.Material.mainTexture = texture;
			this._projector.material = this.Material;
		}

		void OnDestroy() {
			Ins = null;
		}

		[ContextMenu("FollowTarget")]
		void FollowTarget() {
			if (this.Target != null) {
				this.transform.position = this.Target.position + this.Offset;
			}
		}

		public void SetEnable(bool enable) {
			this.shadowRendererCamera.enabled = enable;
			this._projector.enabled = enable;
		}

		// Use this for initialization
//		void Start() {
//			this._projector = GetComponent<Projector>();
//			RenderTexture texture = new RenderTexture((int)this.Size.x, (int)this.Size.y, 0, this.Format);
//
//			this.shadowRendererCamera = GetComponent<Camera>();
////			this.shadowRendererCamera.enabled = false;
//			this.shadowRendererCamera.targetTexture = texture;
//
//			this.Material.mainTexture = texture;
//			this._projector.material = this.Material;
//		}

		// Update is called once per frame
		void Update() {
			FollowTarget();

			var matVP = GL.GetGPUProjectionMatrix(this.shadowRendererCamera.projectionMatrix, true) * this.shadowRendererCamera.worldToCameraMatrix;
			this.Material.SetMatrix("ShadowMatrix", matVP);
			//this.shadowRendererCamera.Render();
		}

	}

}
