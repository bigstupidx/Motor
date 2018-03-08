using System;
using System.Collections.Generic;
using System.IO;
using ClientEngine;
using CommandModel;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class FetchRoomsRequest : RpcRequest {
		public int Offset;
		public int Limit;

		public override void OnSendRpc(Stream stream) {
			stream.Write(this.Offset);
			stream.Write(this.Limit);
		}

		public override void OnReceiveRpc(object session, Stream stream) {
			this.Offset = stream.ReadInt();
			this.Limit = stream.ReadInt();
		}

		public override void ExecuteRequestAsync(object session, Action<ICommand> onDone) {
			throw new NotImplementedException();
		}
	}

	public class FetchRoomsReply : CommandBase {
		public List<object> Rooms = new List<object>();
		public override void OnSend(Stream stream) {
			stream.WriteObject(this.Rooms);
		}

		public override void OnReceive(object session, Stream stream) {
			this.Rooms = stream.ReadObject<List<object>>();
		}

		public override void OnExecute(object session) {
		}
	}
}