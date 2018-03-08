using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaskBar : MonoBehaviour {

	public float Width = 100;
	private float _timer;
	private float _animTime;

	private RectTransform rectTransform;

	private float _targetWidth;
	private float _lerpStartWidth;


	void Awake() {
		this.rectTransform = GetComponent<RectTransform>();
	}

	public void SetValue(float value, float animTime) {
		this._targetWidth = value*this.Width;
		this._animTime = animTime;
		this._timer = 0;
		this._lerpStartWidth = this.rectTransform.sizeDelta.x;
	}

	void Update() {
		if (this._timer < this._animTime) {
			this._timer += Time.deltaTime;
			var size = this.rectTransform.sizeDelta;
			size.x = Mathf.Lerp(this._lerpStartWidth, this._targetWidth, this._timer/this._animTime);
			this.rectTransform.sizeDelta = size;
		}

	}
}
