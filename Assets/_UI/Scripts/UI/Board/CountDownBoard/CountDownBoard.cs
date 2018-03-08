using System.Collections;
using Game;
using UnityEngine;
using GameClient;

namespace GameUI {
	public class CountDownBoard : Singleton<CountDownBoard> {


		public const string UIPrefabName = "UI/Board/CountDownBoard";

		public static void Show() {
			ModMenu.Ins.Cover(new string[]{ UIPrefabName }, "CountDownBoard", true);
		}


		public CanvasGroup Dark;
	}
}
