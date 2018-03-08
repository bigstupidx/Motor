using System;
using System.Collections;
using GameClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using XPlugin.Update;
using XUI;

namespace GameUI {
	public class GameUIInterface : Singleton<GameUIInterface> {

		/// <summary>
		/// 从游戏中退回界面
		/// </summary>
		public void ExitGame(UIGroup backGroup, Action onDone = null) {
			StartCoroutine(ExitGameToUI(backGroup, onDone));
		}

		private IEnumerator ExitGameToUI(UIGroup backGroup, Action onDone = null) {
			UResources.ReqScene(DataDef.MenuBG_Scene);
			var op = SceneManager.LoadSceneAsync(DataDef.MenuBG_Scene);
			while (!op.isDone) {
				LoadingBoard.Show((LString.GAMEUI_GAMEUIINTERFACE_LOADSCENE).ToLocalized(), op.progress, true);
				yield return op;
			}
			yield return null;
			LoadingBoard.Show((LString.GAMEUI_GAMEUIINTERFACE_LOADSCENE).ToLocalized(), 1, true);
			yield return null;
			SfxManager.Ins.SetMusicVolume(Client.User.UserInfo.Setting.MusicVolume);
			ModMenu.Ins.BackTo(backGroup);
			if (onDone != null) {
				onDone();
			}
		}

	}
}
