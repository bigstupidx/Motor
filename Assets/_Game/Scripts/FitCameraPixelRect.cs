using UnityEngine;
using System.Collections;


namespace Game {

	public class FitCameraPixelRect : MonoBehaviour {
		private Vector2 designRes = new Vector2(16f, 9f);
		private int screenW, screenH;
		private Rect nowRect;
		private Camera cam;

		void Awake() {
			if (cam == null) {
				cam = GetComponent<Camera>();
			}
			if (cam == null) {
				enabled = false;
			}
			OnResize();
		}

		void Update()
		{
			OnResize();
			if (cam.pixelRect != nowRect) {
				cam.pixelRect = nowRect;
			}
		}

		void OnResize()
		{
			if (cam == null) {
				return;
			}

			int w = Screen.width;
			int h = Screen.height;

			if (w < h) {
				int t = w;
				w = h;
				w = t;
			}

			if (screenW == w && screenH == h) {
				return;
			}

			screenW = w;
			screenH = h;
			
			float designWHRate = designRes.x / designRes.y;
			nowRect = new Rect(0f, 0f, screenW, screenH);
			float nowWHRate = nowRect.width / nowRect.height;

			if (nowWHRate < designWHRate) {
				float widthRate = nowRect.width / designRes.x;
				float targetHeight = widthRate * designRes.y;
				float heightDiv = nowRect.height - targetHeight;
				nowRect.y = heightDiv / 2f;
				nowRect.height = targetHeight;
			} else {
				float heightRate = nowRect.height / designRes.y;
				float targetWidth = heightRate * designRes.x;
				float widthDiv = nowRect.width - targetWidth;
				nowRect.x = widthDiv / 2f;
				nowRect.width = targetWidth;
			}
			cam.pixelRect = nowRect;
		}

		public static bool InputOutScreen(Camera camera)
		{
			return !camera.pixelRect.Contains(Input.mousePosition);
		}
	}
}
