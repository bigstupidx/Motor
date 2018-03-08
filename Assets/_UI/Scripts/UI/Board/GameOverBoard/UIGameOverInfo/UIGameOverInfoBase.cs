using Game;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class UIGameOverInfoBase : UIStackBehaviour {

		public override void OnUIEnterStack() {
			Init();
		}

		public MaskableGraphic[] NeedGrey;
		public GameObject WinBtns;

		protected virtual void Init() {
			if (GameModeBase.Ins.GetWinOrFail()) {
				Win();
			} else {
				Loss();
			}
		}

		protected virtual void Win() {
			foreach (var maskableGraphic in NeedGrey) {
				maskableGraphic.SetGreyMaterail(false);
			}
			WinBtns.SetActive(true);
		}

		protected virtual void Loss() {
			foreach (var maskableGraphic in NeedGrey) {
				maskableGraphic.SetGreyMaterail(true);
			}
			WinBtns.SetActive(false);
		}

	}

}

