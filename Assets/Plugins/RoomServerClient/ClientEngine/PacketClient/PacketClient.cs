using System;
using System.IO;
using Logging;

namespace ClientEngine {
	public abstract class PacketClient : IPacketSession {

		public ILog Log { get; set; }

		protected PacketClient() {
			this.Log = LogFactory.ForContext(this.GetType());
		}

		public virtual bool Connected { get; protected set; }

		public virtual void Connect(string host, int port) {
		}

		public virtual void Close(CloseCause cause) {
		}

		protected virtual void OnPacket(Stream stream) {
		}

		public virtual void Send(byte[] data, int offset, int count) {
		}

		protected virtual void OnError(Exception e) {
		}

		protected virtual void OnConnected() {
			this.Log.Debug("OnConnected");
		}

		protected virtual void OnConnectFail(Exception e) {
			this.Log.Error("OnConnectFail", e);
		}

		protected virtual void OnClosed(CloseCause cause) {
			this.Log.Debug(GetType().Name+ " OnClosed " + cause);
		}
	}
}