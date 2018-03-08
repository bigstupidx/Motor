//
// GUIWindow.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;

namespace GameClient
{
	public class GUIWindow
	{
		public int WinID = -1;
		public string WinTitle;
		public Rect WinRect;
		public bool Dragable;
		public bool IsModel;
		public Action WinFunc;
		public Action OnWinOpen;
		public Action OnWinClose;

		public GUIWindow(string title, float width, float height, bool dragable = true)
		{
			WinTitle = title;
			WinRect = new Rect(0, 0, width, height);
			Dragable = dragable;
		}

		public void SetSize(float width, float height)
		{
			Vector2 center = WinRect.center;
			WinRect.size = new Vector2(width, height);
			WinRect.center = center;
		}
	}
}
