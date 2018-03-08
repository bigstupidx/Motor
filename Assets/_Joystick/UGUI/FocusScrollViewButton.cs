using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Joystick.UGUI {

	/// <summary>
	/// 按钮内容的聚焦滚动ScrollView
	/// </summary>
	public class FocusScrollViewButton : FocusItemPointerClick {
		public FocusScrollViewItem mItem;

		void Start() {
			if (mItem == null) {
				mItem = GetComponent<FocusScrollViewItem>();
				if (mItem == null) {
					mItem = GetComponentInParent<FocusScrollViewItem>();
				}
			}
		}

		protected override FocusItemBase Search(Vector3 dir, List<FocusItemBase> list) {
			Vector3 myScreenPos = GetScreenPos();
			float min = float.MaxValue;
			float weightMax = 0;
			//计算的最佳项目
			FocusItemBase ret = null;
			for (int i = 0; i < list.Count; ++i) {
				var item = list[i];
				//跳过自己本身和不活跃的
				if (item == this || !item.IsActive) {
					continue;
				}

				Vector3 navCenter = item.GetScreenPos();
				//目标对象与起点的方向向量
				Vector3 targetDir = navCenter - myScreenPos;
				//单位向量点积
				float dot = Vector3.Dot(dir, targetDir.normalized);
				//大于一定角度度跳过
				if (dot <= 0.2f) {
					continue;
				}

				float mag = targetDir.magnitude;
				if (mag == 0f) {
					Debug.LogError("找到完全重合的点，跳过" + item.name);
					continue;
				}

				float weight = 1f / mag * 10f;
				if (dot > 0.99f) {//如果角度极为一致
								  //判断是否属于同一组物体
					if (this.transform.parent == item.transform.parent) {
						weight += 1000f;
					}
					if (item is FocusScrollViewButton) {
						FocusScrollViewButton btnItem = item as FocusScrollViewButton;
						if (this.mItem.transform.parent == btnItem.mItem.transform.parent) {
							weight += 1000f;
						}
					}
				}
				if (weight > weightMax) {
					weightMax = weight;
					ret = item;
				}
			}
			return ret;
		}

		public override void OnFocused(FocusItemBase lastFocus) {
			base.OnFocused(lastFocus);
			if (mItem != null) {
				mItem.UpdatePos();
			}
		}

		public override void OnLostFocuse(FocusItemBase newFocus) {
			base.OnLostFocuse(newFocus);
			if (mItem != null) {
				mItem.IsCanMove = false;
			}
		}
	}

}