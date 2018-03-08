using System;
using System.Collections.Generic;
using System.IO;
using Protocol;
using RoomBasedClient.Commands;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient {
	public class GameRoom {
		public static readonly int MaxViewId = 999;

		private Dictionary<object, object> _selfProperties = new Dictionary<object, object>();
		public Dictionary<object, object> LobbyCustomProperties = new Dictionary<object, object>();
		public Dictionary<object, object> OtherCustomProperties = new Dictionary<object, object>();

		public ToRoomClient Session { get; set; }
		public RoomPlayer MinePlayer { get; set; }

		public Dictionary<int, RoomPlayer> Players = new Dictionary<int, RoomPlayer>();
		public Dictionary<int, RoomView> Views = new Dictionary<int, RoomView>();

		internal int GetNewSceneViewId() {
			for (int i = 1; i < MaxViewId; i++) {
				if (!this.Views.ContainsKey(i)) {
					return i;
				}
			}
			throw new Exception("No More Scene View Id Can allocate");
		}

		public string Name {
			get { return (string)this._selfProperties["Name"]; }
			//房间名字在创建的时候确定后不能再修改
		}

		public int MaxPlayerCount {
			get { return (int)this._selfProperties["MaxPlayerCount"]; }
			set {
				if (MaxPlayerCount != value) {
					SendSelfPropChange("MaxPlayerCount", value);
				}
			}
		}

		public string Password {
			get { return (string)this._selfProperties["Password"]; }
			set {
				if (Password != value) {
					SendSelfPropChange("Password", value);
				}

			}
		}

		public bool IsOpen {
			get { return (bool)this._selfProperties["IsOpen"]; }
			set {
				if (IsOpen != value) {
					SendSelfPropChange("IsOpen", value);
				}
			}
		}

		public int PlayerCount {
			get { return this.Players.Count; }
		}

		public bool IsRoomFull {
			get { return this.PlayerCount >= this.MaxPlayerCount; }
		}

		private void SendSelfPropChange(object key, object value) {
			var changedPart = new Dictionary<object, object>();
			changedPart.Add(key, value);
			UpdateSelfPropInternal(changedPart);
			this.Session.Send(new SetRoomSelfProp() {
				ChangedPart = changedPart
			});
		}

		internal void UpdateSelfPropInternal(Dictionary<object, object> changedPart) {
			this._selfProperties.AddOrUpdateOrRemove(changedPart);
			this.Session.RoomClient.Callback.OnRoomSelfPropertiesChanged(changedPart);
		}

		public void SetLobbyProp(Dictionary<object, object> changedPart) {
			foreach (var o in changedPart) {
				if (!SerializableTypeRegister.IsInternalType(o.Key.GetType())) {
					throw new ArgumentException(string.Format("Lobby Properties Only Support Internal Type ,{0}  is not internal type", o.Key.GetType()));
				}
				if (o.Value != null && !SerializableTypeRegister.IsInternalType(o.Value.GetType())) {
					throw new ArgumentException(string.Format("Lobby Properties Only Support Internal Type ,{0}  is not internal type", o.Value.GetType()));
				}
			}
			SetLobbyPropInternal(changedPart);
			this.Session.Send(new SetRoomLobbyProp() {
				ChangedPart = changedPart
			});
		}

		internal void SetLobbyPropInternal(Dictionary<object, object> changedPart) {
			if (this.LobbyCustomProperties == null) {
				this.LobbyCustomProperties = new Dictionary<object, object>();
			}
			this.LobbyCustomProperties.AddOrUpdateOrRemove(changedPart);
			this.Session.RoomClient.Callback.OnRoomLobbyPropertiesChanged(changedPart);
		}

		public void SetOtherProp(Dictionary<object, object> changedPart) {
			SetOtherPropInternal(changedPart);
			this.Session.Send(new SetRoomOtherProp() {
				ChangedPart = changedPart
			});
		}

		internal void SetOtherPropInternal(Dictionary<object, object> changedPart) {
			if (this.OtherCustomProperties == null) {
				this.OtherCustomProperties = new Dictionary<object, object>();
			}
			this.OtherCustomProperties.AddOrUpdateOrRemove(changedPart);
			this.Session.RoomClient.Callback.OnRoomOtherPropertiesChanged(changedPart);
		}


		public void Serilize(Stream stream) {
			lock (this._selfProperties) {
				stream.WriteObject(this._selfProperties);
			}
			if (this.LobbyCustomProperties == null) {
				stream.WriteObject(null);
			} else {
				lock (this.LobbyCustomProperties) {
					stream.WriteObject(this.LobbyCustomProperties);
				}
			}
			if (this.OtherCustomProperties == null) {
				stream.WriteObject(null);
			} else {
				lock (this.OtherCustomProperties) {
					//自定义属性允许服务器无法序列化的类型，所以转为bytes后再传到服务器
					var bytesDict = DictionaryExtensions.PropToBytesDict(this.OtherCustomProperties);
					stream.WriteObject(bytesDict);
				}
			}
		}

		public void Deserilize(Stream stream) {
			this._selfProperties = stream.ReadObject<Dictionary<object, object>>();
			this.LobbyCustomProperties = stream.ReadObject<Dictionary<object, object>>();
			//服务器传来的是包含置空的值，需要排除掉
			this.OtherCustomProperties = DictionaryExtensions.BytesDictToProp(stream.ReadObject<Dictionary<object, object>>(), false);
		}

		public override string ToString() {
			string lobbyProp = this.LobbyCustomProperties == null ? "null" : this.LobbyCustomProperties.ToStringFull();
			string otherProp = this.OtherCustomProperties == null ? "null" : this.OtherCustomProperties.ToStringFull();
			return string.Format("GameRoom Name={0} Player={1}/{2} Password={3} IsOpen={4} LobbyProp={5} OtherProp={6}",
				Name, PlayerCount, MaxPlayerCount, Password, IsOpen, lobbyProp, otherProp);
		}

		public void OnClose() {
			foreach (var roomView in this.Views.Values) {
				if (roomView != null) {
					UnityEngine.Object.Destroy(roomView.gameObject);
				} else {
					//可能在退出场景时移除了
				}
			}
		}
	}
}