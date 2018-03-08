//
// UIEffect_LoadOnStart.cs
//
// Author:
// [WeiHuajian]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;

namespace GameUI {
	public class UIEffect_LoadOnStart : MonoBehaviour {
		public GameObject prefab = null;
		[System.NonSerialized]
		public GameObject ins = null;
		public int addSortingOrder = 1;

		protected Canvas canvas;
		protected Renderer[] renderers;
		private int[] originSortOrder;

		protected virtual void Awake() {
			this.ins = Instantiate(this.prefab) as GameObject;
			this.ins.transform.SetParent(this.transform);
			this.ins.transform.localScale = Vector3.one;
			this.ins.transform.localPosition = Vector3.zero;
			this.ins.transform.localEulerAngles = Vector3.zero;
			renderers = this.ins.GetComponentsInChildren<Renderer>();
			this.originSortOrder = new int[this.renderers.Length];
			for (int i = 0; i < this.renderers.Length; i++) {
				this.originSortOrder[i] = this.renderers[i].sortingOrder;
			}
		}

		void LateUpdate() {//TODO 没有找到合适的地方放这个
			this.canvas = this.gameObject.GetComponentInParent<Canvas>();
			if (this.canvas == null) {
				//				Debug.LogError("canvas not found");
				return;
			}
			UpdateCanvas();
		}

		public void UpdateCanvas() {
			int i = 0;
			foreach (var ren in renderers) {
				ren.sortingLayerID = this.canvas.sortingLayerID;
				ren.sortingOrder = this.canvas.sortingOrder + this.addSortingOrder + this.originSortOrder[i];
				i++;
			}
		}

	}
}
