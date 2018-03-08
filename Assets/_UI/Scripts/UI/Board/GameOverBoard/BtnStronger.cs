using GameClient;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI {
	public class BtnStronger : MonoBehaviour, IPointerClickHandler {
		public GoToSee SeeWhat;

		public void OnPointerClick(PointerEventData eventData) {
			switch (Client.Game.MatchInfo.MatchMode) {
				case MatchMode.Guide:
					GameUIInterface.Ins.ExitGame(MainBoard.GroupIns);
					break;
				case MatchMode.Championship:
					GameUIInterface.Ins.ExitGame(ChampionshipDetailBoard.GroupIns, Open);
					break;
				case MatchMode.Online:
				case MatchMode.OnlineRandom:
					GameUIInterface.Ins.ExitGame(MainBoard.GroupIns, Open);
					break;
				case MatchMode.Normal:
					if (SeeWhat == GoToSee.None) {
						GameUIInterface.Ins.ExitGame(LevelChooseBoard.GroupIns);
					} else {
						GameUIInterface.Ins.ExitGame(LevelChooseBoard.GroupIns, Open);
					}
					break;
			}
		}

		public void Open() {
			switch (SeeWhat) {
				case GoToSee.GarageBoard:
					GarageBoard.Show(Client.User.UserInfo.ChoosedBikeID);
					break;
				case GoToSee.HeroBoard:
					HeroBoard.Show(Client.User.UserInfo.ChoosedHeroID);
					break;
			}
		}
	}

}

