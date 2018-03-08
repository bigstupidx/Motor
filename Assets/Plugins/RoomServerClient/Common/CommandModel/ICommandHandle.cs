using System.IO;

namespace CommandModel {
	public interface ICommandHandle {
		ICommand GetCommand(object key);

		ICommand OnReceiveCommand(object session, Stream stream);

	}
}
