using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandModel;
using RoomBasedClient.Commands;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;

namespace RoomBasedClient {
	public class RoomPlayer {
		internal static Dictionary<string, MethodInfo> CachedMethodInfos = new Dictionary<string, MethodInfo>();

		//因为没有实现直接放在场景中的SceneView，暂时用这个顶替一下
		public static Dictionary<string, Action<object[]>> RegRpcs = new Dictionary<string, Action<object[]>>();

		public int Id;
		public string Name;
		public bool IsMaster;
		public Dictionary<object, object> CustomProperties = new Dictionary<object, object>();

		public GameRoom Room { get; set; }
		public ToRoomClient Session { get; set; }
		public bool IsMine { get { return Session != null; } }

		public Dictionary<int, RoomView> Views = new Dictionary<int, RoomView>();

		public override string ToString() {
			string prop = this.CustomProperties == null ? "null" : this.CustomProperties.ToStringFull();
			return string.Format("Player Id={0} Name={1} IsMaster={2} View={3} Prop={4}", this.Id, this.Name, this.IsMaster, this.Views.Count, prop);
		}

		internal void Serilize(Stream stream) {
			stream.Write(this.Id);
			stream.Write(this.Name);
			stream.Write(this.IsMaster);
			if (this.CustomProperties == null) {
				stream.WriteObject(this.CustomProperties);
			} else {
				lock (CustomProperties) {
					stream.WriteObject(DictionaryExtensions.PropToBytesDict(this.CustomProperties));
				}
			}
		}

		internal void Deserilize(Stream stream) {
			this.Id = stream.ReadInt();
			this.Name = stream.ReadString();
			this.IsMaster = stream.ReadBool();
			this.CustomProperties = DictionaryExtensions.BytesDictToProp(stream.ReadObject<Dictionary<object, object>>(), false);
		}

		private int GetNewViewIdInPlayer() {
			for (int i = 1; i < GameRoom.MaxViewId; i++) {
				if (!this.Views.ContainsKey(this.Id + i)) {
					return i;
				}
			}
			throw new Exception("No More Player View Id Can allocate For Player " + this.Id);
		}

		public void SetProp(Dictionary<object, object> changedPart) {
			SetPropInternal(changedPart);
			Room.MinePlayer.Session.Send(new SetPlayerProp() {
				PlayerId = this.Id,
				ChangedPart = changedPart
			});
		}

		internal void SetPropInternal(Dictionary<object, object> changedPart) {
			if (this.CustomProperties == null) {
				this.CustomProperties = new Dictionary<object, object>();
			}
			this.CustomProperties.AddOrUpdateOrRemove(changedPart);
			this.Room.MinePlayer.Session.RoomClient.Callback.OnPlayerPropertiesChanged(this, changedPart);
		}

		private void Broadcast(BroadcastType type, ICommand command) {
			if (!IsMine) {
				throw new Exception("只能在自己的玩家中调用RPC");
			}
			bool broadcastToSelfLocally = false;
			bool broadcastToServer = true;
			if (type == BroadcastType.All) {
				broadcastToSelfLocally = true;
			} else if (type == BroadcastType.MasterOnly) {
				if (IsMaster) {
					broadcastToSelfLocally = true;
					broadcastToServer = false;
				}
			}
			if (broadcastToSelfLocally) {
				command.OnExecute(this.Session);
			}
			if (broadcastToServer) {
				this.Session.Send(command);
			}
		}

		internal void Rpc(BroadcastType type, string methodName, params object[] args) {
			PlayerRpc command = new PlayerRpc() {
				BroadcastType = type,
				MethodName = methodName,
				PlayerId = Id,
				Args = args
			};
			Broadcast(type, command);
		}

		internal void ViewRpc(BroadcastType type, int viewId, string methodName, params object[] args) {
			ViewRpc command = new ViewRpc() {
				BroadcastType = type,
				ViewId = viewId,
				MethodName = methodName,
				Args = args
			};
			Broadcast(type, command);
		}

		internal GameObject Instantiate(string prefabPath, Vector3 pos, Quaternion rot,
			object[] customData) {
			int viewIdInPlayer = GetNewViewIdInPlayer();
			GameObject ret = _Instantiate(viewIdInPlayer, prefabPath, pos, rot, customData);
			Rpc(BroadcastType.Others, "_Instantiate", viewIdInPlayer, prefabPath, pos, rot, customData);
			return ret;
		}

		private GameObject _Instantiate(int viewIdInPlayer, string prefabPath, Vector3 pos, Quaternion rot,
			object[] customData) {
			GameObject ins = RoomClient.InstantiateFunc(prefabPath, pos, rot);
			RoomView view = ins.GetComponent<RoomView>();
			if (view == null) {
				throw new Exception(prefabPath + " don't hava a RoomPlayerView component");
			}
			view.Room = this.Room;
			view.OwnerId = this.Id;
			view.ViewIdInPlayer = viewIdInPlayer;
			this.Views.Add(view.Id, view);
			this.Room.Views.Add(view.Id, view);
			view.OnInstantiate(customData);
			return ins;
		}

		internal GameObject InstantiateSceneView(string prefabPath, Vector3 pos, Quaternion rot,
			object[] customData) {
			int newSceneViewId = Room.GetNewSceneViewId();
			GameObject ret = _InsSceneView(newSceneViewId, prefabPath, pos, rot, customData);
			Rpc(BroadcastType.Others, "_InsSceneView", newSceneViewId, prefabPath, pos, rot, customData);
			return ret;
		}

		private GameObject _InsSceneView(int viewId, string prefabPath, Vector3 pos, Quaternion rot,
			object[] customData) {
			GameObject ins = RoomClient.InstantiateFunc(prefabPath, pos, rot);
			RoomView view = ins.GetComponent<RoomView>();
			if (view == null) {
				throw new Exception(prefabPath + " don't hava a RoomPlayerView component");
			}
			view.Room = this.Room;
			view.OwnerId = 0;//scene view
			view.ViewIdInPlayer = viewId;
			this.Room.Views.Add(view.Id, view);
			view.OnInstantiate(customData);
			return ins;
		}

		internal void DestroyView(RoomView view) {
			Rpc(BroadcastType.All, "_DestroyView", view.Id);
		}

		private void _DestroyView(int viewId) {
			var view = this.Room.Views[viewId];
			if (view.Owner != null) {
				view.Owner.Views.Remove(viewId);
			}
			this.Room.Views.Remove(viewId);
			UnityEngine.Object.Destroy(view.gameObject);
		}


		public void CallRegRpc(BroadcastType broadcastType, string methodName, params object[] args) {
			Rpc(broadcastType, "_CallRegRpc", methodName, args);
		}

		private void _CallRegRpc(string methodName, object[] args) {
			Action<object[]> method = RegRpcs[methodName];
			method(args);
		}

		internal void OnLeave() {
			//玩家离开的时候移除所有view
			foreach (var viewKV in this.Views) {
				if (viewKV.Value != null) {
					if (viewKV.Value.DestroyOnPlayerLeave) {
						UnityEngine.Object.Destroy(viewKV.Value.gameObject);
					}
				}
				this.Room.Views.Remove(viewKV.Key);
			}
			this.Views.Clear();
			this.Room.Players.Remove(this.Id);
			this.Room.Session.RoomClient.Callback.OnPlayerLeave(this);
		}

		internal void InvokeMethod(string methodName, object[] args) {
			MethodInfo methodInfo;
			if (!CachedMethodInfos.TryGetValue(methodName, out methodInfo)) {
				var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (var method in methods) {
					if (method.Name == methodName) {
						methodInfo = method;
						CachedMethodInfos.Add(methodName, methodInfo);
						break;
					}
				}
			}
			if (methodInfo == null) {
				throw new Exception("Player Rpc Method : " + methodName + " Not Found");
			}
			methodInfo.Invoke(this, args);
		}
	}
}