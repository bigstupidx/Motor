using System.IO;
using CommandModel;
using Protocol;

namespace ClientEngine {
	public abstract class CommandBase : ICommand {

		public virtual object Key { get { return GetType().Name; } }

		public virtual bool ShowDebugLog {
			get { return true; }
		}

		public void WriteToStream(Stream stream) {
			SerializableTypeRegister.Serialize(Key, stream);
			OnSend(stream);
		}

		public abstract void OnSend(Stream stream);

		public abstract void OnReceive(object session, Stream stream);

		public abstract void OnExecute(object session);

	}

	public abstract class CommandBase<TSession> : CommandBase {
		public override void OnReceive(object session, Stream stream) {
			OnReceive((TSession)session, stream);
		}

		public abstract void OnReceive(TSession session, Stream stream);


		public override void OnExecute(object session) {
			OnExecute((TSession)session);
		}

		public abstract void OnExecute(TSession session);
	}
}