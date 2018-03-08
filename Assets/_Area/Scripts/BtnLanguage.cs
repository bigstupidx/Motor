using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XPlugin.Localization;

public class BtnLanguage : MonoBehaviour, IPointerClickHandler {
	public LanguageEnum Language;
	public Text Txt;

	void Awake() {
		if (Localization.Language == LanguageEnum.fr_FR) {
			switch (Language) {
				case LanguageEnum.fr_FR:
					Txt.text = "français";
					break;
				case LanguageEnum.en_US:
					Txt.text = "Anglais";
					break;
			}
		} else {
			switch (Language) {
				case LanguageEnum.fr_FR:
					Txt.text = "French";
					break;
				case LanguageEnum.en_US:
					Txt.text = "English";
					break;
			}
		}
	}


	public void OnPointerClick(PointerEventData eventData) {
		LanguageChooseManager.Ins.SetLanguage(Language);
	}
}
