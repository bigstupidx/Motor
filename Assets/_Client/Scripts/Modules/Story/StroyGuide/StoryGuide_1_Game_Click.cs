using UnityEngine;
using UnityEngine.EventSystems;

namespace GameClient {
	public class StoryGuide_1_Game_Click :MonoBehaviour,IPointerClickHandler{
		public void OnPointerClick(PointerEventData eventData) {
			StoryGuide_1_Game.Ins.GoNext = true;
		}
	}
}