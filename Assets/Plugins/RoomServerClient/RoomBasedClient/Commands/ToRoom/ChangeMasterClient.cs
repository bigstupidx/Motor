using System.IO;
using ClientEngine;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class ChangeMasterClient : CommandBase<ToRoomClient> {
		public int NewMasterClientId;
		public override void OnSend(Stream stream) {
			stream.Write(this.NewMasterClientId);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.NewMasterClientId = stream.ReadInt();
		}

		public override void OnExecute(ToRoomClient session) {
			session.ChangeMasterClient(this.NewMasterClientId);
		}
	}
}