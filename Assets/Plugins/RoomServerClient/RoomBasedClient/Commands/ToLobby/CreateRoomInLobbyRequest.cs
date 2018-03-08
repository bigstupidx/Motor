using System;
using System.Collections.Generic;
using System.IO;
using ClientEngine;
using CommandModel;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class CreateRoomInLobbyRequest : RpcRequest {
		public string Name;
		public int MaxPlayerCount;
		public string Password;
		public Dictionary<object, object> LobbyCustomProperties;

		public override void OnSendRpc(Stream stream) {
			stream.Write(this.Name);
			stream.Write(this.MaxPlayerCount);
			stream.Write(this.Password);
			stream.WriteObject(this.LobbyCustomProperties);
		}

		public override void OnReceiveRpc(object session, Stream stream) {
			this.Name = stream.ReadString();
			this.MaxPlayerCount = stream.ReadInt();
			this.Password = stream.ReadString();
			this.LobbyCustomProperties = stream.ReadObject<Dictionary<object, object>>();
		}

		public override void ExecuteRequestAsync(object session, Action<ICommand> onDone) {
			throw new NotImplementedException();
		}
	}

	public class CreateRoomInLobbyReply : CommandBase {
		public CreateRoomResult Result;
		public string Name;
		public string Token;
		public HostPort RoomServerHostPort;

		public override void OnSend(Stream stream) {
			stream.Write((int)this.Result);
			stream.Write(this.Name);
			stream.Write(this.Token);
			stream.WriteObject(this.RoomServerHostPort);
		}

		public override void OnReceive(object session, Stream stream) {
			this.Result = (CreateRoomResult)stream.ReadInt();
			this.Name = stream.ReadString();
			this.Token = stream.ReadString();
			this.RoomServerHostPort = stream.ReadObject<HostPort>();
		}

		public override void OnExecute(object session) {
		}
	}
}