using System;
using GameClient;
using GameUI;
using Joystick;
using UnityEngine;

namespace Game {
	public class BikeInputRemoteContol : BikeInputKeyboard {
		public static BikeInputRemoteContol Ins { get; private set; }
		private float lastLeftTime = 0;
		private float lastRightTime = 0;

		public override void Awake() {
			base.Awake();
			Ins = this;
		}

		protected virtual void OnDestroy() {
			Ins = null;
		}

		void Update() {
			KeyboardUpdate();
			float nowTime = Time.timeSinceLevelLoad;
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				if (nowTime - this.lastLeftTime < 0.3f) {
					bikeInput.OnDrift();
					this.lastLeftTime = 0;
				} else {
					this.lastLeftTime = nowTime;
				}
				bikeInput.Horizontal = -1;
			} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
				if (nowTime - this.lastRightTime < 0.3f) {
					bikeInput.OnDrift();
					this.lastRightTime = 0;
				} else {
					this.lastRightTime = nowTime;
				}
				bikeInput.Horizontal = 1;
			}

			if (GameModeBase.Ins.State != GameState.Pause) {
				if (Input.GetKeyDown(KeyCode.UpArrow)) {
					bikeInput.OnBoost();
				}
				//				if (Input.GetKeyDown(KeyCode.DownArrow)) {
				//					if (GamePlayBoard.Ins != null && GamePlayBoard.Ins.GameControl.BtnEquipProp != null) {
				//						GamePlayBoard.Ins.GameControl.BtnEquipProp.OnClick();
				//					}
				//				}
				if (JoystickInput.Ins.IsConfirmDown) {
					if (GamePlayBoard.Ins != null) {
						if (GamePlayBoard.Ins.GameControl.BtnGameProp != null && GamePlayBoard.Ins.GameControl.BtnGameProp.PropType != PropType.None) {
							GamePlayBoard.Ins.GameControl.BtnGameProp.OnClick();
						} else if (GamePlayBoard.Ins.GameControl.BtnEquipProp != null) {
							GamePlayBoard.Ins.GameControl.BtnEquipProp.OnClick();
						}
					}
				}
			}
		}


	}
}