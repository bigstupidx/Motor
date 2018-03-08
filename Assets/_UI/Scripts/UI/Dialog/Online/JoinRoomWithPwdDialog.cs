using System.Collections.Generic;
using GameClient;
using RoomBasedClient;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class JoinRoomWithPwdDialog : UIStackBehaviour {

		public const string UIPrefabPath = "UI/Dialog/Online/JoinRoomWithPwdDialog";

		public static void Show(LobbyRoom room) {
			string[] UIPrefabNames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath,
			};
			JoinRoomWithPwdDialog ins = ModMenu.Ins.Overlay(UIPrefabNames)[1].Instance.GetComponent<JoinRoomWithPwdDialog>();
			ins.Room = room;
			ins.PwdInput.text = "";
		}


		public override void OnUISpawned() {
			base.OnUISpawned();
			this.PwdInput.text = "";
		}


		public InputField PwdInput;
		public LobbyRoom Room;

		public void __OnJoinClick() {
			if (this.PwdInput.text != this.Room.Password) {
				CommonTip.Show("密码错误");
				return;
			} else {
				Lobby.Ins.ConnectCanceled = false;
				WaittingTipWithCancel.Show("正在加入房间......",() => {
					Lobby.Ins.ConnectCanceled = true;
					Lobby.Ins.Disconnect();
					ChooseOnlineMode.BackToMe();
				});
				Lobby.Ins.RoomClient.JoinRoom(this.Room.Name, this.PwdInput.text, Client.User.UserInfo.Setting.Nickname, new Dictionary<object, object>() {
					{Lobby.PlayerInfoKey,Client.User.ChoosedPlayerInfo },
					{Lobby.PlayerStateKey,Lobby.PlayerState.None }
				});
			}

		}

	}
}