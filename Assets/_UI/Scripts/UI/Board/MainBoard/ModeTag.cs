using UnityEngine;
using GameClient;
using GameUI;
using UnityEngine.UI;

public class ModeTag : MonoBehaviour {

	public ModeType Type;
	public Image Selected, UnSelected;

	public void SetSelectState(bool isSelected) {
		if (isSelected) {
			Selected.gameObject.SetActive(true);
			UnSelected.gameObject.SetActive(false);
		} else {
			Selected.gameObject.SetActive(false);
			UnSelected.gameObject.SetActive(true);
		}
	}
}
