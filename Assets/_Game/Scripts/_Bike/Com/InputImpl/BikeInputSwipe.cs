using System;
using GameClient;
using UnityEngine;

namespace Game {
	public class BikeInputSwipe : BikeInputKeyboard {
//		public static BikeInputSwipe Ins { get; private set; }
//
//		public override void Awake() {
//			base.Awake();
//			Ins = this;
//		}
//
//		void OnEnable() {
//			EasyTouch.On_TouchDown += EasyTouchOnOnTouchDown;
//			EasyTouch.On_SwipeEnd += EasyTouchOnOnSwipeEnd;
//		}
//
//	    void OnDisable()
//		{
//			EasyTouch.On_TouchDown -= EasyTouchOnOnTouchDown;
//			EasyTouch.On_SwipeEnd -= EasyTouchOnOnSwipeEnd;
//		}
//
//		private void EasyTouchOnOnSwipeEnd(Gesture gesture) {
//			if (gesture.swipeLength < 50) {//滑动距离小于一定值认为不是滑动
//				return;
//			}
//			if (gesture.actionTime > 3f) {//滑动时间超过多少秒认为不是滑动
//				return;
//			}
//
//			if (gesture.swipe == EasyTouch.SwipeDirection.Up
//				|| gesture.swipe == EasyTouch.SwipeDirection.UpLeft
//				|| gesture.swipe == EasyTouch.SwipeDirection.UpRight) {
//				bikeInput.OnBoost();
//			} else if (gesture.swipe == EasyTouch.SwipeDirection.Down
//				 || gesture.swipe == EasyTouch.SwipeDirection.DownLeft
//				 || gesture.swipe == EasyTouch.SwipeDirection.DownRight) {
//				bikeInput.OnDrift();
//			}
//		}
//
//		private void EasyTouchOnOnTouchDown(Gesture gesture) {
//			float screenWidth = Screen.width;
//			if (gesture.position.x < screenWidth / 2f) { //点击了左边
//				bikeInput.Horizontal = -1;
//			} else {
//				bikeInput.Horizontal = 1;
//			}
//		}
//
//		protected virtual void OnDestroy() {
//			Ins = null;
//			EasyTouch.On_TouchDown -= EasyTouchOnOnTouchDown;
//			EasyTouch.On_SwipeEnd -= EasyTouchOnOnSwipeEnd;
//		}
//
//		void Update() {
//			bikeInput.Horizontal = 0f;
//			KeyboardUpdate();
//			bikeInput.Vertical = Input.GetAxis("Vertical") >= 0 ? 1f : -1f;
//		}


	}
}