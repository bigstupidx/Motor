using System.IO;
using SilentOrbit.ProtocolBuffers;

namespace RoomServerModel {
	public class HostPort : ISerilizable {
		public string Host;
		public int Port;

		public HostPort() {
		}

		public HostPort(string host, int port) {
			this.Host = host;
			this.Port = port;
		}

		public void Serilize(Stream stream) {
			stream.Write(this.Host);
			stream.Write(this.Port);
		}

		public void Deserilize(Stream stream) {
			this.Host = stream.ReadString();
			this.Port = stream.ReadInt();
		}
	}
}