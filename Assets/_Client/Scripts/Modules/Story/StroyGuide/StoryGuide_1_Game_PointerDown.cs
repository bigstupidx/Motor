using UnityEngine;
using UnityEngine.EventSystems;

namespace GameClient {
	public class StoryGuide_1_Game_PointerDown :MonoBehaviour,IPointerDownHandler{
		public void OnPointerDown(PointerEventData eventData) {
			if (!StoryGuideBoard.Ins.CheckEnd())
			{
				return;
			}
			StoryGuide_1_Game.Ins.GoNext = true;
		}
	}
}