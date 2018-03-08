using GameUI;
using UnityEngine;
using UnityEngine.UI;

public class WaittingTip : MonoBehaviour {

	public const string PREFAB_PATH = "UI/Tip/WaittingTip";

	public Text Tip;

	public static WaittingTip Ins { get; private set; }

	public static void Show() {
		Show(LString.WaittingTip_Text.ToLocalized());
	}

	public static void Show(string txt) {
		WaittingTip ins = ModTip.Ins.Cover(new string[] { PREFAB_PATH })[0].Instance.GetComponent<WaittingTip>();
		ins.Tip.text = txt;
	}

	public static void Update(string txt) {
		if (Ins != null) {
			Ins.Tip.text = txt;
		}
	}

	public static void Hide() {
		if (Ins != null) {
			ModTip.Ins.Back();
		}
	}

	void OnEnable() {
		Ins = this;
	}

	void OnDisable() {
		Ins = null;
	}


}
