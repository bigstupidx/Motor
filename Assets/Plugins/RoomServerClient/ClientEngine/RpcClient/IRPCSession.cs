namespace ClientEngine {

	public interface IRpcSession {
		RpcRequest GetRpcRequest(string id);
		void RemoveRpcRequest(string id);
	}
}