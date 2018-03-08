using System;
using System.Collections.Generic;
using ClientEngine;
using CommandModel;
using RoomBasedClient.Commands;
using RoomServerModel;
using Ping = RoomBasedClient.Commands.Ping;

namespace RoomBasedClient {
	public class ToLobbyClient : RpcClient {
		public RoomClient RoomClient;
		public ToRoomClient ToRoomClient { get { return this.RoomClient.ToRoomClient; } }

		public List<LobbyRoom> FetchedRooms = new List<LobbyRoom>();

		public ToLobbyClient(RoomClient roomClient) : base(CommandExecuteMode.Manual) {
			this.RoomClient = roomClient;
		}

		private long _rpcId = 1;
		protected override string GetNewRpcId() {//在Unity中这么做似乎比guid省一点
			return this._rpcId++.ToString();
		}

		protected override void OnConnected() {
			base.OnConnected();
			this._pingResponsed = true;
			this._heartbeatTimer = 0;
			this.RoomClient.RunInMainThread(() => {
				this.RoomClient.Callback.OnConnected();
				this.RoomClient.Callback.OnJoinedLobby();
			});
			FetchRooms(0, 100);//连接后自动获取房间列表
			if (this.ToRoomClient.Connected) {//从房间退回到大厅后关闭房间连接
				this.ToRoomClient.Close(CloseCause.ClosedByClientLogic);
			}
		}

		protected override void OnConnectFail(Exception e) {
			base.OnConnectFail(e);
			this.RoomClient.RunInMainThread(() => {
				this.RoomClient.Callback.OnConnectFail(e);
			});
		}

		protected override void OnClosed(CloseCause cause) {
			base.OnClosed(cause);
			this.RoomClient.RunInMainThread(() => {
				this.RoomClient.Callback.OnLeftLobby();
				if (!this.RoomClient.ToRoomClient.Connected) {
					this.RoomClient.Callback.OnDisconnected(cause);
				}
			});
		}

		public void FetchRooms(int offset, int limit) {
			RpcAsync(new FetchRoomsRequest() {
				Offset = 0,
				Limit = 100
			}, result => {
				FetchRoomsReply reply = (FetchRoomsReply)result;
				this.FetchedRooms = new List<LobbyRoom>();
				foreach (LobbyRoom room in reply.Rooms) {
					this.FetchedRooms.Add(room);
				}
				this.RoomClient.RunInMainThread(() => {
					this.RoomClient.Callback.OnRoomListUpdate();
				});
			});
		}

		public void CreateRoom(string name, int maxPlayerCount, string password,
			Dictionary<object, object> lobbyCustomProperties,
			Dictionary<object, object> otherCustomProperties,
			string playerName,
			Dictionary<object, object> playerProperties) {
			RpcAsync(new CreateRoomInLobbyRequest() {
				Name = name,
				Password = password,
				MaxPlayerCount = maxPlayerCount,
				LobbyCustomProperties = lobbyCustomProperties
			}, result => {
				CreateRoomInLobbyReply reply = (CreateRoomInLobbyReply)result;
				if (reply.Result == CreateRoomResult.Success) {
					this.ToRoomClient.Connect(reply.RoomServerHostPort.Host, reply.RoomServerHostPort.Port, () => {
						CreateRoomInRoomServerRequest createRequest = new CreateRoomInRoomServerRequest() {
							Name = reply.Name,
							MaxPlayerCount = maxPlayerCount,
							Password = password,
							LobbyCustomProperties = lobbyCustomProperties,
							OtherCustomProperties = otherCustomProperties,
							Token = reply.Token,
							PlayerName = playerName,
							PlayerProperties = playerProperties
						};
						this.ToRoomClient.RpcAsync(createRequest, joinResult => {
							CreateRoomInRoomServerReply joinReply = (CreateRoomInRoomServerReply)joinResult;
							if (joinReply.Result != JoinRoomResult.Success) {
								this.Log.Error("Create room fail when join: " + reply.Result);
								this.RoomClient.Callback.OnCreateRoomFailed(reply.Result);
							} else {
								this.ToRoomClient.OnCreatedRoom(joinReply);
								Close(CloseCause.ClosedByClientLogic);
							}
						});

					}, exception => {
						this.Log.Error("connect to room server fail", exception);
						this.RoomClient.RunInMainThread(() => {
							this.RoomClient.Callback.OnCreateRoomFailed(CreateRoomResult.ConnectToRoomServerFail);
						});
					});
				} else {
					this.Log.Error("create room in lobby fail :" + reply.Result);
					this.RoomClient.Callback.OnCreateRoomFailed(reply.Result);
				}
			});
		}

		public void JoinRoom(string name, string password, string playerName, Dictionary<object, object> playerProperties) {
			RpcAsync(new JoinRoomInLobbyRequest() {
				Name = name,
				Password = password
			}, result => {
				JoinRoomInLobbyReply reply = (JoinRoomInLobbyReply)result;
				if (reply.Result == JoinRoomResult.Success) {
					this.ToRoomClient.Connect(reply.RoomServerHostPort.Host, reply.RoomServerHostPort.Port, () => {
						this.ToRoomClient.RpcAsync(new JoinRoomInRoomServerRequest() {
							Name = name,
							Password = password,
							PlayerName = playerName,
							PlayerProperties = playerProperties
						}, joinResult => {
							JoinRoomInRoomServerReply joinReply = (JoinRoomInRoomServerReply)joinResult;
							if (joinReply.Result != JoinRoomResult.Success) {
								this.Log.Error("Join room fail : " + joinReply.Result);
								this.RoomClient.Callback.OnJoinRoomFailed(joinReply.Result);
							} else {
								this.ToRoomClient.OnJoinRoom(joinReply);
								this.Close(CloseCause.ClosedByClientLogic);
							}
						});
					}, exception => {
						this.Log.Error("connect to room server fail", exception);
						this.RoomClient.RunInMainThread(() => {
							this.RoomClient.Callback.OnJoinRoomFailed(JoinRoomResult.ConnectToRoomServerFail);
						});
					});
				} else {
					this.Log.Error("join room fail : " + reply.Result);
					this.RoomClient.Callback.OnJoinRoomFailed(reply.Result);
				}
			});

		}


		//TODO 和ToRoomClient代码重复，考虑复用
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
		private float heartbeatInterval = 2;
		private float _heartbeatTimer = 0;
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