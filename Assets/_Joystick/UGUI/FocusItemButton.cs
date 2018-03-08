using UnityEngine;
using UnityEngine.EventSystems;

namespace Joystick.UGUI {
	/// <summary>
	/// 使用EventSystem的选择系统，可以触发Button的过渡效果
	/// </summary>
	public class FocusItemButton : FocusItemUGUI {
		public override void OnFocused(FocusItemBase lastFocus) {
			base.OnFocused(lastFocus);
			EventSystem.current.SetSelectedGameObject(this.gameObject);
		}

		public override void OnLostFocuse(FocusItemBase newFocus) {
			base.OnLostFocuse(newFocus);
			if (EventSystem.current != null) {
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

	}
}