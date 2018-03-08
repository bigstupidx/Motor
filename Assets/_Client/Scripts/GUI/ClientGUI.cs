//
// ClientDebugGUI.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameClient {
	public class ClientGUI : MonoBehaviour {
		public enum ScreenFixMode {
			FixedWidth,
			FixedHeight
		}

		public bool ShowGUI = true;
		public float BlackAlpha = 0.7f;
		public ScreenFixMode ScreenMode = ScreenFixMode.FixedHeight;
		public Vector2 DesignResolution = new Vector2(800, 600);

		public static ClientGUI Ins;
		public Vector2 OriginPos { get; protected set; }
		public Vector2 CenterPos { get; protected set; }
		public Vector2 VisibleSize { get; protected set; }

		float originX, originY;
		Matrix4x4 mat;
		Texture2D blackTex;

		Dictionary<object, GUIWindow> modDic = new Dictionary<object, GUIWindow>();
		Dictionary<int, GUIWindow> idDic = new Dictionary<int, GUIWindow>();
		//		bool nguiDisabled = false;
		int nowWinID = 0;

		void Awake() {
			if (Ins == null) {
				Ins = this;
				DontDestroyOnLoad(gameObject);

				blackTex = new Texture2D(1, 1);
				blackTex.SetPixel(0, 0, new Color(0, 0, 0, 1));
				blackTex.Apply();

				AdjustScreen();
			} else {
				Debug.LogError("ClientDebugGUI has been created mutiple times!");
			}
		}

		void Update() {
			if (ShowGUI) {
				if (modDic.Count == 0) {
					ShowGUI = false;
				}
			} else {
				if (modDic.Count > 0) {
					CloseAllWindow();
				}
			}

			//			if (UI.MainNGUICamera.instance != null) {
			//				if (ShowGUI && !nguiDisabled) {
			//					UICamera uicam = UI.MainNGUICamera.instance.NguiCamera;
			//					uicam.enabled = false;
			//					nguiDisabled = true;
			//				} else if (!ShowGUI && nguiDisabled) {
			//					UICamera uicam = UI.MainNGUICamera.instance.NguiCamera;
			//					uicam.enabled = true;
			//					nguiDisabled = false;
			//				}
			//			}
		}

		void AdjustScreen() {
			if (originX != Screen.width || originY != Screen.height) {
				switch (ScreenMode) {
					case ScreenFixMode.FixedWidth:
						float scaleX = Screen.width / DesignResolution.x;
						float offsetY = (Screen.height / scaleX - DesignResolution.y) / 2;
						OriginPos = new Vector2(-offsetY, 0);
						VisibleSize = new Vector2(Screen.width / scaleX, Screen.height / scaleX);
						CenterPos = OriginPos + VisibleSize / 2;
						mat = Matrix4x4.identity;
						mat *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleX, 1));
						mat *= Matrix4x4.TRS(-OriginPos, Quaternion.identity, new Vector3(1, 1, 1));
						break;
					case ScreenFixMode.FixedHeight:
						float scaleY = Screen.height / DesignResolution.y;
						float offsetX = (Screen.width / scaleY - DesignResolution.x) / 2;
						OriginPos = new Vector2(-offsetX, 0);
						VisibleSize = new Vector2(Screen.width / scaleY, Screen.height / scaleY);
						CenterPos = OriginPos + VisibleSize / 2;
						mat = Matrix4x4.identity;
						mat *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleY, scaleY, 1));
						mat *= Matrix4x4.TRS(-OriginPos, Quaternion.identity, new Vector3(1, 1, 1));
						break;
				}
				originX = Screen.width;
				originY = Screen.height;
			}
		}

		void OnValidate() {
			AdjustScreen();
		}

		void OnGUI() {
			if (!ShowGUI)
				return;

			AdjustScreen();
			GUI.matrix = mat;

			if (BlackAlpha > 0) {
				Rect rect = new Rect();
				rect.position = OriginPos - VisibleSize;
				rect.size = VisibleSize * 3;
				Color color = GUI.color;
				GUI.color = new Color(1, 1, 1, BlackAlpha);
				GUI.DrawTexture(rect, blackTex);
				GUI.color = color;
			}

			bool hasModelWin = false;
			foreach (object mod in modDic.Keys) {
				GUIWindow win = modDic[mod];
				if (win.IsModel && !hasModelWin) {
					win.WinRect = GUI.ModalWindow(win.WinID, win.WinRect, WindowFunc, win.WinTitle);
					hasModelWin = true;
				} else {
					win.WinRect = GUI.Window(win.WinID, win.WinRect, WindowFunc, win.WinTitle);
				}
			}
		}

		public void ChangeDesignResolution(Vector2 resolution) {
			DesignResolution = resolution;
			originX = 0;
			originY = 0;
			AdjustScreen();
		}

		public bool ShowWindow(object module, bool isModel = false) {
			ShowGUI = true;
			GUIWindow win = null;
			modDic.TryGetValue(module, out win);
			if (win == null) {
				FieldInfo fieldInfo = module.GetType().GetField("Window", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (fieldInfo == null || (win = fieldInfo.GetValue(module) as GUIWindow) == null) {
					Debug.LogError("Object [" + module + "] has no Window!");
					return false;
				}

				MethodInfo methodInfo = module.GetType().GetMethod("WinFunc", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (methodInfo != null) {
					win.WinFunc = (Action)Delegate.CreateDelegate(typeof(Action), module, methodInfo);
				}

				if (win.WinFunc == null) {
					Debug.LogError("Object [" + module + "] has no WinFunc!");
					return false;
				}

				methodInfo = module.GetType().GetMethod("OnWinOpen", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (methodInfo != null) {
					win.OnWinOpen = (Action)Delegate.CreateDelegate(typeof(Action), module, methodInfo);
				}

				methodInfo = module.GetType().GetMethod("OnWinClose", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (methodInfo != null) {
					win.OnWinClose = (Action)Delegate.CreateDelegate(typeof(Action), module, methodInfo);
				}

				/*
				while (idDic.ContainsKey(nowWinID)) {
					nowWinID++;
				}
				*/
				win.WinID = nowWinID++;
				win.IsModel = isModel;
				win.WinRect.center = CenterPos;

				if (win.OnWinOpen != null) {
					win.OnWinOpen();
				}

				modDic.Add(module, win);
				idDic.Add(win.WinID, win);
			} else {
				int oldID = win.WinID;
				win.WinID = nowWinID++;
				idDic.Remove(oldID);
				idDic.Add(win.WinID, win);
			}
			return true;
		}

		public bool IsWindowOpen(object module) {
			return modDic.ContainsKey(module);
		}

		public GUIWindow GetWindow(object module) {
			GUIWindow win = null;
			modDic.TryGetValue(module, out win);
			return win;
		}

		public void CloseWindow(object module) {
			GUIWindow win = null;
			modDic.TryGetValue(module, out win);
			if (win != null) {
				int oldID = win.WinID;
				modDic.Remove(module);
				idDic.Remove(win.WinID);
				win.WinID = -1;

				if (oldID >= 0 && win.OnWinClose != null) {
					win.OnWinClose();
				}
			}

			if (modDic.Count == 0) {
				ShowGUI = false;
			}
		}

		public void CloseAllWindow() {
			List<object> keylist = new List<object>(modDic.Keys);
			foreach (object obj in keylist) {
				CloseWindow(obj);
			}
		}

		public void WindowFunc(int windowID) {
			GUIWindow win = null;
			idDic.TryGetValue(windowID, out win);
			if (win != null) {
				if (win.Dragable) {
					GUI.DragWindow(new Rect(0, 0, win.WinRect.width, 20));
				}
				//GUI.DragWindow();
				win.WinFunc();
			}
		}
	}
}

