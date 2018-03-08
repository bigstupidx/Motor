using System;
using System.IO;
using ClientEngine;
using CommandModel;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class JoinRoomInLobbyRequest : RpcRequest {
		public string Name;
		public string Password;

		public override void OnSendRpc(Stream stream) {
			stream.Write(this.Name);
			stream.Write(this.Password);
		}

		public override void OnReceiveRpc(object session, Stream stream) {
			this.Name = stream.ReadString();
			this.Password = stream.ReadString();
		}

		public override void ExecuteRequestAsync(object session, Action<ICommand> onDone) {
			throw new NotImplementedException();
		}
	}

	public class JoinRoomInLobbyReply:CommandBase {
		public JoinRoomResult Result;
		public HostPort RoomServerHostPort;
		public override void OnSend(Stream stream) {
			stream.Write((int)this.Result);
			stream.WriteObject(this.RoomServerHostPort);
		}

		public override void OnReceive(object session, Stream stream) {
			this.Result = (JoinRoomResult)stream.ReadInt();
			this.RoomServerHostPort = stream.ReadObject<HostPort>();
		}

		public override void OnExecute(object session) {
		}
	}
}