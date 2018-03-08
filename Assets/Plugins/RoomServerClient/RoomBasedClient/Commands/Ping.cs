using System.IO;
using ClientEngine;

namespace RoomBasedClient.Commands {
	public class Ping : CommandBase {
		public override bool ShowDebugLog { get { return false; } }
		public override object Key {
			get { return "Pi"; }
		}

		public override void OnSend(Stream stream) {
		}

		public override void OnReceive(object session, Stream stream) {
		}

		public override void OnExecute(object session) {
		}
	}

	public class Pong : CommandBase {
		public override bool ShowDebugLog { get { return false; } }
		public override object Key {
			get { return "Po"; }
		}

		public override void OnSend(Stream stream) {
		}

		public override void OnReceive(object session, Stream stream) {
		}

		public override void OnExecute(object session) {
			RoomClient client;
			if (session is ToRoomClient) {
				var toRoomClient = session as ToRoomClient;
				toRoomClient.OnRevicePong();
			} else if (session is ToLobbyClient) {
				ToLobbyClient toLobbyClient = session as ToLobbyClient;
				toLobbyClient.OnRevicePong();
			}
		}
	}
}