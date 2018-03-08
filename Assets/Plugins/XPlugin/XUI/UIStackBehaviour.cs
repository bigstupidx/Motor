using UnityEngine;

namespace XUI {

	public class SingleUIStackBehaviour<T> : UIStackBehaviour where T: SingleUIStackBehaviour<T> {
		public static T Ins { get; private set; }

		public override void OnUISpawned() {
			base.OnUISpawned();
			Ins = (T) this;
		}

		public override void OnUIDespawn() {
			base.OnUIDespawn();
			Ins = null;
		}
	}

	public class UIStackBehaviour :MonoBehaviour,IUIPoolBehaviour,IUIStackBehaviour,IUICoverBehaviour,IUIOverlayBehaviour{
		public virtual void OnUISpawned() {
		}

		public virtual void OnUIDespawn() {
		}

		public virtual void OnUILeaveStack() {
		}

		public virtual void OnUIEnterStack() {
		}

		public virtual void OnUIDeCover() {
		}

		public virtual void OnUIBeenCover() {
		}

		public virtual void OnUIDeOverlay() {
		}

		public virtual void OnUIBeenOverlay() {
		}
	}
}