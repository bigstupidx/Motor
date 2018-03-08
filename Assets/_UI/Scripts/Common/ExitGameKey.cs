using UnityEngine;
using GameClient;

namespace GameUI {
	public class ExitGameKey : MonoBehaviour {
		void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (!Client.IsInited || WaittingTip.Ins != null) {
					return;
				}

				if (StoryGuideBoard.Ins != null) {
					return;
				}
				if (MainBoard.Ins != null && MainBoard.Ins.gameObject.activeSelf && MainBoard.IsShow) {
					if (SDKManager.Instance.IsSupport("exitGame")) {
						SDKManager.Instance.ExitGame();
					} else {
						CommonDialog.Show((LString.GAMEUI_ANDROIDRETURNKEY_UPDATE).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_1).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_3).ToLocalized(), () => {
							Application.Quit();
#if UNITY_EDITOR
							UnityEditor.EditorApplication.isPlaying = false;
#endif
						}, null);
					}

				}
			}


		}
	}


}
