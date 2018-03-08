using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using GameUI;
using UnityEngine.UI;
using XUI;

public class OnlineRoomBoard : SingleUIStackBehaviour<OnlineRoomBoard> {

	#region base

	public const string UIPrefabPath = "UI/Dialog/Online/OnlineRoomBoard";

	public static void Show() {
		string[] UIPrefabNames ={
			UICommonItem.MENU_BACKGROUND,
			UIPrefabPath,
		};
		GroupIns = ModMenu.Ins.Cover(UIPrefabNames);
	}

	public override void OnUISpawned() {
		base.OnUISpawned();
		Lobby.Ins.SetState(Lobby.PlayerState.Room);
		this.RoomIdText.text = Lobby.Ins.RoomClient.ToRoomClient.Room.Name;
		this.StartGameText.text = "开始游戏";
	}

	#endregion

	public static UIGroup GroupIns { get; private set; }
	public RoomPlayerItem[] Players;
	public Text RoomIdText;
	public Text PwdText;
	public Text ModeText;
	public Text MapText;
	public Text LapText;
	public Text StartGameText;

	void OnValidate() {
		this.Players = GetComponentsInChildren<RoomPlayerItem>();
		for (var i = 0; i < this.Players.Length; i++) {
			var item = this.Players[i];
			item.Index = i;
		}
	}

	void Update() {
		var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
		this.PwdText.text = string.IsNullOrEmpty(room.Password) ? "无" : room.Password;
		this.ModeText.text = Lobby.GetGameModeStr((int)room.LobbyCustomProperties[Lobby.ModeKey]);
		this.MapText.text = Lobby.GetMapName((int)room.LobbyCustomProperties[Lobby.MapKey]);
		this.LapText.text = room.LobbyCustomProperties[Lobby.LapCountKey] + "圈";
	}

	public void __OnStartGameClick() {
		if (!Lobby.Ins.RoomClient.ToRoomClient.Room.MinePlayer.IsMaster) {
			CommonTip.Show("只有房主可以进行本操作");
			return;
		} else if (Lobby.Ins.TotalPlayerCount < 2) {
			CommonTip.Show("至少需要两名玩家");
		} else {
			bool allStateInRoom = true;
			var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
			foreach (var player in room.Players.Values) {
				object state;
				if (!player.CustomProperties.TryGetValue(Lobby.PlayerStateKey, out state)) {
					state = Lobby.PlayerState.None;
				}
				if ((Lobby.PlayerState)state != Lobby.PlayerState.Room) {
					allStateInRoom = false;
					break;
				}
			}
			if (!allStateInRoom) {
				CommonTip.Show("玩家尚未准备好");
			} else {
				Lobby.Ins.StartImmediately();
			}
		}
	}

	public void __OnRoomSettingClicked() {
		if (!Lobby.Ins.RoomClient.ToRoomClient.Room.MinePlayer.IsMaster) {
			CommonTip.Show("只有房主可以进行本操作");
		} else {
			RoomSettingDialog.Show();
		}
	}

	public void __OnBackClicked() {
		Lobby.Ins.Disconnect();
		ChooseOnlineMode.BackAndEnterLobby();
	}


	public void OnKickClick(int index) {
		var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
		if (!room.MinePlayer.IsMaster) {
			CommonTip.Show("只有房主可以进行本操作");
		} else {
			foreach (var player in room.Players.Values) {
				int spawnPos = ((PlayerInfo)player.CustomProperties[Lobby.PlayerInfoKey]).SpawnPos;
				if (spawnPos == index) {
					Lobby.Ins.KickPlayer(player);
					return;
				}
			}
			foreach (var robot in Lobby.Ins.Robots) {
				if (robot.SpawnPos == index) {
					Lobby.Ins.Robots.Remove(robot);
					Lobby.Ins.SetRoomRobots(Lobby.Ins.Robots);
					return;
				}
			}
		}
	}
}
