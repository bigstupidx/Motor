using System.Collections.Generic;
using Joystick.UGUI;
using UnityEngine;
using UnityEngine.UI;

//自动筛选聚焦容器子节点的聚焦项并绑上聚焦脚本
//：排除背景的带碰撞体的子元素
namespace Joystick {
	public class FocusableItemAutoer : MonoBehaviour {

#if UNITY_EDITOR
		//可用的聚焦项列表
		public List<FocusItemBase> items = new List<FocusItemBase>();
		//排除掉的子元素
		public List<Selectable> bannedList = new List<Selectable>();

		[ContextMenu("AutoAddInChild")]
		void AutoAddInChild() {
			this.items.Clear();
			foreach (var t in transform.GetComponentsInChildren<Transform>(true)) {
				var c = t.GetComponent<Selectable>();
				if (c != null) {
					if (!this.bannedList.Contains(c)) {
						//如果已经有FocusableItem及其子类脚本就不用重复添加
						if (t.gameObject.GetComponent<FocusItemBase>()) {
							continue;
						}
						//添加FocusableItem脚本
						var i = t.gameObject.AddComponent<FocusItemPointerClick>();
						//添加聚焦项
						this.items.Add(i);
					}
				}
			}
		}

		//		[ContextMenu("自动添加取消响应组件")]
		//		void AutoAddCancel() {
		//			foreach (var t in transform.GetComponentsInChildren<Transform>(true)) {
		//				var c = t.GetComponent<BoxCollider>();
		//				if (c != null) {
		//					if (t.gameObject.name == "CloseButton" || t.gameObject.name == "PauseButton") {
		//						if (t.gameObject.GetComponent<CancelKeyHandler>()) {
		//							continue;
		//						}
		//						//添加FocusableItem脚本
		//						var i = t.gameObject.AddComponent<CancelKeyHandler>();
		//						Debug.Log("添加取消响应组件:" + t.gameObject);
		//					}
		//				}
		//			}
		//		}

		[ContextMenu("AutoDeleteInChild")]
		void AutoDeleteInChild() {
			foreach (var t in transform.GetComponentsInChildren<Transform>(true)) {
				var c = t.GetComponent<FocusItemBase>();
				if (c != null) {
					DestroyImmediate(c);
				}
				this.items.Clear();
			}

		}
#endif
	}
}