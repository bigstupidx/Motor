using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI {
	public class BtnBack : MonoBehaviour, IPointerClickHandler {

		public void OnPointerClick(PointerEventData eventData) {
			ModMenu.Ins.Back();
		}
	}

}


