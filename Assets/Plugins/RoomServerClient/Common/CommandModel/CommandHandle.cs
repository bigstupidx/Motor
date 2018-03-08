using System;
using System.Collections.Generic;
using System.IO;
using CommandModel.AssemblyCommandLoader;
using Protocol;

namespace CommandModel {

	public class CommandHandle : ICommandHandle {
		public Dictionary<object, Type> Commands { get; set; }

		public CommandHandle() {
			Commands = new AssemblyCommandLoader<ICommand>().LoadCommand();
		}

		public ICommand GetCommand(object key) {
			return (ICommand)Activator.CreateInstance(this.Commands[key]);
		}

		public ICommand OnReceiveCommand(object session, Stream stream) {
			object commandKey = SerializableTypeRegister.Deserialize(stream);
			var command = GetCommand(commandKey);
			command.OnReceive(session, stream);
			return command;
		}

	}
}