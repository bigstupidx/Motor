using System.IO;
using ClientEngine;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;
using Protocol;

namespace RoomBasedClient.Commands {
	public class ViewRpc : CommandBase<ToRoomClient> {

		public BroadcastType BroadcastType;
		public int ViewId;
		public string MethodName;
		public object[] Args;

		public override bool ShowDebugLog {
			get { return false; }
		}

		public override object Key {
			get { return "ViewRpc"; }
		}

		public override void OnSend(Stream stream) {
			ProtocolParser.WriteInt32(stream, (int)this.BroadcastType);
			ProtocolParser.WriteInt32(stream, this.ViewId);
			ProtocolParser.WriteString(stream, this.MethodName);
			SerializableTypeRegister.Serialize(this.Args, stream);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.BroadcastType = (BroadcastType)ProtocolParser.ReadInt32(stream);
			this.ViewId = ProtocolParser.ReadInt32(stream);
			this.MethodName = ProtocolParser.ReadString(stream);
			this.Args = (object[])SerializableTypeRegister.Deserialize(stream);
		}

		public override void OnExecute(ToRoomClient session) {
			if (session.InRoom) {
				var view = session.Room.Views[this.ViewId];
				if (view != null) {
					view.InvokeMethod(this.MethodName, this.Args);
				}
			}
		}
	}
}