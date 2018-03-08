using System.IO;
using ClientEngine;
using SilentOrbit.ProtocolBuffers;

namespace RoomBasedClient.Commands {
	public class KickPlayer : CommandBase<ToRoomClient> {

		public int PlayerId;

		public override void OnSend(Stream stream) {
			stream.Write(this.PlayerId);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.PlayerId = stream.ReadInt();
		}

		public override void OnExecute(ToRoomClient session) {
		}
	}

	public class BeenKick : CommandBase<ToRoomClient> {
		public override void OnSend(Stream stream) {
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			session.RoomClient.BeenKick = true;
			session.Close(CloseCause.ClosedByClientLogic);//收到此命令后服务器会立即断开连接，服务器断开的消息会先于命令执行，所以在Recieve中就执行客户端关闭
		}

		public override void OnExecute(ToRoomClient session) {
			session.RoomClient.Callback.OnBeenKickRoom();
//			session.Close(CloseCause.ClosedByClientLogic);
		}
	}
}