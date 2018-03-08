using UnityEngine;
using GameClient;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour {
	public RectTransform barRect;
	private RectTransform parentRect;
	private float percent = 0f;

	void Start() {
		if (!SDKManager.Instance.IsSupport("login")) {
			SceneManager.LoadScene(DataDef.Menu_Scene);
		}
	}

	void Update() {
		percent += Time.deltaTime * 0.3f;
		SetLoadingBarValue(percent);
	}

	void SetLoadingBarValue(float percent) {
		if (parentRect == null) {
			parentRect = (RectTransform)barRect.parent;
		}
		if (percent >= 0.99f) {
			percent = 1.0f;
		}
		if (percent <= 0.0f) {
			percent = 0.0f;
		}
		barRect.sizeDelta = new Vector2((parentRect.sizeDelta.x - 10) * percent, barRect.sizeDelta.y);
	}
}
