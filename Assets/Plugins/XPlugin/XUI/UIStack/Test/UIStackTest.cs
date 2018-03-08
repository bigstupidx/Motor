using System.Collections.Generic;
using UnityEngine;

namespace XUI {
	public class UIStackTest : MonoBehaviour {

		public UIStack stack;

		public GameObject up;
		public GameObject down;
		public GameObject BGDrak;
		public GameObject BGImage;
		public GameObject ActivityPanel;
		public GameObject HeroListBoard;


		public UIGroup toBack;


		void OnGUI() {
			if (GUILayout.Button("主界面")) {
				this.stack.Overlay(new List<GameObject>() { this.up, this.down });
			}
			if (GUILayout.Button("主界面点击活动")) {
				this.stack.Overlay(new[] { this.BGDrak, this.ActivityPanel });
			}
			if (GUILayout.Button("主界面点击英雄")) {
				this.toBack = this.stack.Cover(new[] { this.BGImage, this.up, this.HeroListBoard }, false);
			}
			if (GUILayout.Button("主界面进入章节选择"))
			{
				this.toBack = this.stack.Cover(new[] { this.BGImage, this.up, this.HeroListBoard }, false);
			}
			if (GUILayout.Button("进入关卡选择"))
			{
				this.stack.Cover(new[] { this.BGImage, this.up, this.ActivityPanel }, false);
			}
			if (GUILayout.Button("loading"))
			{
				this.stack.Cover(new[] { this.BGDrak }, false);
			}
			if (GUILayout.Button("play"))
			{
				this.stack.Cover(new[] { this.BGImage }, false);
			}
			if (GUILayout.Button("返回章节界面"))
			{
				this.stack.BackTo(this.toBack, false);
			}
			if (GUILayout.Button("返回上一个界面")) {
				this.stack.Back(true);
			}
			if (GUILayout.Button("返回英雄界面")) {
				this.stack.BackTo(this.toBack, false);
			}

		}



	}
}