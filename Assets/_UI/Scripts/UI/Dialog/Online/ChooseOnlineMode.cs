using System;
using GameClient;
using GameUI;
using UnityEngine;
using XUI;

public class ChooseOnlineMode : SingleUIStackBehaviour<ChooseOnlineMode> {

	public const string PrefabPath = "UI/Dialog/Online/ChooseOnlineMode";

	public static void Show() {
		string[] prefabNames ={
			UICommonItem.MENU_BACKGROUND,
			PrefabPath,
			UICommonItem.TOP_BOARD_BACK
		};
		GroupIns = ModMenu.Ins.Cover(prefabNames, "ChooseOnlineMode");
	}

	public static void BackToMe() {
		if (GroupIns == null) {
			throw new Exception("ChooseOnlineMode 不在栈中，无法后退至此");
		}
		ModMenu.Ins.BackTo(GroupIns);
	}

	public static void BackAndEnterLobby() {
		if (GroupIns == null) {
			throw new Exception("ChooseOnlineMode 不在栈中，无法后退至此");
		}
		ModMenu.Ins.BackTo(GroupIns);
		Ins.OnLobbyModeClicked();
	}

	public override void OnUILeaveStack() {
		base.OnUILeaveStack();
		GroupIns = null;
	}

	public override void OnUISpawned() {
		base.OnUISpawned();
		this.SensorOnlyTag.SetActive(Client.Config.OnlySensorCanEnterLobby);
	}

	public static UIGroup GroupIns { get; private set; }

	public GameObject SensorOnlyTag;

	public void OnRandomModeClicked() {
		NetMatchBoard.Show();
	}

	public void OnLobbyModeClicked() {
		if (Client.Config.OnlySensorCanEnterLobby) {
			if (!BikeOrbecController.HasOrbbecDevice()) {
				CommonDialog.Show("仅限体感设备用户进入", "如果您已拥有体感设备，请将其连接到电视设备", LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), () => QRCodeAdDialog.Show());
				return;
			}
		}
		Lobby.Ins.Connect();
	}


}
