namespace CommandModel {
	public struct SessionCommandTuple {
		public object Session;
		public ICommand Command;

		public SessionCommandTuple(object session, ICommand command) {
			this.Session = session;
			this.Command = command;
		}

		public void Execute() {
			if (this.Session != null) {
				this.Command.OnExecute(this.Session);
			}
		}
	}
}