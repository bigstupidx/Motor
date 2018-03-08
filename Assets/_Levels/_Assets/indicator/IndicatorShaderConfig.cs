using Game;
using UnityEngine;

public class IndicatorShaderConfig : MonoBehaviour {

	private float speed = 1;
	private Vector4 arg;
	// Use this for initialization
	void Start() {
		this.arg.y = 1;
		if (RaceLineManager.Ins.Current.IsReverse) {
			this.arg.y = 1;
		} else {
			arg.y = -1;
		}
	}

	// Update is called once per frame
	void Update() {
		this.arg.x += this.speed * Time.deltaTime * this.arg.y;
		Shader.SetGlobalVector("_Indicator_Move", arg);
	}
}
