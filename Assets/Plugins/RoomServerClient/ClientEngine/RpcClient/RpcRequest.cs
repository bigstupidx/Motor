using System.IO;
using CommandModel;
using SilentOrbit.ProtocolBuffers;
#if COREFX
using System.Threading.Tasks;
#else
using System;
#endif


namespace ClientEngine {

	public abstract class RpcRequest : CommandBase {

		internal string Id { get; set; }
		public RpcError Error { get; set; }
#if COREFX
		internal TaskCompletionSource<ICommand> Tcs = new TaskCompletionSource<ICommand>();
#else
		internal Action<ICommand> OnDone;
#endif

		public override void OnSend(Stream stream) {
			ProtocolParser.WriteString(stream, Id);
			OnSendRpc(stream);
		}
		public abstract void OnSendRpc(Stream stream);

		public override void OnReceive(object session, Stream stream) {
			this.Id = ProtocolParser.ReadString(stream);
			OnReceiveRpc(session, stream);
		}
		public abstract void OnReceiveRpc(object session, Stream stream);

		public override void OnExecute(object session) {
#if COREFX
			Task.Run(async () => {
				//调用实现，并返回结果
				var replyCommand = await ExecuteRequestAsync(session);
				RpcReply reply = new RpcReply() {
					Request = this,
					ReplyCommand = replyCommand
				};
				ICommandSession commandSession = (ICommandSession)session;
				commandSession.Send(reply);
			});
#else
			ExecuteRequestAsync(session, replyCommand => {
				RpcReply reply = new RpcReply() {
					Request = this,
					ReplyCommand = replyCommand
				};
				ICommandSession commandSession = (ICommandSession)session;
				commandSession.Send(reply);
			});
#endif
		}


#if COREFX
		public abstract Task<ICommand> ExecuteRequestAsync(object session);
#else
		public abstract void ExecuteRequestAsync(object session, Action<ICommand> onDone);
#endif

	}



	public abstract class RpcRequest<TSession> : RpcRequest {
#if COREFX
		public override Task<ICommand> ExecuteRequestAsync(object session) {
			return ExecuteRequestAsync((TSession)session);
		}
#else
		public override void ExecuteRequestAsync(object session, Action<ICommand> onDone) {
			ExecuteRequestAsync((TSession)session, onDone);
		}
#endif

		public override void OnReceiveRpc(object session, Stream stream) {
			OnReceiveRpc((TSession)session, stream);
		}

		public abstract void OnReceiveRpc(TSession session, Stream stream);

#if COREFX
		public abstract Task<ICommand> ExecuteRequestAsync(TSession session);
#else
		public abstract void ExecuteRequestAsync(TSession session, Action<ICommand> onDone);
#endif

	}

}