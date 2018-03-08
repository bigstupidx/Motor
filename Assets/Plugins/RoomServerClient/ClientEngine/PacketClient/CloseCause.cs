namespace ClientEngine {
	public enum CloseCause {
		/// <summary>Connection could not be established.
		/// Possible cause: Local server not running.</summary>
		ExceptionOnConnect,

		/// <summary>Timeout disconnect by server (which decided an ACK was missing for too long).</summary>
		ClosedByServerTimeout,

		/// <summary>Server actively disconnected this client.
		/// Possible cause: Server's send buffer full (too much data for client).</summary>
		ClosedByServerLogic,

		/// <summary>Some exception caused the connection to close.</summary>
		UnknownException,

		/// <summary>Timeout disconnect by client (which decided an ACK was missing for too long).</summary>
		ClosedByClientTimeout,

		ClosedByClientLogic,

		/// <summary>Exception in the receive-loop.
		/// Possible cause: Socket failure.</summary>
		ReceiveException,
		SendException,
	}
}