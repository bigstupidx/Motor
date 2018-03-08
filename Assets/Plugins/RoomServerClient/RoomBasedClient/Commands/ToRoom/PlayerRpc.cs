using System;
using System.IO;
using System.Reflection;
using ClientEngine;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;
using Protocol;

namespace RoomBasedClient.Commands {
	public class PlayerRpc : CommandBase<ToRoomClient> {

		public BroadcastType BroadcastType;
		public int PlayerId;
		public string MethodName;
		public object[] Args;

		public override object Key {
			get { return "PlayerRpc"; }
		}

		public override void OnSend(Stream stream) {
			ProtocolParser.WriteInt32(stream, (int)this.BroadcastType);
			ProtocolParser.WriteInt32(stream, this.PlayerId);
			ProtocolParser.WriteString(stream, this.MethodName);
			SerializableTypeRegister.Serialize(this.Args, stream);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.BroadcastType = (BroadcastType)ProtocolParser.ReadInt32(stream);
			this.PlayerId = ProtocolParser.ReadInt32(stream);
			this.MethodName = ProtocolParser.ReadString(stream);
			this.Args = (object[])SerializableTypeRegister.Deserialize(stream);
		}

		public override void OnExecute(ToRoomClient session) {
			var player = session.Room.Players[this.PlayerId];
			player.InvokeMethod(this.MethodName, this.Args);
		}
	}
}