using System;
using System.Collections.Generic;
using System.Reflection;
using RoomServerModel;
using UnityEngine;

namespace RoomBasedClient {
	public class RoomView : MonoBehaviour {

		internal class ObjMethodInfo {
			public Type type;
			public MethodInfo MethodInfo;
		}

		public bool DestroyOnPlayerLeave = true;

		internal static Dictionary<string, ObjMethodInfo> RpcMethodInfos = new Dictionary<string, ObjMethodInfo>();
		public GameRoom Room { get; internal set; }

		public RoomPlayer Owner {
			get {
				if (this.OwnerId <= 0) {
					return null;
				} else {
					if (this.Room.Players.ContainsKey(this.OwnerId)) {
						return this.Room.Players[this.OwnerId];
					} else {
						return null;
					}
				}
			}
		}

		public int OwnerId { get; internal set; }
		internal int ViewIdInPlayer { get; set; }

		public int Id {
			get { return this.OwnerId + this.ViewIdInPlayer; }
		}

		public bool IsSceneView {
			get { return this.Owner == null; }
		}

		public bool IsMine {
			get { return this.Owner != null && this.Owner.IsMine; }
		}

		public bool IsControledByMe {
			get { return this.IsMine || (this.IsSceneView && this.Room.MinePlayer.IsMaster); }
		}

		public virtual void OnInstantiate(object[] customData) {
		}

		public void Rpc(BroadcastType broadcastType, string methodName, params object[] objects) {
			if (RoomClient.OfflineMode) {
				if (broadcastType == BroadcastType.All
					|| broadcastType == BroadcastType.AllViaServer
					|| broadcastType == BroadcastType.MasterOnly) {
					InvokeMethod(methodName, objects);
				}
			} else {
				this.Room.MinePlayer.ViewRpc(broadcastType, this.Id, methodName, objects);
			}
		}

		internal void InvokeMethod(string methodName, object[] args) {
			ObjMethodInfo objMethodInfo;
			if (!RpcMethodInfos.TryGetValue(methodName, out objMethodInfo)) {
				foreach (var behaviour in GetComponents<MonoBehaviour>()) {
					var methods = behaviour.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					foreach (var method in methods) {
						if (method.Name == methodName) {
							objMethodInfo = new ObjMethodInfo() {
								type = behaviour.GetType(),
								MethodInfo = method
							};
							RpcMethodInfos.Add(methodName, objMethodInfo);
							break;
						}
					}
				}
			}
			if (objMethodInfo == null) {
				throw new Exception("View Rpc Method : " + methodName + " Not Found");
			}
			objMethodInfo.MethodInfo.Invoke(GetComponent(objMethodInfo.type), args);
		}

		public virtual void SendUpdate(List<object> toSend) {
		}

		public virtual void RecieveUpdate(List<object> recieve) {
		}
	}
}