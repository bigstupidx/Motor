using UnityEngine;

namespace Game {
	public class BikeInputGravitySwipe : BikeInputKeyboard {
		public static BikeInputGravitySwipe Ins { get; private set; }
		[System.NonSerialized]
		public bool TurnLeft;
		[System.NonSerialized]
		public bool TurnRight;

		public override void Awake() {
			base.Awake();
			Ins = this;
		}

		//		void OnEnable() {
		//			EasyTouch.On_SwipeEnd += EasyTouchOnOnSwipeEnd;
		//		}
		//
		//		void OnDisable() {
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
		protected virtual void OnDestroy() {
			Ins = null;
			//			EasyTouch.On_SwipeEnd -= EasyTouchOnOnSwipeEnd;
		}

		void Update() {
			bikeInput.Horizontal = 0f;
			KeyboardUpdate();
			bikeInput.Vertical = Input.GetAxis("Vertical") >= 0 ? 1f : -1f;
			bikeInput.Horizontal = Input.acceleration.x * 1.7f;
		}


	}
}