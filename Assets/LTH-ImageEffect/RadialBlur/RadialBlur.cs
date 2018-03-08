using UnityEngine;

[ExecuteInEditMode]
public sealed class RadialBlur : LTHImageEffectBase {
	//Variables//
	public Vector2 center = new Vector2(0.5f, 0.5f);
	[Range(-0.3f, 0.3f)]
	public float strength = 1f;

	public static RadialBlur instance;

	private AnimationCurve curve;
	private float t;
	private float time;
	private float disableTime;
	private bool anim;
	private Transform target;

	void Awake() {
		instance = this;
		enabled = false;
	}

	void OnDestroy() {
		instance = null;
	}

	public static void StartAnimation(AnimationCurve curve, float time, float disableTime, Transform target, Vector2 center) {
		if (instance == null) {
			Debug.LogError("radial blur instance is null , but you are tring to access it");
			return;
		}
		instance.StartAnim(curve, time, disableTime, target, center);
	}
	public void StartAnim(AnimationCurve curve, float time, float disableTime, Transform target, Vector2 center) {
		this.curve = curve;
		this.time = time;
		this.disableTime = disableTime;
		this.target = target;
		this.center = center;
		enabled = true;
		this.t = 0;
		this.anim = true;

	}

	// Update is called once per frame
	void Update() {
		if (Application.isPlaying) {
			t += Time.deltaTime;
			if (anim) {
				strength = curve.Evaluate(t / time);
				if (target != null) {//follow target
					center = target.ScreenPosRate();
				} else {
					center = new Vector2(0.5f, 0.5f);
				}
			}

			if (t >= disableTime) {
				enabled = false;
			} else if (t >= time) {
				anim = false;
			}
		}
	}



	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		base.material.SetFloat("_CenterX", center.x);
		base.material.SetFloat("_CenterY", center.y);
		base.material.SetFloat("_Strength", strength);
		Graphics.Blit(source, destination, material);
	}
}

