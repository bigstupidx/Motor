////
//// TrialsRotateContrler.cs
////
//// Author:
//// [WeiHuajian]
////
//// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
//
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace GameUI{
//	public class TrialsRotateController : RotateController
//	{
//		public UIEffectDialog effect;
//
//		private Vector3 lastMousePos = Vector3.zero;	
//		private bool isPressed = false;
//
//		void OnEnable() {
//			isPressed = false;
//		}
//
//		void Update() {
//			if (UI.Dialog.CurrentDialog == UIType.None && !effect.IsPlaying)
//			{
//				if (Input.GetMouseButtonDown(0))
//				{
//					isPressed = true;
//					OnButton(true);
//					lastMousePos = Input.mousePosition;
//				}
//				if (Input.GetMouseButtonUp(0))
//				{
//					isPressed = false;
//					OnButton(false);
//				}
//				if (isPressed && !doNext)
//				{
//					OnMove(Input.mousePosition - lastMousePos);
//					lastMousePos = Input.mousePosition;
//				}
//			}
//		}
//
//		public void ShowRight() {
//			ShowNext(true);
//		}
//
//		public void ShowLeft() {
//			ShowNext(false);
//		}
//	}
//}
