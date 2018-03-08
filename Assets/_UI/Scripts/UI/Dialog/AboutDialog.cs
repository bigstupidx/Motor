using UnityEngine;

namespace GameUI {
	public class AboutDialog : MonoBehaviour {
		public const string UIPrefabPath = "UI/Dialog/AboutDialog";
		public static void Show() {
			string[] UINames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath
			};
			ModMenu.Ins.Overlay(UINames);
		}

	}

}

