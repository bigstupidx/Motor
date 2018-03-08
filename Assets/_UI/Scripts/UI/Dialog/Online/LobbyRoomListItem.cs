using System;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using GameClient;
using RoomBasedClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class LobbyRoomListItem : EnhancedScrollerCellView {

		public Image Icon;
		public Text RoomId;
		public GameObject PasswordFlag;
		public Text RoomMasterName;
		public Text GameMode;
		public Text MapName;
		public Text LapCount;
		public Text PlayerCount;


		public LobbyRoom Room;

		public void SetData(int i, LobbyRoom fetchedRoom) {
			this.Room = fetchedRoom;
			this.RoomId.text = fetchedRoom.Name;
			this.PasswordFlag.SetActive(!string.IsNullOrEmpty(fetchedRoom.Password));
			int robotCount = 0;
			if (fetchedRoom.LobbyCustomProperties.ContainsKey(Lobby.RobotCountKey)) {
				robotCount = (int)fetchedRoom.LobbyCustomProperties[Lobby.RobotCountKey];
			}
            this.PlayerCount.text = LString.LobbyRoomListItem_Player .ToLocalized() + (fetchedRoom.PlayerCount + robotCount) + "/" + fetchedRoom.MaxPlayerCount; ;// "玩家:" + (fetchedRoom.PlayerCount + robotCount) + "/" + fetchedRoom.MaxPlayerCount;
			this.GameMode.text = Lobby.GetGameModeStr((int)fetchedRoom.LobbyCustomProperties[Lobby.ModeKey]);
			this.MapName.text = Lobby.GetMapName((int)fetchedRoom.LobbyCustomProperties[Lobby.MapKey]);
            this.LapCount.text = fetchedRoom.LobbyCustomProperties[Lobby.LapCountKey].ToString() + LString.LobbyRoomListItem_Loop.ToLocalized();// "圈";
			this.RoomMasterName.text = fetchedRoom.LobbyCustomProperties[Lobby.MasterNameKey].ToString();
			this.Icon.sprite = Client.Icon[(int)fetchedRoom.LobbyCustomProperties[Lobby.MasterIconKey]].Sprite;
		}
         
		public void __OnJoinClicked() {
			if (string.IsNullOrEmpty(this.Room.Password)) {
				Lobby.Ins.ConnectCanceled = false;//正在加入房间.
                WaittingTipWithCancel.Show(LString.LobbyRoomListItem_JoinRoom.ToLocalized(), () => {
					Lobby.Ins.ConnectCanceled = true;
					Lobby.Ins.Disconnect();
					ChooseOnlineMode.BackToMe();
				});
				Lobby.Ins.RoomClient.JoinRoom(this.Room.Name, null, Client.User.UserInfo.Setting.Nickname, new Dictionary<object, object>() {
					{Lobby.PlayerInfoKey,Client.User.ChoosedPlayerInfo },
					{Lobby.PlayerStateKey,Lobby.PlayerState.None }
				});
			} else {
				JoinRoomWithPwdDialog.Show(this.Room);
			}
		}
	}
}