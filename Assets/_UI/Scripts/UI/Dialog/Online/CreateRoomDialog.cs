using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using GameUI;
using UnityEngine.UI;
using XUI;

public class CreateRoomDialog : UIStackBehaviour {

	#region base
	public const string UIPrefabPath = "UI/Dialog/Online/CreateRoomDialog";

	public static void Show() {
		string[] UIPrefabNames ={
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath,
		};
		ModMenu.Ins.Overlay(UIPrefabNames);

	}
	public override void OnUISpawned() {
		base.OnUISpawned();
		this.NeedPasswordToggle.isOn = false;
		this.PlayerCountSpin.Init(6, 2, 6, null);
		this.LapCountSpin.Init(2, 1, 3, null);
		this.MapSpin.Init(Lobby.MapOptions, 0, true, null);
		this.ModeSpin.Init(Lobby.GameModeOptions, 0, true, null);
	}

	#endregion

	public Toggle NeedPasswordToggle;
	public NumSpin PlayerCountSpin;
	public OptionSpin MapSpin;
	public NumSpin LapCountSpin;
	public OptionSpin ModeSpin;

	public void OnCreateRoomClick() {
		bool needPassword = this.NeedPasswordToggle.isOn;
		int playerCount = this.PlayerCountSpin.Value;
		int mapIndex = this.MapSpin.Index;
		int lapCount = this.LapCountSpin.Value;
		int modeIndex = this.ModeSpin.Index;
		Lobby.Ins.ConnectCanceled = false;
		WaittingTipWithCancel.Show("正在创建房间...",() => {
			Lobby.Ins.ConnectCanceled = true;
			Lobby.Ins.Disconnect();
			ChooseOnlineMode.BackToMe();
		});
		string password = needPassword ? Lobby.GeneratePassword() : null;
		Lobby.Ins.RoomClient.CreateRoom(Guid.NewGuid().ToString(), playerCount, password, new Dictionary<object, object>() {
			{Lobby.MapKey,mapIndex },
			{Lobby.LapCountKey,lapCount },
			{Lobby.ModeKey,modeIndex },
			{Lobby.MasterNameKey,Client.User.UserInfo.Setting.Nickname},
			{Lobby.MasterIconKey,Client.User.ChoosedPlayerInfo.ChoosedHero.Data.Icon.ID},
		}, null, Client.User.UserInfo.Setting.Nickname, new Dictionary<object, object>() {
			{Lobby.PlayerInfoKey,Client.User.ChoosedPlayerInfo },
			{Lobby.PlayerStateKey,Lobby.PlayerState.None }
		});

	}


}
