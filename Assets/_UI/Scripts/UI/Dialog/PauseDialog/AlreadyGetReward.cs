using UnityEngine;
using Game;
using UnityEngine.UI;

namespace GameUI {
	public class AlreadyGetReward : MonoBehaviour {
		public int Index;
		public Image Icon;
		public Text Amount;

		private void OnEnable() {
			Amount.text = GameModeBase.Ins.GetTaskReward(Index).ToString();
		}
	}
}
