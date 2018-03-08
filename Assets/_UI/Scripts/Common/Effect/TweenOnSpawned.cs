using UnityEngine;
using XUI;

namespace GameUI {
	public class TweenOnSpawned : MonoBehaviour, IUIPoolBehaviour {
		[Reorderable]
		public UITweener[] Tweeners;

		[ContextMenu("Autofind")]
		void __AutoFind() {
			this.Tweeners = GetComponents<UITweener>();
		}

		public void OnUISpawned() {
			if (this.Tweeners == null || this.Tweeners.Length == 0) {
				this.Tweeners = GetComponents<UITweener>();
			}
			foreach (var t in this.Tweeners) {
				t.ResetToBeginning();
				t.PlayForward();
			}
		}

		public void OnUIDespawn() {
		}

	}
}