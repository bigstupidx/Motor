using UnityEngine;
using System.Collections;
using System;
using Game;
using Joystick;
using UnityEngine.EventSystems;

namespace GameUI {

	public class HelpOnClick : MonoBehaviour, IPointerClickHandler {

		public static bool CanPreseveHelpClick = false;
		public static bool PreserveHelpClick = false;

		public Action ActClick;

		void Update() {
			if (CanPreseveHelpClick && PreserveHelpClick) {
				return;
			}
			if (FocusManager.TVMode) {
				if (JoystickInput.Ins.IsConfirmDown) {
					if (ActClick != null) {
						ActClick();
					}
				}
			}
		}

		public void OnPointerClick(PointerEventData eventData) {
			if (ActClick != null) {
				ActClick();
			}
		}

		public void Clear() {
			ActClick = null;
		}

		public static void SendOnClick(GameObject targetGO) {
			PointerEventData eventData = new PointerEventData(null);
			targetGO.SendMessage("OnPointerClick", eventData);
		}
	}
}
