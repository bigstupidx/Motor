using System;
using System.Collections.Generic;
using System.IO;
using ClientEngine;
using CommandModel;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class CreateRoomInRoomServerRequest : RpcRequest {
		public string Name;
		public int MaxPlayerCount;
		public string Password;
		public Dictionary<object, object> LobbyCustomProperties;
		public Dictionary<object, object> OtherCustomProperties;
		public string Token;

		public string PlayerName;
		public Dictionary<object, object> PlayerProperties;

		public override void OnSendRpc(Stream stream) {
			stream.Write(this.Name);
			stream.Write(this.MaxPlayerCount);
			stream.Write(this.Password);
			stream.WriteObject(this.LobbyCustomProperties);
			stream.WriteObject(DictionaryExtensions.PropToBytesDict(this.OtherCustomProperties));//ע��Ҫ ToBytesDict
			stream.Write(this.Token);
			stream.Write(this.PlayerName);
			stream.WriteObject(DictionaryExtensions.PropToBytesDict(this.PlayerProperties));//!!!
		}

		public override void OnReceiveRpc(object session, Stream stream) {
			this.Name = stream.ReadString();
			this.MaxPlayerCount = stream.ReadInt();
			this.Password = stream.ReadString();
			this.LobbyCustomProperties = stream.ReadObject<Dictionary<object, object>>();
			this.OtherCustomProperties = DictionaryExtensions.BytesDictToProp(stream.ReadObject<Dictionary<object, object>>());//ע��Ҫ BytesDictToOrigin
			this.Token = stream.ReadString();
			this.PlayerName = stream.ReadString();
			this.PlayerProperties = DictionaryExtensions.BytesDictToProp(stream.ReadObject<Dictionary<object, object>>());//!!!
		}

		public override void ExecuteRequestAsync(object session, Action<ICommand> onDone) {
			throw new NotImplementedException();
		}
	}

	public class CreateRoomInRoomServerReply : CommandBase {
		public JoinRoomResult Result;
		public GameRoom Room;
		public RoomPlayer SelfPlayer;
		public List<object> OtherPlayers;

		public override void OnSend(Stream stream) {
			stream.Write((int)this.Result);
			stream.WriteObject(this.Room);
			stream.WriteObject(this.SelfPlayer);
			stream.WriteObject(this.OtherPlayers);
		}

		public override void OnReceive(object session, Stream stream) {
			this.Result = (JoinRoomResult)stream.ReadInt();
			this.Room = stream.ReadObject<GameRoom>();
			this.SelfPlayer = stream.ReadObject<RoomPlayer>();
			this.OtherPlayers = stream.ReadObject<List<object>>();
		}

		public override void OnExecute(object session) {
		}
	}
}