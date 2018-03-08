using GameClient;
using UnityEngine;

namespace GameUI {
	public class UIGameControlBase : MonoBehaviour {
		public BtnEquipProp BtnEquipProp;
		public BtnGameProp BtnGameProp;

		protected void OnEnable() {
			if (Client.Game.MatchInfo.Data.GameMode == GameMode.EliminationProp ||
				Client.Game.MatchInfo.Data.GameMode == GameMode.RacingProp) {
				BtnGameProp.gameObject.SetActive(true);
			} else {
				BtnGameProp.gameObject.SetActive(false);
			}

		}
	}

}

