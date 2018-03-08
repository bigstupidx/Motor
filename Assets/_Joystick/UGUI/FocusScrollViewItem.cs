using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Joystick.UGUI {
	/// <summary>
	/// 聚焦滑动列表物件：
	/// 放置在滑动列表物体的最外层，用于收集物体大小及位置信息，需要配合其他聚焦滑动列表物价使用
	/// </summary>
	public class FocusScrollViewItem : MonoBehaviour {

		public RectTransform mRect, mViewRect;
		public ScrollRect mView;
		public FocusScrollViewFix mViewFix;

		public float offsetX = 0.0f;
		public float offsetY = 0.0f;
		private Vector3 targetPos = Vector3.zero;

		void Awake() {
			if (mRect == null) {
				mRect = GetComponent<RectTransform>();
			}
			IsCanMove = false;
		}

		void Start() {
			if (mView == null) {
				mView = GetComponentInParent<ScrollRect>();
				mViewRect = mView.GetComponent<RectTransform>();
			}
			if (mViewFix == null) {
				mViewFix = GetComponentInParent<FocusScrollViewFix>();
			}
		}

		void Update() {
			if (!IsCanMove) {
				return;
			}
			if (mView != null && mView.content != null) {
				mView.content.localPosition = Vector3.Lerp(mView.content.localPosition, targetPos, 8 * Time.deltaTime);
			}
		}

		/// <summary>
		/// 水平方向左边界
		/// </summary>
		private float LeftX {
			get {
				float leftX = mRect.localPosition.x - mRect.sizeDelta.x * mRect.pivot.x;
				if (mViewFix != null) {
					leftX += mViewFix.LeftX;
				}
				return leftX;
			}
		}

		/// <summary>
		/// 水平方向右边界
		/// </summary>
		private float RightX {
			get {
				float rightX = mRect.localPosition.x + mRect.sizeDelta.x * (1.0f - mRect.pivot.x);
				if (mViewFix != null) {
					rightX += mViewFix.LeftX;
				}
				return rightX;
			}
		}

		/// <summary>
		/// 竖直方向底边界
		/// </summary>
		private float BottomY {
			get {
				float bottomY = mRect.localPosition.y - mRect.sizeDelta.y * mRect.pivot.y;
				if (mViewFix != null) {
					bottomY += mViewFix.TopY;
				}
				return bottomY;
			}
		}

		/// <summary>
		/// 竖直方向上边界
		/// </summary>
		private float TopY {
			get {
				float topY = mRect.localPosition.y + mRect.sizeDelta.y * (1.0f - mRect.pivot.y);
				if (mViewFix != null) {
					topY += mViewFix.TopY;
				}
				return topY;
			}
		}
		/// <summary>
		/// 是否可以移动
		/// </summary>
		public bool IsCanMove { get; set; }
		/// <summary>
		/// 更新位置
		/// </summary>
		public void UpdatePos() {
			if (mView != null && mRect != null && mView.content != null && mViewRect != null) {
				offsetX = 0.0f;
				offsetY = 0.0f;
				if (mView.horizontal) {
					if (-mView.content.localPosition.x - mViewRect.rect.width * mView.content.pivot.x > LeftX) {
						offsetX = LeftX + mView.content.localPosition.x + mViewRect.rect.width * mView.content.pivot.x;
					} else if (-mView.content.localPosition.x + mViewRect.rect.width * (1.0f - mView.content.pivot.x) < RightX) {
						offsetX = RightX + mView.content.localPosition.x - mViewRect.rect.width * (1.0f - mView.content.pivot.x);
					}
					offsetX = -offsetX;
				}
				if (mView.vertical) {
					if (mView.content.localPosition.y - mViewRect.rect.height * (1.0f - mView.content.pivot.y) > -TopY) {
						offsetY = TopY + mView.content.localPosition.y - mViewRect.rect.height * (1.0f - mView.content.pivot.y);
					} else if (mView.content.localPosition.y + mViewRect.rect.height * mView.content.pivot.y < -BottomY) {
						offsetY = BottomY + mView.content.localPosition.y + mViewRect.rect.height * mView.content.pivot.y;
					}
					offsetY = -offsetY;
				}
				targetPos = mView.content.localPosition + new Vector3(offsetX, offsetY, 0.0f);
				IsCanMove = true;
			}
		}

	}
}