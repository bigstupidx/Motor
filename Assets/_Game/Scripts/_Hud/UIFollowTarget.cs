using GameUI;
using UnityEngine;

namespace Game {
	public class UIFollowTarget : MonoBehaviour {

		public Transform target;
		public GameObject goHide;

		public bool disableIfInvisible = false;

		public Camera gameCamera;
		public Camera uiCamera;

		void Start() {
			gameCamera = Camera.main;
			uiCamera = UICamera.Ins.Camera;
		}

		void LateUpdate() {
			if (target == null) {
				return;
			}
			if (gameCamera == null) {
				gameCamera = Camera.main;
				return;
			}
			Vector3 tempPos = gameCamera.WorldToViewportPoint(target.position);
			bool isVisible = IsTargetVisible(tempPos);
			if (this.disableIfInvisible && !isVisible) {
				if (goHide != null) {
					goHide.SetActive(false);
				}
			} else {
				if (goHide != null) {
					goHide.SetActive(true);
				}
				Move(tempPos);
			}
		}

		private void Move(Vector3 tempPos) {
			transform.position = uiCamera.ViewportToWorldPoint(tempPos);
			tempPos = transform.localPosition;
			tempPos.x = Mathf.FloorToInt(tempPos.x);
			tempPos.y = Mathf.FloorToInt(tempPos.y);
			tempPos.z = 0f;
			transform.localPosition = tempPos;
		}

		public bool IsTargetVisible(Vector3 tempPos) {
			return  tempPos.x > 0f && tempPos.x < 1f 
				&& tempPos.y > 0f && tempPos.y < 1f 
				&& tempPos.z > 0f
				&& tempPos.z < gameCamera.farClipPlane;
		}

//		public static void SetToUIPos(Transform target, Transform hud) {
//			if (Camera.main == null || target == null) {
//				return;
//			}
//			Vector3 tempPos = Camera.main.WorldToViewportPoint(target.position);
//			hud.position = GameObject.Find("UICamera").GetComponent<Camera>().ViewportToWorldPoint(tempPos);
//			tempPos = hud.localPosition;
//			tempPos.x = Mathf.FloorToInt(tempPos.x);
//			tempPos.y = Mathf.FloorToInt(tempPos.y);
//			tempPos.z = 0f;
//			hud.localPosition = tempPos;
//		}

	}
}
