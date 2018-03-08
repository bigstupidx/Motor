//
// GMMenu.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using Game;

namespace GameClient {
	public class GMMenu : MonoBehaviour {
		GMReset gmreset = new GMReset();
		ClientTest clientTest = null;
		string[] btnList;
		object[] modList;
		int selected = -1;

		int lastTouchCount = 0;

		void Start() {
			btnList = new string[] { "关卡", "重置" };
			modList = new object[] { Client.Match, gmreset };
			clientTest = FindObjectOfType<ClientTest>();
		}

		void OnDisable() {
			ClientGUI.Ins.CloseWindow(this);
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.F1)) {
				if (ClientGUI.Ins.IsWindowOpen(this)) {
					ClientGUI.Ins.CloseWindow(this);
				} else {
					ClientGUI.Ins.ShowWindow(this);
				}
			} else if (Input.GetKeyDown(KeyCode.Escape)) {
				ClientGUI.Ins.CloseWindow(this);
			}

			if (lastTouchCount != Input.touchCount) {
				lastTouchCount = Input.touchCount;
				if (lastTouchCount >= 3) {
					if (ClientGUI.Ins.IsWindowOpen(this)) {
						ClientGUI.Ins.CloseWindow(this);
					} else {
						ClientGUI.Ins.ShowWindow(this);
					}
				}
			}

			if (selected >= 0) {
				if (!ClientGUI.Ins.IsWindowOpen(modList[selected])) {
					selected = -1;
				}
			}
		}

		protected GUIWindow Window = new GUIWindow("GM控制台", 0, 0, false);

		void OnWinOpen() {
			Window.WinRect.position = ClientGUI.Ins.OriginPos;
			Window.WinRect.size = new Vector2(90, ClientGUI.Ins.VisibleSize.y);
			selected = -1;
		}

		void OnWinClose() {
			ClientGUI.Ins.ShowGUI = false;
		}

		void WinFunc() {
			float btnH = 30;


			if (!Client.Ins.Inited) {
				GUILayout.Label("Client\n未初始化");

#if CLIENT_GM || UNITY_EDITOR
				if (GUILayout.Button("清除数据", GUILayout.Height(btnH))) {
					GMConsole.ResetAll((bool ret) => {
						Application.Quit();
#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
#endif
					});
				}
#endif
			} else {
#if CLIENT_GM || UNITY_EDITOR
				GUILayout.Space(100);
				int sel = GUILayout.SelectionGrid(selected, btnList, 1, GUILayout.Height(btnH * btnList.Length));
				if (sel != selected) {
					if (selected != -1) {
						ClientGUI.Ins.CloseWindow(modList[selected]);
					}

					if (ClientGUI.Ins.ShowWindow(modList[sel])) {
						selected = sel;
						GUIWindow win = ClientGUI.Ins.GetWindow(modList[selected]);

						win.WinRect.center = new Vector2(ClientGUI.Ins.CenterPos.x + Window.WinRect.width / 2, ClientGUI.Ins.CenterPos.y);

						if (win.WinRect.xMin < Window.WinRect.xMax) {
							win.WinRect.position = new Vector2(Window.WinRect.xMax, win.WinRect.position.y);
						}
					}
				}
#else
				GUILayout.Label("未打开GM功能\n请定义CLIENT_GM！");
#endif
			}

			GUILayout.FlexibleSpace();

			if (GameModeBase.Ins != null && GameModeBase.Ins.State == GameState.Gaming) {
				if (GUILayout.Button("AI", GUILayout.Height(btnH))) {
					BikeManager.Ins.SetBikeAsAi(BikeManager.Ins.CurrentBike);
				}
			}


			if (GUILayout.Button("FPS", GUILayout.Height(btnH))) {
				var fps = FindObjectOfType<LTHUtility.ShowFPS>();
				if (fps != null) {
					fps.enabled = !fps.enabled;
				}
			}

			if (clientTest != null && GUILayout.Button("测试", GUILayout.Height(btnH))) {
				ClientGUI.Ins.CloseAllWindow();
				ClientGUI.Ins.ShowWindow(clientTest);
			}

			//			if (GUILayout.Button("110", GUILayout.Height(btnH))) {
			//				PhotonNetwork.PhotonServerSettings.UseMyServer("192.168.10.110", 5055, null);
			//			}
			//			if (GUILayout.Button("备用81", GUILayout.Height(btnH))) {
			//				PhotonNetwork.PhotonServerSettings.UseMyServer("123.59.139.81", 5055, null);
			//			}
			//			if (GUILayout.Button("备用85", GUILayout.Height(btnH))) {
			//				PhotonNetwork.PhotonServerSettings.UseMyServer("123.59.139.85", 5055, null);
			//			}


			if (GUILayout.Button("关闭", GUILayout.Height(btnH))) {
				ClientGUI.Ins.CloseWindow(this);
			}
		}
	}
}
