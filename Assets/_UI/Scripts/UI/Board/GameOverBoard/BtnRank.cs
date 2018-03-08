using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI {
	public class BtnRank : MonoBehaviour, IPointerClickHandler {
		public void OnPointerClick(PointerEventData eventData) {
			RankDialog.Show();
		}
	}


}
