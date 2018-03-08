using UnityEngine;
using GameClient;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using ClientEngine;
using Game;
using Protocol;
using RoomBasedClient;
using RoomBasedClient.Commands;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;
using Random = UnityEngine.Random;

public partial class Lobby : RoomClientCallback {
	public static Lobby Ins { get; private set; }
	public enum PlayerState {
		None, Room, Loaded, ReadyGame, Gaming
	}

	public RoomClient RoomClient;

	public string Host;
	public int Port;

	public bool ConnectCanceled = false;

	void Start() {
		Ins = this;
		var hostPort = Client.Config.PvpHostPort.Split(':');
		Host = hostPort[0];
		Port = int.Parse(hostPort[1]);
		this.RoomClient = new RoomClient(Host, Port, this);
		this.RoomClient.ViewUpdateInterval = 0.025f;
		RoomPlayer.RegRpcs.Add("__StartGameRpc", this.__StartGameRpc);
		RoomPlayer.RegRpcs.Add("__StartOnlineModeCountdown", this.__StartOnlineModeCountdown);
		byte b = 200;
		SerializableTypeRegister.RegisterType(typeof(MatchInfo), b++, MatchInfo.OnlineSerialize, MatchInfo.OnlineDeserialize);
		SerializableTypeRegister.RegisterType(typeof(PlayerInfo), b++, PlayerInfo.OnlineSerialize, PlayerInfo.OnlineDeserialize);
		SerializableTypeRegister.RegisterType(typeof(List<PlayerInfo>), b++, PlayerInfo.OnlineSerializeList, PlayerInfo.OnlineDeserializeList);
		SerializableTypeRegister.RegisterType(typeof(BikeNetwork), b++, (stream, obj) => {
			BikeNetwork bike = (BikeNetwork)obj;
			stream.Write(bike.Id);
		}, stream => {
			int id = stream.ReadInt();
			return this.RoomClient.ToRoomClient.Room.Views[id];
		});
	}


	public void StartOnlineModeCountdown() {
		this.RoomClient.ToRoomClient.MimePlayer.CallRegRpc(BroadcastType.AllViaServer, "__StartOnlineModeCountdown");
	}
	private void __StartOnlineModeCountdown(object[] obj) {
		GameModeBase.Ins.StartOnlineMode();
	}

	protected void OnDestroy() {
		Ins = null;
		Disconnect();
	}

	void Update() {
		this.RoomClient.Update();
	}

	void SetPlayerInfo(RoomPlayer player, PlayerInfo playerInfo) {
		player.SetProp(new Dictionary<object, object>() {
			{ PlayerInfoKey, playerInfo }
		});
	}

	public void SetState(PlayerState playerState) {
		this.RoomClient.ToRoomClient.MimePlayer.SetProp(new Dictionary<object, object>() {
			{ Lobby.PlayerStateKey, playerState }
		});
	}

	public void Connect() {
		this.RoomClient._lobbyHost = this.Host;
		this.RoomClient._lobbyPort = this.Port;
		this.ConnectCanceled = false;
		WaittingTipWithCancel.Show(() => {
			this.ConnectCanceled = true;
			Disconnect();
		});
		this.RoomClient.ConnectLobby();
	}

	public override void OnConnectFail(Exception e) {
		Debug.Log("OnConnectionFail " + e.ToString().Colored(LogColors.green));
		if (this.ConnectCanceled) {//连接已经被取消
			return;
		}
		WaittingTipWithCancel.Hide();
		CommonDialog.Show("", "连接失败", "确定", () => {
			if (BikeManager.Ins != null) {
				ChooseOnlineMode.BackToMe();
			} else {
				ModMenu.Ins.BackTo(ChooseOnlineMode.GroupIns);
			}
			if (WaittingTipWithCancel.Ins != null) {
				WaittingTipWithCancel.Hide();
			}
		});
	}

	public void Disconnect() {
		Debug.Log("Try Disconnect ".Colored(LogColors.green));
		if (this.RoomClient.ToRoomClient.Connected) {
			this.RoomClient.ToRoomClient.Close(CloseCause.ClosedByClientLogic);
		}
		if (this.RoomClient.ToLobbyClient.Connected) {
			this.RoomClient.ToLobbyClient.Close(CloseCause.ClosedByClientLogic);
		}
	}

	public override void OnDisconnected(CloseCause cause) {
		Debug.Log("OnConnectionFail " + cause.ToString().Colored(LogColors.green));
		if (WaittingTip.Ins != null) {
			WaittingTip.Hide();
		}
		if (cause == CloseCause.ClosedByClientLogic) {
			return;
		}
		if (this.RoomClient.BeenKick) {
			return;
		}
		if (GameModeBase.Ins != null) {//游戏内断开
			CommonTip.Show("您已断开连接");
			GameUIInterface.Ins.ExitGame(ChooseOnlineMode.GroupIns, () => {
				ChooseOnlineMode.Ins.OnLobbyModeClicked();
			});

		} else {//游戏外断开
			ChooseOnlineMode.BackToMe();
			CommonDialog.Show("", "连接断开", "确定", () => {
				if (WaittingTipWithCancel.Ins != null) {
					WaittingTipWithCancel.Hide();
				}
			});
		}
	}

	public override void OnJoinedLobby() {
		base.OnJoinedLobby();
		Debug.Log("OnJoinedLobby ".Colored(LogColors.green));
		if (this.ConnectCanceled) {
			Debug.Log("进入大厅，但是用户已经取消连接 ".Colored(LogColors.green));
			Disconnect();
		} else {
			OnlineLobbyBoard.Show();
			WaittingTipWithCancel.Hide();
		}
	}

	public override void OnRoomListUpdate() {
		base.OnRoomListUpdate();
		Debug.Log("OnReceivedRoomListUpdate ".Colored(LogColors.green));
		if (OnlineLobbyBoard.Ins != null) {
			OnlineLobbyBoard.Ins.RefreshRoomList();
		}
	}

	public override void OnCreatedRoom() {
		base.OnCreatedRoom();
		OnJoinedRoom();
	}

	public override void OnCreateRoomFailed(CreateRoomResult result) {
		base.OnCreateRoomFailed(result);
		WaittingTipWithCancel.Hide();
		if (this.ConnectCanceled) {
			return;
		}
		CommonTip.Show("创建房间失败 " + GetCreateRoomFailReason(result));
	}

	public override void OnJoinedRoom() {
		base.OnJoinedRoom();

		WaittingTipWithCancel.Hide();
		if (this.ConnectCanceled) {
			return;
		}
		this.staringGame = false;
		ModMenu.Ins.BackTo(OnlineLobbyBoard.GroupIns);
		OnlineRoomBoard.Show();
		this.Robots = new List<PlayerInfo>();
		SetState(PlayerState.Room);
		if (this.RoomClient.ToRoomClient.Room.MinePlayer.IsMaster) {
			PlayerInfo info = (PlayerInfo)this.RoomClient.ToRoomClient.MimePlayer.CustomProperties[PlayerInfoKey];
			if (info.SpawnPos < 0) {
				info.SpawnPos = GetAvaliableSpwanPos();
				Debug.Log(("自己进入 位置是" + info.SpawnPos).Colored(LogColors.aqua));
				SetPlayerInfo(this.RoomClient.ToRoomClient.MimePlayer, info);
			}
		}
		var prop = this.RoomClient.ToRoomClient.Room.OtherCustomProperties;
		if (prop.ContainsKey(RobotsKey)) {
			this.Robots = prop[RobotsKey] as List<PlayerInfo>;
		} else {
			this.Robots = new List<PlayerInfo>();
		}
		RefreshPlayerList();
		StartCoroutine(AddRobot());
	}

	public override void OnJoinRoomFailed(JoinRoomResult result) {
		base.OnJoinRoomFailed(result);
		WaittingTipWithCancel.Hide();
		if (!this.ConnectCanceled) {
			CommonTip.Show("加入房间失败 " + GetJoinRoomFailReason(result));
		}
	}

	public override void OnLeftRoom() {
		StopAllCoroutines();
	}

	public override void OnBeenKickRoom() {
		base.OnBeenKickRoom();
		CommonTip.Show("你已被请出房间");
		if (RaceManager.Ins != null) {
			//游戏中
			GameUIInterface.Ins.ExitGame(ChooseOnlineMode.GroupIns, () => {
				ChooseOnlineMode.Ins.OnLobbyModeClicked();
			});
		} else {
			ChooseOnlineMode.BackAndEnterLobby();
		}
	}

	public List<PlayerInfo> Robots;
	IEnumerator AddRobot() {
		while (true) {
			yield return new WaitForSeconds(Random.Range(5, 10));
			if (!string.IsNullOrEmpty(this.RoomClient.ToRoomClient.Room.Password)) {
				continue;
			}
			//离开或房间关闭后结束协程
			if (!this.RoomClient.ToRoomClient.InRoom || !this.RoomClient.ToRoomClient.Room.IsOpen) {
				continue;
			}
			//			if (this.Robots.Count >= this.RoomClient.ToRoomClient.Room.MaxPlayerCount - 2) {//房间至少需要2个玩家
			//				continue;
			//			}
			if (this.RoomClient.ToRoomClient.Room.PlayerCount + this.Robots.Count < this.RoomClient.ToRoomClient.Room.MaxPlayerCount) {
				if (this.RoomClient.ToRoomClient.MimePlayer.IsMaster) {//由主机添加机器人
					var robotPlayerInfo = Client.Online.GetRandomPlayerInfo(-1);
					robotPlayerInfo.SpawnPos = GetAvaliableSpwanPos();//分配位置
					Debug.Log(("机器人进入 位置是" + robotPlayerInfo.SpawnPos).Colored(LogColors.aqua));
					this.Robots.Add(robotPlayerInfo);
					SetRoomRobots(this.Robots);
				}
			}
		}
	}

	public override void OnMasterClientSwitched(RoomPlayer newMasterClient) {
		base.OnMasterClientSwitched(newMasterClient);
		Debug.Log(("OnMasterClientSwitched To " + newMasterClient).Colored(LogColors.aqua));
		if (this.RoomClient.ToRoomClient.MimePlayer.IsMaster) {
			//改变大厅的头像和房主名称
			this.RoomClient.ToRoomClient.Room.SetLobbyProp(new Dictionary<object, object>() {
				{MasterNameKey, Client.User.UserInfo.Setting.Nickname },
				{MasterIconKey, Client.User.ChoosedHeroInfo.Data.Icon.ID}
			});
		}
		//对没有分配位置的重新分配位置
		foreach (var player in this.RoomClient.ToRoomClient.Room.Players.Values) {
			PlayerInfo playerInfo = (PlayerInfo)player.CustomProperties[PlayerInfoKey];
			if (playerInfo.SpawnPos < 0) {
				playerInfo.SpawnPos = GetAvaliableSpwanPos();
				Debug.Log(("主机更换，重新分配未分配位置的玩家 " + playerInfo.NickName + "  " + playerInfo.SpawnPos).Colored(LogColors.aqua));
				SetPlayerInfo(player, playerInfo);
			}
		}

		RefreshPlayerList();
	}

	public override void OnRoomSelfPropertiesChanged(Dictionary<object, object> changedPart) {
		base.OnRoomSelfPropertiesChanged(changedPart);
		if (changedPart.ContainsKey("MaxPlayerCount")) {//TODO HardCode 应该使用常量，并且简化名称降低传输量
			RefreshPlayerList();
		}
	}

	public override void OnRoomOtherPropertiesChanged(Dictionary<object, object> changedPart) {
		base.OnRoomOtherPropertiesChanged(changedPart);
		if (changedPart.ContainsKey(RobotsKey)) {
			this.Robots = this.RoomClient.ToRoomClient.Room.OtherCustomProperties[RobotsKey] as List<PlayerInfo>;
			if (this.Robots == null) {
				this.Robots = new List<PlayerInfo>();
			}
		}
		RefreshPlayerList();
	}

	public override void OnPlayerPropertiesChanged(RoomPlayer player, Dictionary<object, object> changedPart) {
		Debug.Log("OnPlayerPropertiesChanged".Colored(LogColors.green));
		foreach (var p in this.RoomClient.ToRoomClient.Room.Players.Values) {
			PlayerInfo info = (PlayerInfo)p.CustomProperties[PlayerInfoKey];
			Debug.Log(("spawnpos=" + info.SpawnPos + "   " + p.Id + p.CustomProperties.ToStringFull()).Colored(LogColors.blue));
			if (this.RoomClient.ToRoomClient.MimePlayer.IsMaster) {
				if (info.SpawnPos < 0) {
					info.SpawnPos = GetAvaliableSpwanPos();
					SetPlayerInfo(p, info);
				}
			}
		}
		if (this.RoomClient.ToRoomClient.InRoom) {
			RefreshPlayerList();
		}
	}

	public override void OnPlayerJoin(RoomPlayer newPlayer) {
		if (this.RoomClient.ToRoomClient.MimePlayer.IsMaster) {//主机为新进入的玩家分配位置
			PlayerInfo playerInfo = (PlayerInfo)newPlayer.CustomProperties[PlayerInfoKey];
			if (playerInfo.SpawnPos < 0) {
				playerInfo.SpawnPos = GetAvaliableSpwanPos();
				Debug.Log(("新玩家进入 位置是" + playerInfo.SpawnPos).Colored(LogColors.aqua));
				SetPlayerInfo(newPlayer, playerInfo);
			}
		}
		RefreshPlayerList();
	}

	public override void OnPlayerLeave(RoomPlayer otherPlayer) {
		base.OnPlayerLeave(otherPlayer);
		RefreshPlayerList();
	}

	public override void OnPingUpdate(int ping) {
		base.OnPingUpdate(ping);
		if (ping > 400) {
			CommonTip.Show("网络状态不佳 " + ping + "ms");//TODO 替换为图片标识
		}
	}

	public void RefreshPlayerList() {

		if (this.Robots == null) {
			this.Robots = new List<PlayerInfo>();
		}
		if (OnlineRoomBoard.Ins == null) {
			return;
		}
		var room = this.RoomClient.ToRoomClient.Room;
		if (room.MinePlayer.IsMaster) {
			if (!room.IsOpen) {
				room.IsOpen = true;
			}
		}


		foreach (var item in OnlineRoomBoard.Ins.Players) {
			item.SetAsEmpty();
		}
		bool allStateInRoom = true;
		foreach (var player in room.Players.Values) {
			PlayerInfo info = (PlayerInfo)player.CustomProperties.GetValue(PlayerInfoKey);
			if (info.SpawnPos < 0) {//没有分配位置，暂时不显示
				continue;
			}
			object state;
			if (!player.CustomProperties.TryGetValue(PlayerStateKey, out state)) {
				state = PlayerState.Room;
			}
			if ((PlayerState)state != PlayerState.Room) {
				allStateInRoom = false;
			}
			OnlineRoomBoard.Ins.Players[info.SpawnPos].SetAsPlayer(info, player.IsMine, player.IsMaster, room.MinePlayer.IsMaster, (PlayerState)state);
		}
		foreach (var info in this.Robots) {
			if (info.SpawnPos < 0) {
				continue;
			}
			OnlineRoomBoard.Ins.Players[info.SpawnPos].SetAsPlayer(info, false, false, room.MinePlayer.IsMaster, PlayerState.Room);
		}

		int needLock = OnlineRoomBoard.Ins.Players.Length - room.MaxPlayerCount;
		for (int i = OnlineRoomBoard.Ins.Players.Length - 1; i >= 0; i--) {//从后往前上锁
			if (needLock == 0) {
				break;
			}
			if (OnlineRoomBoard.Ins.Players[i].HasPlayer) {//跳过有人的
				continue;
			}
			OnlineRoomBoard.Ins.Players[i].SetAsLock();
			needLock--;
		}

		if (TotalPlayerCount >= this.RoomClient.ToRoomClient.Room.MaxPlayerCount) {//人满后，开始倒计时
			if (allStateInRoom) {
				this._startGameCoroutine = StartCoroutine(StartGameCoroutine());
			}
		}
	}

	public void StartImmediately() {
		if (this._startGameCoroutine != null) {
			StopCoroutine(this._startGameCoroutine);
		}
		this.RoomClient.ToRoomClient.Room.IsOpen = false;
		this.RoomClient.ToRoomClient.MimePlayer.CallRegRpc(BroadcastType.AllViaServer, "__StartGameRpc", GetMatchInfo());
	}

	public float StartGameCd = 10;
	public bool staringGame = false;
	private Coroutine _startGameCoroutine;
	IEnumerator StartGameCoroutine() {
		if (!this.staringGame) {
			this.staringGame = true;
			if (this.RoomClient.ToRoomClient.MimePlayer.IsMaster) {
				this.RoomClient.ToRoomClient.Room.IsOpen = false;
			}
			StartGameCd = 10;
			while (true) {
				if (OnlineRoomBoard.Ins == null) {
					break;
				}
				var room = this.RoomClient.ToRoomClient.Room;
				if (room.PlayerCount + this.Robots.Count < room.MaxPlayerCount) {//中途有玩家退出
					room.IsOpen = true;
					this.staringGame = false;
					OnlineRoomBoard.Ins.StartGameText.text = "开始游戏";
					break;
				}
				this.StartGameCd -= Time.unscaledDeltaTime;
				if (this.StartGameCd <= 0) {
					this.StartGameCd = 0;
					this.staringGame = false;
					OnlineRoomBoard.Ins.StartGameText.text = "即将开始";
					if (this.RoomClient.ToRoomClient.MimePlayer.IsMaster) {//时间到，主机通知开始
						StartImmediately();
					}
				}
				OnlineRoomBoard.Ins.StartGameText.text = "开始游戏 " + (int)this.StartGameCd;
				yield return null;
			}
		}
		yield return null;
	}

	public void __StartGameRpc(object[] args) {
		this.staringGame = false;
		Client.Game.StartGame((MatchInfo)args[0]);
	}

	public MatchInfo GetMatchInfo() {
		GameRoom room = this.RoomClient.ToRoomClient.Room;
		var sceneData = GetSceneData((int)room.LobbyCustomProperties[MapKey]);
		GameMode mode = GetGameMode((int)room.LobbyCustomProperties[ModeKey]);
		string objLine = mode == GameMode.RacingProp ? sceneData.RandomPropObjLine : sceneData.RandomRaceObjLine;
		var data = new MatchData {
			ID = -1,
			GameMode = mode,
			Scene = sceneData,
			RaceLine = sceneData.RandomRaceLine,
			Turn = (int)room.LobbyCustomProperties[LapCountKey],
			ObjLine = objLine
		};
		List<PlayerInfo> list = new List<PlayerInfo>();
		var info = new MatchInfo(data, list, MatchMode.Online);
		return info;
	}


	public int TotalPlayerCount {
		get {
			return this.RoomClient.ToRoomClient.Room.PlayerCount + this.Robots.Count;
		}
	}

	public void SetRoomRobots(List<PlayerInfo> value) {
		this.RoomClient.ToRoomClient.Room.SetOtherProp(new Dictionary<object, object>() { { RobotsKey, value } });
		this.RoomClient.ToRoomClient.Room.SetLobbyProp(new Dictionary<object, object>() {
			{RobotCountKey, this.Robots.Count }
		});
	}

	private int GetAvaliableSpwanPos() {
		List<int> alreadyUsed = new List<int>();
		foreach (var player in this.RoomClient.ToRoomClient.Room.Players.Values) {
			PlayerInfo p = (PlayerInfo)player.CustomProperties[PlayerInfoKey];
			alreadyUsed.Add(p.SpawnPos);
		}
		if (this.Robots == null) {
			this.Robots = new List<PlayerInfo>();
		}
		foreach (var robot in this.Robots) {
			alreadyUsed.Add(robot.SpawnPos);
		}
		for (int i = 0; i < 10; i++) {
			if (!alreadyUsed.Contains(i)) {
				return i;
			}
		}
		return -1;
	}

	public void KickPlayer(RoomPlayer player) {
		this.RoomClient.ToRoomClient.Send(new KickPlayer() {
			PlayerId = player.Id
		});
	}

}
