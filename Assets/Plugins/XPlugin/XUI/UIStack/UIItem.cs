using UnityEngine;

namespace XUI {
	public class UIItem {
		public GameObject Prefab;
		public GameObject Instance;
		public Canvas InstanceCanvas;
//		public IUIStackBehaviour[] UiStackBehaviours;
//		public IUICoverBehaviour[] UiCoverBehaviours;
//		public IUIOverlayBehaviour[] UiOverlayBehaviours;

		public UIItem(GameObject prefab, GameObject instance, Canvas instanceCanvas) {
			this.Prefab = prefab;
			this.Instance = instance;
			this.InstanceCanvas = instanceCanvas;
//			UiStackBehaviours = this.Instance.GetComponents<IUIStackBehaviour>();
//			UiCoverBehaviours = this.Instance.GetComponents<IUICoverBehaviour>();
//			UiOverlayBehaviours = this.Instance.GetComponents<IUIOverlayBehaviour>();
		}

		public void SendDeOverlay() {
//			foreach (var behaviour in this.UiOverlayBehaviours) {
//				behaviour.OnUIDeOverlay();
//			}
//			if (this.UiOverlayBehaviours.Length == 0) {
				this.Instance.SendMessage("OnUIDeOverlay", SendMessageOptions.DontRequireReceiver);
//			}
		}

		public void SendBeenOverlay() {
//			foreach (var behaviour in this.UiOverlayBehaviours) {
//				behaviour.OnUIBeenOverlay();
//			}
//			if (this.UiOverlayBehaviours.Length == 0) {
				this.Instance.SendMessage("OnUIBeenOverlay", SendMessageOptions.DontRequireReceiver);
//			}
		}

		public void SendLeaveStack() {
//			foreach (var behaviour in this.UiStackBehaviours) {
//				behaviour.OnUILeaveStack();
//			}
//			if (this.UiStackBehaviours.Length == 0) {
				this.Instance.SendMessage("OnUILeaveStack", SendMessageOptions.DontRequireReceiver);
//			}
		}

		public void SendEnterStack() {
//			foreach (var behaviour in this.UiStackBehaviours) {
//				behaviour.OnUIEnterStack();
//			}
//			if (this.UiStackBehaviours.Length == 0) {
				this.Instance.SendMessage("OnUIEnterStack", SendMessageOptions.DontRequireReceiver);
//			}
		}

		public void SendDeCover() {
//			foreach (var behaviour in this.UiCoverBehaviours) {
//				behaviour.OnUIDeCover();
//			}
//			if (this.UiCoverBehaviours.Length == 0) {
				this.Instance.SendMessage("OnUIDeCover", SendMessageOptions.DontRequireReceiver);
//			}
		}

		public void SendBeenCover() {
//			foreach (var behaviour in this.UiCoverBehaviours) {
//				behaviour.OnUIBeenCover();
//			}
//			if (this.UiCoverBehaviours.Length == 0) {
				this.Instance.SendMessage("OnUIBeenCover", SendMessageOptions.DontRequireReceiver);
//			}
		}
	}
}