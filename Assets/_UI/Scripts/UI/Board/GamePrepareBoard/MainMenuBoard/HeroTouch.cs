using UnityEngine;
using System.Collections;
using GameUI;
using UnityEngine.EventSystems;

public class HeroTouch : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
	public Transform Target;
	public Vector3 Offset = Vector3.up;


	public float SpeedX = 10.0f;
	public float SpeedLimitX = 300.0f;
	public float Damping = 0.95f;
	private float _vx = 0.0f;

	private bool _move = false;

	private Vector3 touchPos = Vector3.zero;

	public void OnPointerClick(PointerEventData eventData) {
		ModelShow.Ins.OnHeroClick();
	}

	public void OnPointerDown(PointerEventData eventData) {
		this._move = true;
		this.Target = ModelShow.Ins.Hero.transform;
	}

	public void OnPointerUp(PointerEventData eventData) {
		this._move = false;
	}


	void Update() {
		if (!this._move) {
			return;
		}
		if (this.Target == null) {
			return;
		}

		if (Input.GetMouseButton(0)) {
			if (touchPos == Vector3.zero) {
				touchPos = Input.mousePosition;
			} else {
				Vector3 nowTouch = Input.mousePosition;
				this._vx = (nowTouch.x - touchPos.x) * SpeedX;
				touchPos = nowTouch;

				if (Mathf.Abs(this._vx) > SpeedLimitX) {
					this._vx = Mathf.Sign(this._vx) * SpeedLimitX;
				}
			}
		} else {
			touchPos = Vector3.zero;
			_vx *= Damping;
		}

		this.Target.Rotate(Vector3.up, -_vx * Time.deltaTime);

	}

}
