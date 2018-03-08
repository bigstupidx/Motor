using GameClient;
using GameUI;
using UnityEngine;
public class BlockClickDialog : Singleton<BlockClickDialog> {

	#region base
	public const string UIPrefabPath = "UI/Dialog/BlockClickDialog";

	public static string[] UIPrefabNames =
	{
		UIPrefabPath
	};

	public static void Show() {
		ModHelp.Ins.Overlay(UIPrefabNames);
	}

	#endregion

}
