// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.IO;
using CommandModel;
using Protocol;
using SilentOrbit.ProtocolBuffers;

namespace ClientEngine {

	public class CommandClient : TcpPacketClient, ICommandHandler, ICommandSession {

		public ICommandHandle Handle { get; set; }

		public Action<SessionCommandTuple> OnCommand { get; set; }
		private CommandExecuteMode _commandExecuteMode;

		private Pool<MemoryStream> _commandStreamPool = new Pool<MemoryStream>(1, () => new MemoryStream());

		public CommandClient(CommandExecuteMode commandExecuteMode) {
			this.Handle = new CommandHandle();
			this._commandExecuteMode = commandExecuteMode;
		}

		protected override void OnPacket(Stream stream) {
			try {
				ICommand command;
				command = this.Handle.OnReceiveCommand(this, stream);
				if (command.ShowDebugLog) {
					this.Log.Debug("Recieve Command " + command.Key);
				}
				switch (_commandExecuteMode) {
					case CommandExecuteMode.Direct:
						command.OnExecute(this);
						break;
					case CommandExecuteMode.Manual:
						this.OnCommand(new SessionCommandTuple(this, command));
						break;
				}
			} catch (Exception e) {
				Log.Error(null, e);
				OnError(e);
			}
		}

		public void Send(ICommand command) {
			MemoryStream stream = _commandStreamPool.Pop();
			stream.Clean();
			command.WriteToStream(stream);
			ArraySegment<byte> finalSend;
			stream.TryGetBuffer(out finalSend);
			this.Send(finalSend.Array, finalSend.Offset, finalSend.Count);
			_commandStreamPool.Push(stream);
		}

	}
}