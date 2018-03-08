using System;
using System.Collections.Generic;
using ClientEngine;
using RoomServerModel;

namespace RoomBasedClient {
	public interface IRoomClientCallback {

		void OnConnected();

		void OnConnectFail(Exception e);

		void OnDisconnected(CloseCause cause);

		void OnCreateRoomFailed(CreateRoomResult result);

		void OnJoinRoomFailed(JoinRoomResult result);

		void OnCreatedRoom();

		void OnJoinedLobby();

		void OnLeftLobby();

		void OnRoomListUpdate();

		void OnJoinedRoom();

		void OnBeenKickRoom();

		void OnLeftRoom();

		void OnPlayerJoin(RoomPlayer newPlayer);

		void OnPlayerLeave(RoomPlayer otherPlayer);

		void OnMasterClientSwitched(RoomPlayer newMasterClient);

		void OnRoomSelfPropertiesChanged(Dictionary<object, object> changedPart);

		void OnRoomLobbyPropertiesChanged(Dictionary<object, object> changedPart);

		void OnRoomOtherPropertiesChanged(Dictionary<object, object> changedPart);

		void OnPlayerPropertiesChanged(RoomPlayer player, Dictionary<object, object> changedPart);

		void OnPingUpdate(int ping);




	}
}