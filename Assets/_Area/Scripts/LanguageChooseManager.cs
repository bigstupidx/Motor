using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XPlugin.Localization;

public class LanguageChooseManager : Singleton<LanguageChooseManager> {
	const string LanguageSaveKey = "currentLanguage";
	public Text Title;
	public GameObject Mask;

	void Start() {
		LanguageEnum language = (LanguageEnum)PlayerPrefs.GetInt(LanguageSaveKey, (int)LanguageEnum.Unkonwn);
		if (language != LanguageEnum.Unkonwn) {
			SetLanguage(language);
			return;
		}
		Mask.SetActive(false);
		switch (Localization.Language) {
			case LanguageEnum.fr_FR:
				Title.text = "Langue Choisissez";
				break;
			default:
				Title.text = "Language Choose";
				break;
		}
	}


	public void SetLanguage(LanguageEnum language) {
		Mask.SetActive(true);
		Localization.Language = language;
		PlayerPrefs.SetInt(LanguageSaveKey, (int)Localization.Language);
		PlayerPrefs.Save();
		SceneManager.LoadScene("MenuScene");
	}
}
