using System;
using System.IO;
using CommandModel;
using SilentOrbit.ProtocolBuffers;
using Protocol;

namespace ClientEngine {
	public class RpcReply : CommandBase {
		public RpcRequest Request { get; set; }
		public ICommand ReplyCommand { get; set; }

		public override void OnSend(Stream stream) {
			ProtocolParser.WriteString(stream, Request.Id);
			SerializableTypeRegister.Serialize(ReplyCommand.Key, stream);
			ReplyCommand.OnSend(stream);
		}

		public override void OnReceive(object session, Stream stream) {
			IRpcSession rpcSession = (IRpcSession)session;
			string id = ProtocolParser.ReadString(stream);
			this.Request = rpcSession.GetRpcRequest(id);
			ICommandHandler handler = (ICommandHandler)session;
			ReplyCommand = handler.Handle.OnReceiveCommand(session, stream);
			ReplyCommand.OnExecute(session);
		}

		public override void OnExecute(object session) {
			IRpcSession rpcSession = (IRpcSession)session;
			lock (this.Request) {
#if COREFX
				if (!Request.Tcs.Task.IsCompleted) {
					Request.Error = RpcError.Success;
					rpcSession.RemoveRpcRequest(Request.Id);
					Request.Tcs.SetResult(ReplyCommand);
				}
#else
				if (Request.OnDone != null) {
					Request.Error = RpcError.Success;
					rpcSession.RemoveRpcRequest(Request.Id);
					try {
						Request.OnDone(ReplyCommand);
					} catch (Exception e) {
						var rpcClient = (RpcClient)session;
						rpcClient.Log.Error(e);
					}
				}
#endif
			}
		}
	}
}