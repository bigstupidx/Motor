using System.Collections.Generic;
using UnityEngine;
using GameClient;
using GameUI;
using UnityEngine.UI;

public class InputRoomIdDialog : MonoBehaviour {

	#region base
	public const string UIPrefabPath = "UI/Dialog/Online/InputRoomIdDialog";

	public static void Show() {
		string[] UIPrefabNames ={
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath,
		};
		ModMenu.Ins.Overlay(UIPrefabNames);

	}
	#endregion

	public InputField RoomIdInput;
	public InputField PasswordInput;

	public void __OnJoinRoomClick() {

		string roomId = this.RoomIdInput.text;
		string password = this.PasswordInput.text;

		if (string.IsNullOrEmpty(roomId)) {
			CommonTip.Show("房间号不能为空");
		}

		Lobby.Ins.ConnectCanceled = false;
		WaittingTipWithCancel.Show("正在加入房间...",() => {
			Lobby.Ins.ConnectCanceled = true;
			Lobby.Ins.Disconnect();
			ChooseOnlineMode.BackToMe();
		});
		Lobby.Ins.RoomClient.JoinRoom(roomId, password, Client.User.UserInfo.Setting.Nickname, new Dictionary<object, object>() {
			{Lobby.PlayerInfoKey,Client.User.ChoosedPlayerInfo },
			{ Lobby.PlayerStateKey,Lobby.PlayerState.None }
		});


	}


}
