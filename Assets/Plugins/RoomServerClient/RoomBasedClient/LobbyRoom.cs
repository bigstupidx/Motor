using System.Collections.Generic;
using System.IO;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient {
	public class LobbyRoom {
		public string Token;

		private Dictionary<object, object> _selfProperties = new Dictionary<object, object>();
		public Dictionary<object, object> LobbyCustomProperties = new Dictionary<object, object>();

		public string Name {
			get { return (string)this._selfProperties["Name"]; }
		}

		public int PlayerCount {
			get { return (int)this._selfProperties["PlayerCount"]; }
		}

		public int MaxPlayerCount {
			get { return (int)this._selfProperties["MaxPlayerCount"]; }
		}

		public string Password {
			get { return (string)this._selfProperties["Password"]; }
		}

		public bool IsOpen {
			get { return (bool)this._selfProperties["IsOpen"]; }
		}

		public bool IsRoomFull {
			get { return this.PlayerCount >= this.MaxPlayerCount; }
		}

		public void Serilize(Stream stream) {
			lock (this._selfProperties) {
				stream.WriteObject(this._selfProperties);
			}
			if (this.LobbyCustomProperties == null) {
				stream.WriteObject(null);
			} else {
				lock (this.LobbyCustomProperties) {
					stream.WriteObject(this.LobbyCustomProperties);
				}
			}
		}

		public void Deserilize(Stream stream) {
			this._selfProperties = stream.ReadObject<Dictionary<object, object>>();
			this.LobbyCustomProperties = stream.ReadObject<Dictionary<object, object>>();
		}

		public override string ToString() {
			string lobbyProp = this.LobbyCustomProperties == null ? "null" : this.LobbyCustomProperties.ToStringFull();
			return string.Format("LobbyRoom Name={0} Player={1}/{2} Password={3} IsOpen={4} LobbyProp={5}", Name, PlayerCount, MaxPlayerCount,
				Password, IsOpen, lobbyProp);
		}
	}
}