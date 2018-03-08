using System;
using System.Collections.Generic;
using ClientEngine;
using RoomServerModel;
using UnityEngine;

namespace RoomBasedClient {
	public class RoomClientCallback : MonoBehaviour, IRoomClientCallback {
		public virtual void OnConnected() {
		}

		public virtual void OnConnectFail(Exception e) {
		}

		public virtual void OnDisconnected(CloseCause cause) {
		}

		public virtual void OnCreateRoomFailed(CreateRoomResult result) {
		}

		public virtual void OnJoinRoomFailed(JoinRoomResult result) {
		}

		public virtual void OnCreatedRoom() {
		}

		public virtual void OnJoinedLobby() {
		}

		public virtual void OnLeftLobby() {
		}

		public virtual void OnRoomListUpdate() {
		}

		public virtual void OnJoinedRoom() {
		}

		public virtual void OnBeenKickRoom() {
		}

		public virtual void OnLeftRoom() {
		}

		public virtual void OnPlayerJoin(RoomPlayer newPlayer) {
		}

		public virtual void OnPlayerLeave(RoomPlayer otherPlayer) {
		}

		public virtual void OnMasterClientSwitched(RoomPlayer newMasterClient) {
		}

		public virtual void OnRoomSelfPropertiesChanged(Dictionary<object, object> changedPart) {
		}

		public virtual void OnRoomLobbyPropertiesChanged(Dictionary<object, object> changedPart) {
		}

		public virtual void OnRoomOtherPropertiesChanged(Dictionary<object, object> changedPart) {
		}

		public virtual void OnPlayerPropertiesChanged(RoomPlayer player, Dictionary<object, object> changedPart) {
		}

		public virtual void OnPingUpdate(int ping) {
		}
	}
}