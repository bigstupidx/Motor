using UnityEngine;
using UnityEngine.EventSystems;

namespace Joystick.UGUI {
	/// <summary>
	/// 点击确认会对目标发送 OnPointerClick 消息
	/// </summary>
	public class FocusItemPointerClick : FocusItemUGUI {
		public override void OnFocused(FocusItemBase lastFocus) {
			base.OnFocused(lastFocus);
			if (EventSystem.current != null) {
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		public override void OnLostFocuse(FocusItemBase newFocus) {
			base.OnLostFocuse(newFocus);
			if (EventSystem.current != null) {
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		public override void OnConfirmDown() {
			base.OnConfirmDown();
			SendMessage("OnPointerClick", new PointerEventData(EventSystem.current), SendMessageOptions.DontRequireReceiver);
		}

	}
}