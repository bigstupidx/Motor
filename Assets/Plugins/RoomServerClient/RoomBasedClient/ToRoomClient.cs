using System;
using System.Collections.Generic;
using ClientEngine;
using CommandModel;
using RoomBasedClient.Commands;
using Ping = RoomBasedClient.Commands.Ping;

namespace RoomBasedClient {
	public class ToRoomClient : RpcClient {
		public RoomClient RoomClient;
		public ToLobbyClient ToLobbyClient { get { return this.RoomClient.ToLobbyClient; } }

		private Action _onConnected;//用于连接后发送加入房间请求的回调
		private Action<Exception> _onConnectFail;

		public GameRoom Room { get; set; }

		public bool JoiningRoom = false;

		public bool InRoom {
			get { return this.Room != null; }
		}

		public RoomPlayer MimePlayer {
			get { return Room.MinePlayer; }
		}

		public ToRoomClient(RoomClient roomClient) : base(CommandExecuteMode.Manual) {
			this.RoomClient = roomClient;
		}

		private long _rpcId = 1;
		protected override string GetNewRpcId() {//在Unity中似乎GUID比较耗
			return this._rpcId++.ToString();
		}


		protected override void OnConnected() {
			base.OnConnected();
			this.JoiningRoom = true;
			this._pingResponsed = true;
			this._heartbeatTimer = 0;
			if (this._onConnected != null) {
				this._onConnected();
			}
		}

		protected override void OnConnectFail(Exception e) {
			base.OnConnectFail(e);
			if (this._onConnectFail != null) {
				this._onConnectFail(e);
			}
		}

		protected override void OnClosed(CloseCause cause) {
			base.OnClosed(cause);
			this.RoomClient.RunInMainThread(() => {
				if (this.Room != null) {
					this.Room.OnClose();
					this.Room = null;
					this.RoomClient.Callback.OnLeftRoom();
				}
				if (this.JoiningRoom) {
					Log.Warn("正在加入房间的时候 房间服务器断开了");
					if (this._onConnectFail != null) {
						this._onConnectFail(new Exception("加入房间时连接断开"));
					}
				}
				if (!this.RoomClient.ToLobbyClient.Connected) {
					this.RoomClient.Callback.OnDisconnected(cause);
				}
				this.JoiningRoom = false;
			});
		}

		internal void Connect(string host, int port, Action onConnected, Action<Exception> onConnectFail) {
			this._onConnected = onConnected;
			this._onConnectFail = onConnectFail;
			Log.Info("Connect RoomServer " + host + ":" + port);
			this.Connect(host, port);
		}

		internal void OnCreatedRoom(CreateRoomInRoomServerReply reply) {
			this.JoiningRoom = false;
			Dictionary<int, RoomPlayer> players = new Dictionary<int, RoomPlayer>();
			this.Room = reply.Room;
			this.Room.Session = this;

			this.Room.MinePlayer = reply.SelfPlayer;
			this.Room.MinePlayer.Session = this;
			this.Room.MinePlayer.Room = this.Room;

			players.Add(this.Room.MinePlayer.Id, this.Room.MinePlayer);
			foreach (RoomPlayer otherPlayerInfo in reply.OtherPlayers) {
				otherPlayerInfo.Room = this.Room;
				otherPlayerInfo.Session = null;
				players.Add(otherPlayerInfo.Id, otherPlayerInfo);
			}
			this.Room.Players = players;
			this.RoomClient.Callback.OnCreatedRoom();
		}

		internal void OnJoinRoom(JoinRoomInRoomServerReply reply) {
			this.JoiningRoom = false;
			Dictionary<int, RoomPlayer> players = new Dictionary<int, RoomPlayer>();
			this.Room = reply.Room;
			this.Room.Session = this;
			this.Room.MinePlayer = reply.SelfPlayer;
			this.Room.MinePlayer.Session = this;
			this.Room.MinePlayer.Room = this.Room;
			players.Add(this.Room.MinePlayer.Id, this.Room.MinePlayer);
			foreach (RoomPlayer otherPlayerInfo in reply.OtherPlayers) {
				otherPlayerInfo.Session = null;
				otherPlayerInfo.Room = this.Room;
				players.Add(otherPlayerInfo.Id, otherPlayerInfo);
			}
			this.Room.Players = players;
			this.RoomClient.Callback.OnJoinedRoom();
		}

		internal void ChangeMasterClient(int newMasterClientId) {
			foreach (var player in this.Room.Players) {
				player.Value.IsMaster = player.Key == newMasterClientId;
			}
			this.RoomClient.Callback.OnMasterClientSwitched(this.Room.Players[newMasterClientId]);
		}

		internal void OnPlayerJoin(RoomPlayer newPlayer) {
			newPlayer.Session = null;
			newPlayer.Room = this.Room;
			this.Room.Players.Add(newPlayer.Id, newPlayer);
			this.RoomClient.Callback.OnPlayerJoin(newPlayer);
		}
		internal void OnPlayerLeave(int playerId) {
			var leavePlayer = this.Room.Players[playerId];
			leavePlayer.OnLeave();
		}


		public void Update(float deltaTime) {
			if (this.Connected) {
				this._heartbeatTimer += deltaTime;
				if (this._pingResponsed && this._heartbeatTimer > this.heartbeatInterval) {
					SendPing();
				}
				if (this._heartbeatTimer > this.DisconnectTimeout) {
					Log.Info("TimeOut Close");
					this.Close(CloseCause.ClosedByClientTimeout);
				}
			}
		}


		public float DisconnectTimeout = 15f;
		private float _heartbeatTimer = 0;
		private float heartbeatInterval = 2;
		private bool _pingResponsed = true;

		Ping _ping = new Ping();
		private long _pingSendTime;
		public int Ping { get; private set; }
		private void SendPing() {
			if (this.Connected) {
				this.Send(this._ping);
			} else {
				throw new Exception("Can't send ping while no server connected");
			}
			this._pingResponsed = false;
			this._pingSendTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
		}

		internal void OnRevicePong() {
			this._pingResponsed = true;
			Ping = (int)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000 - this._pingSendTime);
			this._heartbeatTimer = 0;
			this.RoomClient.Callback.OnPingUpdate(Ping);
		}
	}
}