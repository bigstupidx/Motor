using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using GameUI;
using UnityEngine.UI;
using XUI;

public class RoomSettingDialog : MonoBehaviour, IUIPoolBehaviour {

	#region base
	public const string UIPrefabPath = "UI/Dialog/Online/RoomSettingDialog";

	public static void Show() {
		string[] UIPrefabNames ={
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath,
		};
		ModMenu.Ins.Overlay(UIPrefabNames);
	}
	public void OnUISpawned() {
		var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
		this.NeedPasswordToggle.isOn = !string.IsNullOrEmpty(room.Password);
		this.NeedPasswordToggle.onValueChanged.RemoveAllListeners();
		this.NeedPasswordToggle.onValueChanged.AddListener(isOn => {
			if (!isOn) {
				this.ResetPwdBtn.gameObject.SetActive(false);
				this.CurrentPwdText.text = "";
				this.CurrentPwdText.gameObject.SetActive(false);
			} else {
				this.ResetPwdBtn.gameObject.SetActive(true);
				this.CurrentPwdText.text = Lobby.GeneratePassword();
			}
		});
		this.CurrentPwdText.text = room.Password == null ? "" : room.Password;
		this.PlayerCountSpin.Init(room.MaxPlayerCount, 2, 6, null);
		this.MapSpin.Init(Lobby.MapOptions, (int)room.LobbyCustomProperties[Lobby.MapKey], true, null);
		this.LapCountSpin.Init((int)room.LobbyCustomProperties[Lobby.LapCountKey], 1, 3, null);
		this.ModeSpin.Init(Lobby.GameModeOptions, (int)room.LobbyCustomProperties[Lobby.ModeKey], true, null);
	}

	public void OnUIDespawn() {
	}
	#endregion

	public Toggle NeedPasswordToggle;
	public Button ResetPwdBtn;
	public Text CurrentPwdText;
	public NumSpin PlayerCountSpin;
	public OptionSpin MapSpin;
	public NumSpin LapCountSpin;
	public OptionSpin ModeSpin;

	public void __OnChangeSettingClicked() {
		bool needPassword = this.NeedPasswordToggle.isOn;
		string password = this.CurrentPwdText.text;
		int playerCount = this.PlayerCountSpin.Value;
		int mapIndex = this.MapSpin.Index;
		int lapCount = this.LapCountSpin.Value;
		int modeIndex = this.ModeSpin.Index;

		var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
		room.Password = password;
		room.MaxPlayerCount = playerCount;
		room.SetLobbyProp(new Dictionary<object, object>() {
			{Lobby.MapKey,mapIndex },
			{Lobby.LapCountKey,lapCount },
			{Lobby.ModeKey,modeIndex },
		});
		ModMenu.Ins.Back();
	}

	public void __OnResetPwdClicked() {
		this.CurrentPwdText.text = Lobby.GeneratePassword();
	}


}
