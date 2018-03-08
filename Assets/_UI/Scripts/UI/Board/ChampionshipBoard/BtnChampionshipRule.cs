using GameUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnChampionshipRule : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData eventData) {
		ChallengeBoard.Ins.ShowRuleDialog(true);
	}
}
