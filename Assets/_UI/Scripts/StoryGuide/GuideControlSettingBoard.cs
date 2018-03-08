using GameClient;
using GameUI;
using UnityEngine;
public class GuideControlSettingBoard : MonoBehaviour {


	#region base
	public const string UIPrefabPath = "UI/Board/GuideControlSettingBoard";

	public static string[] UIPrefabNames =
	{
		UICommonItem.DIALOG_BLACKBG_NOCLICK,
		UIPrefabPath
	};

	public static void Show() {
		ModHelp.Ins.Cover(UIPrefabNames);
	}

	void OnUISpawned() {
	}

	void OnUIDespawn() {
		StoryGuide_1_Game.Ins.GoNext = true;
	}
	#endregion


	public void OnBtnModeClick() {
		Client.User.UserInfo.Setting.ControlMode = GameControlMode.Btn;
		ModHelp.Ins.Back();
	}

	public void OnGravityModeClick() {
		Client.User.UserInfo.Setting.ControlMode = GameControlMode.GravitySwipe;
		ModHelp.Ins.Back();
	}

}
