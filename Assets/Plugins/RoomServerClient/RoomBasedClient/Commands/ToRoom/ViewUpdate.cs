using System.Collections.Generic;
using System.IO;
using ClientEngine;
using Protocol;

namespace RoomBasedClient.Commands {
	public class ViewUpdate : CommandBase<ToRoomClient> {
		public Dictionary<object, object> ViewAndObjects;

		public override bool ShowDebugLog {
			get { return false; }
		}

		public override object Key {//频繁创建并调用的命令,手动指定Key
			get { return "VU"; }
		}

		public override void OnSend(Stream stream) {
			SerializableTypeRegister.Serialize(this.ViewAndObjects, stream);
		}

		public override void OnReceive(ToRoomClient session, Stream stream) {
			this.ViewAndObjects = SerializableTypeRegister.Deserialize<Dictionary<object, object>>(stream);
		}

		public override void OnExecute(ToRoomClient session) {
			foreach (var kv in this.ViewAndObjects) {
				int viewId = (int)kv.Key;
				List<object> objs = (List<object>)kv.Value;
				if (session.InRoom) {
					session.Room.Views[viewId].RecieveUpdate(objs);
				}
			}
		}
	}
}