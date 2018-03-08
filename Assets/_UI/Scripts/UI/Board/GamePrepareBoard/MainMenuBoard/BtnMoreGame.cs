using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI {
	public class BtnMoreGame : MonoBehaviour, IPointerClickHandler {

		public void OnPointerClick(PointerEventData eventData) {
			SDKManager.Instance.MoreGame();
		}
	}

}

