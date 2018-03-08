//
// GMMenu.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;

namespace GameClient
{
	public class GMReset
	{
#if CLIENT_GM || UNITY_EDITOR
		protected GUIWindow Window = new GUIWindow("重置", 250, 320);

		void WinFunc()
		{
			GUILayout.Space(10);

			if (GUILayout.Button("清除数据")) {
				GMConsole.ResetAll((bool ret) =>
				{
					Application.Quit();
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
				});
			}

//			GUILayout.Space(10);
//
//			float btnW = 110;
//
//			GUILayout.BeginHorizontal();
//			GUILayout.FlexibleSpace();
//			if (GUILayout.Button("重置今日数据", GUILayout.Width(btnW))) {
//				GMConsole.ResetToday();
//			}
//			GUILayout.EndHorizontal();
			
//			GUILayout.BeginHorizontal();
//			if (GUILayout.Button("当日签到", GUILayout.Width(btnW))) {
//				GMConsole.SignInRenewToday();
//			}
//			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("关闭")) {
				ClientGUI.Ins.CloseWindow(this);
			}
		}
#endif
	}
}
