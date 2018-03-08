using System.IO;
using ClientEngine;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class PlayerLeaveRoom : CommandBase<ToRoomClient> {
		public int PlayerId;
		public override void OnSend(Stream stream) {
			stream.Write(this.PlayerId);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.PlayerId = stream.ReadInt();
		}

		public override void OnExecute(ToRoomClient session) {
			session.OnPlayerLeave(this.PlayerId);
		}
	}
}