using System.Collections.Generic;
using System.IO;
using ClientEngine;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class SetRoomLobbyProp : CommandBase<ToRoomClient> {
		public Dictionary<object, object> ChangedPart;

		public override void OnSend(Stream stream) {
			stream.WriteObject(this.ChangedPart);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.ChangedPart = stream.ReadObject<Dictionary<object, object>>();
		}

		public override void OnExecute(ToRoomClient session) {
			session.Room.SetLobbyPropInternal(this.ChangedPart);
		}
	}
}