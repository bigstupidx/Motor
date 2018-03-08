using UnityEngine;
using Game;
using UnityEngine.EventSystems;

public class BtnReset : MonoBehaviour, IPointerClickHandler {

	public float _time;
	private float _timer;

	public void OnPointerClick(PointerEventData eventData) {
		BikeManager.Ins.CurrentBike.racerInfo.DoReset();
	}

	void OnEnable() {
		_timer = 0;

	}

	void Update() {
		_timer += Time.deltaTime;
		if (_timer >= _time && _time > 0) {
			BikeManager.Ins.CurrentBike.racerInfo.DoReset();
		}
	}

	void OnDisable() {

	}
}
