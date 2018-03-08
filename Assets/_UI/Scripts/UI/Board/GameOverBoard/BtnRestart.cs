using UnityEngine;
using GameClient;
using GameUI;
using UnityEngine.EventSystems;
public class BtnRestart : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData eventData) {
		switch (Client.Game.MatchInfo.MatchMode) {
			case MatchMode.Guide:
				GameUIInterface.Ins.ExitGame(MainBoard.GroupIns);
				break;
			case MatchMode.Championship:
				GameUIInterface.Ins.ExitGame(ChampionshipDetailBoard.GroupIns);
				break;
			case MatchMode.Online:
				if (Lobby.Ins.RoomClient.ToRoomClient.Connected) {
					GameUIInterface.Ins.ExitGame(OnlineRoomBoard.GroupIns);
				} else {
					ChooseOnlineMode.BackAndEnterLobby();
				}
				break;
			case MatchMode.OnlineRandom:
				GameUIInterface.Ins.ExitGame(NetMatchBoard.GroupIns);
				break;
			case MatchMode.Normal:
				GameUIInterface.Ins.ExitGame(LevelChooseBoard.GroupIns);
				break;
		}
	}
}
