using Logging;

namespace ClientEngine {
	public interface IPacketSession {

		ILog Log { get; set; }

		bool Connected { get; }

		void Connect(string host, int port);

		void Close(CloseCause cause);

	}
}