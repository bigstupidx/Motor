using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Joystick.UGUI {
	/// <summary>
	/// 用于修正焦点滑动
	/// </summary>
	public class FocusScrollViewFix : MonoBehaviour {

		public RectTransform mRect;

		void Awake() {
			if (mRect == null) {
				mRect = this.GetComponent<RectTransform>();
			}
		}
		/// <summary>
		/// 水平方向左边界
		/// </summary>
		public float LeftX {
			get {
				return mRect.localPosition.x - mRect.sizeDelta.x * mRect.pivot.x;
			}
		}
		/// <summary>
		/// 水平方向右边界
		/// </summary>
		public float RightX {
			get {
				return mRect.localPosition.x + mRect.sizeDelta.x * (1.0f - mRect.pivot.x);
			}
		}

		/// <summary>
		/// 竖直方向底边界
		/// </summary>
		public float BottomY {
			get {
				return mRect.localPosition.y - mRect.sizeDelta.y * mRect.pivot.y;
			}
		}

		/// <summary>
		/// 竖直方向上边界
		/// </summary>
		public float TopY {
			get {
				return mRect.localPosition.y + mRect.sizeDelta.y * (1.0f - mRect.pivot.y);
			}
		}
	}

}
