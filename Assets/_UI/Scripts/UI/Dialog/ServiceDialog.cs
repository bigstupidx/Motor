using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class ServiceDialog : MonoBehaviour
	{
		public const string UIPrefabPath = "UI/Dialog/ServiceDialog";

		public static string[] UINames =
		{
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath
		};

		public static void Show(string s)
		{
			ServiceDialog ins = ModMenu.Ins.Overlay(UINames)[1].Instance.GetComponent<ServiceDialog>();
			ins.Content.text = s;
		}

		public Text Content;


	}

}

