namespace CommandModel {
	public interface ICommandHandler {
		ICommandHandle Handle { get; }
	}
}