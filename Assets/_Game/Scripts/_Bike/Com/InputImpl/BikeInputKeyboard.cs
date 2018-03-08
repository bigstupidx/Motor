using GameClient;
using GameUI;
using UnityEngine;

namespace Game {
	public class BikeInputKeyboard : BikeInputImplBase {

		public void KeyboardUpdate() {
			//#if UNITY_EDITOR || UNITY_STANDALONE
			bikeInput.Horizontal = Input.GetAxis("Horizontal");
			bikeInput.Vertical = Input.GetAxis("Vertical") >= 0 ? 1f : -1f;

			if (Input.GetKeyDown(KeyCode.LeftShift)) {
				bikeInput.OnBoost();
			}
			if (Input.GetKeyDown(KeyCode.RightShift)) {
				bikeInput.OnAttack(null);
			}
			if (Input.GetKeyDown(KeyCode.Space)) {
				this.bikeInput.OnDrift();
			}
			if (Input.GetKeyDown(KeyCode.LeftControl)) {
				//this.bikeProp.Use(PropType.Missile);
				if (GamePlayBoard.Ins != null && GamePlayBoard.Ins.GameControl.BtnEquipProp.gameObject.activeSelf) {
					GamePlayBoard.Ins.GameControl.BtnEquipProp.OnClick();
				}
			}
			if (Input.GetKeyDown(KeyCode.L)) {
				if (bikeAttack.BeLocked) {
					bikeAttack.OnBeLockedCancel();
				} else {
					bikeAttack.OnBeLocked(null, PropType.Missile);
				}
			}
			if (Input.GetKeyDown(KeyCode.F)) {
				this.bikeState.ProcessEvent(BikeFSM.Event.Attack, true);
			}
			//			if (Input.GetKeyDown(KeyCode.Alpha1)) {
			//				this.bikeBuff.buffBlink.Start(5);
			//			}
			//			if (Input.GetKeyDown(KeyCode.Alpha2)) {
			//				this.bikeBuff.buffFloat.Start(5);
			//			}
			//			if (Input.GetKeyDown(KeyCode.Alpha3)) {
			//				this.bikeBuff.buffInvincible.Start(5);
			//			}
			//			if (Input.GetKeyDown(KeyCode.Alpha4)) {
			//				this.bikeBuff.buffVertigo.Start(5);
			//			}
			if (Input.GetKeyDown(KeyCode.RightControl)) {
				if (GamePlayBoard.Ins != null && GamePlayBoard.Ins.GameControl.BtnGameProp.gameObject.activeSelf) {
					GamePlayBoard.Ins.GameControl.BtnGameProp.OnClick();
				}
			}
			if (Input.GetKeyDown(KeyCode.R)) {
				racerInfo.DoReset();
			}
			if (Input.GetKeyDown(KeyCode.T)) {
				BikeManager.Ins.SetBikeAsAi(this);
			}

			if (Input.GetKeyDown(KeyCode.U)) {
				GameModeBase.Ins.FinishGame();
			}

			if (Input.GetKeyDown(KeyCode.C)) {
				var mode = CameraMode.Drift;
				if (BikeCamera.Ins.mode == CameraMode.Drift) {
					mode = CameraMode.Fixed;
				} else {
					mode = CameraMode.Drift;
				}
				BikeCamera.Ins.SetCameraMode(mode);
			}
			//#endif
		}
	}
}