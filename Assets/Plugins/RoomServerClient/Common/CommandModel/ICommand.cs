using System.IO;

namespace CommandModel {

	public interface ICommand {

		bool ShowDebugLog { get; }

		object Key { get; }

		void WriteToStream(Stream stream);

		void OnSend(Stream stream);

		void OnReceive(object session, Stream stream);

		void OnExecute(object session);
	}

}