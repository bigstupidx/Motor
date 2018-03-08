using CommandModel;

namespace ClientEngine {
	public interface ICommandSession {
		void Send(ICommand command);
	}
}