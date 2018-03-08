using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI
{
	public class BtnSignRule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
		
		public void OnPointerDown(PointerEventData eventData) {
			SignBoard.Ins.ShowRule(true);
		}

		public void OnPointerUp(PointerEventData eventData) {
			SignBoard.Ins.ShowRule(false);
		}
	}

}

