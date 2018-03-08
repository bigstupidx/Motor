using System;
using System.Collections.Generic;
using System.Threading;
using ClientEngine;
using Protocol;
using RoomBasedClient.Commands;
using RoomServerModel;
using UnityEngine;
using Ping = RoomBasedClient.Commands.Ping;

namespace RoomBasedClient {

	public class RoomClient : IDisposable {

		public static bool OfflineMode { get; set; }

		public static Func<string, Vector3, Quaternion, GameObject> InstantiateFunc = (path, position, rotation) => {
			return (GameObject)UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path), position, rotation);
		};

		public string _lobbyHost;
		public int _lobbyPort;

		public ToLobbyClient ToLobbyClient;
		public ToRoomClient ToRoomClient;

		public bool BeenKick = false;

		public float ViewUpdateInterval = 0.05f;
		private float _viewUpdateTimer = 0f;

		public IRoomClientCallback Callback;
		private Thread _mainThread;

		public bool IsMainThread {
			get { return Thread.CurrentThread == this._mainThread; }
		}

		private Queue<Action> _mainThreadJobs = new Queue<Action>();
		public void RunInMainThread(Action action) {
			if (IsMainThread) {
				action();
			} else {
				lock (_mainThreadJobs) {
					this._mainThreadJobs.Enqueue(action);
				}
			}
		}

		public RoomClient(string lobbyHost, int lobbyPort, IRoomClientCallback callback) {
			this._mainThread = Thread.CurrentThread;//初始化只能在主线程做
			this._lobbyHost = lobbyHost;
			this._lobbyPort = lobbyPort;
			this.Callback = callback;
			this.ToLobbyClient = new ToLobbyClient(this);
			this.ToLobbyClient.OnCommand = tuple => {
				RunInMainThread(tuple.Execute);
			};
			this.ToRoomClient = new ToRoomClient(this);
			this.ToRoomClient.OnCommand = tuple => {
				RunInMainThread(tuple.Execute);
			};

		}

		public void Dispose() {
			if (this.ToLobbyClient.Connected) {
				this.ToLobbyClient.Close(CloseCause.ClosedByClientLogic);
			}
			if (this.ToRoomClient.Connected) {
				this.ToRoomClient.Close(CloseCause.ClosedByClientLogic);
			}
		}

		public void LeaveRoom() {
			if (this.ToLobbyClient.Connected) {
				this.ToLobbyClient.Log.Error("Already in Lobby Server");
				return;
			}
			if (!this.ToRoomClient.Connected) {
				this.ToLobbyClient.Log.Error("Not in Room Server");
				return;
			}

			this.ToLobbyClient.Connect(this._lobbyHost, this._lobbyPort);
		}


		public void Update() {
			this.ToLobbyClient.Update(Time.deltaTime);
			this.ToRoomClient.Update(Time.deltaTime);

			while (this._mainThreadJobs.Count > 0) {
				Action action;
				lock (this._mainThreadJobs) {
					action = this._mainThreadJobs.Dequeue();
				}
				action();
			}
			SendViewUpdate();
		}

		Pool<List<object>> updateObjectsListPool = new Pool<List<object>>(10, () => new List<object>());
		ViewUpdate cachedViewUpdateCmd = new ViewUpdate() {
			ViewAndObjects = new Dictionary<object, object>()
		};
		void SendViewUpdate() {
			if (this.ToRoomClient == null) {
				return;
			}
			if (!this.ToRoomClient.InRoom) {
				return;
			}
			this._viewUpdateTimer += Time.deltaTime;
			if (this._viewUpdateTimer > this.ViewUpdateInterval) {
				this._viewUpdateTimer = 0;
				this.cachedViewUpdateCmd.ViewAndObjects.Clear();

				foreach (var view in this.ToRoomClient.Room.Views.Values) {
					if (view == null) {
						continue;
					}
					List<object> objects = updateObjectsListPool.Pop();
					objects.Clear();
					view.SendUpdate(objects);
					if (objects.Count > 0) {
						this.cachedViewUpdateCmd.ViewAndObjects.Add(view.Id, objects);
					}
				}
				if (this.cachedViewUpdateCmd.ViewAndObjects.Count > 0) {
					this.ToRoomClient.Send(this.cachedViewUpdateCmd);
				}
				this.updateObjectsListPool.RecycleAll();
			}
		}

		public void ConnectLobby() {
			this.BeenKick = false;
			if (this.ToLobbyClient.Connected) {
				this.ToLobbyClient.Log.Error("Already Connect to Lobby Server");
				return;
			}
			ToLobbyClient.Connect(this._lobbyHost, this._lobbyPort);
		}

		public void CreateRoom(string name, int maxPlayerCount, string password,
			Dictionary<object, object> lobbyCustomProperties,
			Dictionary<object, object> otherCustomProperties,
			string playerName, Dictionary<object, object> playerProperties) {
			this.ToLobbyClient.CreateRoom(name, maxPlayerCount, password,
				lobbyCustomProperties, otherCustomProperties, playerName, playerProperties);
		}

		public void JoinRoom(string name, string password, string playerName, Dictionary<object, object> playerProperties) {
			this.ToLobbyClient.JoinRoom(name, password, playerName, playerProperties);
		}

		public GameObject Instantiate(string prefabPath, Vector3 pos, Quaternion rot, object[] customData) {
			if (OfflineMode) {
				var ret = InstantiateFunc(prefabPath, pos, rot);
				RoomView view = ret.GetComponent<RoomView>();
				view.OnInstantiate(customData);
				return ret;
			} else {
				if (!this.ToRoomClient.InRoom) {
					throw new Exception("Instantiate Operate must in room");
				} else {
					return this.ToRoomClient.Room.MinePlayer.Instantiate(prefabPath, pos, rot, customData);
				}
			}
		}

		public GameObject InstantiateSceneView(string prefabPath, Vector3 pos, Quaternion rot,
			object[] customData) {
			if (OfflineMode) {
				var ret = InstantiateFunc(prefabPath, pos, rot);
				RoomView view = ret.GetComponent<RoomView>();
				view.OnInstantiate(customData);
				return ret;
			} else {
				if (!this.ToRoomClient.InRoom) {
					throw new Exception("Instantiate Operate must in room");
				} else {
					return this.ToRoomClient.Room.MinePlayer.InstantiateSceneView(prefabPath, pos, rot, customData);
				}
			}
		}

		public void DestoryView(RoomView view) {
			if (OfflineMode) {
				UnityEngine.Object.Destroy(view.gameObject);
			} else {
				if (!this.ToRoomClient.InRoom) {
					throw new Exception("DestoryView must in room");
				} else {
					this.ToRoomClient.Room.MinePlayer.DestroyView(view);
				}
			}
		}

	}
}