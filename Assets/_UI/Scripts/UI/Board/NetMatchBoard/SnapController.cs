using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI {
	public class SnapController :MonoBehaviour, IBeginDragHandler,IEndDragHandler {
		public EnhancedScroller Scroller;
		public void OnBeginDrag(PointerEventData eventData) {
			this.Scroller.snapping = false;
		}

		public void OnEndDrag(PointerEventData eventData) {
			this.Scroller.snapping = true;
		}
	}
}