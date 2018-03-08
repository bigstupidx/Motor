using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Joystick.UGUI {


	public class CancelKeyHandlerUGUI : CancelKeyHandlerBase {
		public override void OnCancel() {
			base.OnCancel();
			if (StoryGuideBoard.Ins == null) {
				SendMessage("OnPointerClick", new PointerEventData(EventSystem.current), SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
