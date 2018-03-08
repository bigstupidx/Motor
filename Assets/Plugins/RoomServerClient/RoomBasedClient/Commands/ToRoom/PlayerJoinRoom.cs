using System.Collections.Generic;
using System.IO;
using ClientEngine;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class PlayerJoinRoom : CommandBase<ToRoomClient> {
		public RoomPlayer NewPlayer;

		public override void OnSend(Stream stream) {
			stream.WriteObject(this.NewPlayer);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.NewPlayer = stream.ReadObject<RoomPlayer>();
		}

		public override void OnExecute(ToRoomClient session) {
			session.OnPlayerJoin(this.NewPlayer);
		}
	}
}