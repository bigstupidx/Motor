using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI {
	public static class UIUtility {


		/// <summary>
		/// 检测是否点击在UI上,注意GC开销
		/// </summary>
		/// <returns></returns>
		public static bool IsTouchUI() {
			if (EventSystem.current == null) {
				return false;
			}

			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

			return results.Count > 0;
		}
	}
}