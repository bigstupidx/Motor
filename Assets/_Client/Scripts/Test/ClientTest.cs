//
// ClientTest.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using XPlugin.Data.Json;

namespace GameClient {
	public class ClientTest : MonoBehaviour {
		public bool LoadUI = false;

		void Awake() {
//			if (LoadUI && FindObjectOfType<MainNGUIController>() == null) {
//				GameObject uiPrefab = Resources.Load("UI/UI") as GameObject;
//				GameObject ui = Instantiate(uiPrefab) as GameObject;
//				ui.name = uiPrefab.name;
//				DontDestroyOnLoad(ui);
//			}
			DontDestroyOnLoad(gameObject);
		}

		protected GUIWindow Window = new GUIWindow((LString.GAMECLIENT_CLIENTTEST_AWAKE).ToLocalized(), 220, 330);

		void WinFunc() {
			GUILayout.Label((LString.GAMECLIENT_CLIENTTEST_WINFUNC).ToLocalized());
//			DeviceLevel.Score = (int)GUILayout.HorizontalSlider(DeviceLevel.Score, 0, 100);
			
			GUILayout.Space(10);


			GUILayout.FlexibleSpace();
			if (GUILayout.Button((LString.GAMECLIENT_CLIENTTEST_WINFUNC_1).ToLocalized())) {
				ClientGUI.Ins.CloseWindow(this);
			}
		}
	}
}
