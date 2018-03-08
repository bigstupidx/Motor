using System.Collections;
using GameClient;
using GameUI;
using UnityEngine;

namespace Game {
	public class BikeInputSomatosensory : BikeInputKeyboard {

		public static readonly float TurnMul = 1.8f;
		public static readonly float TurnThresholdMin = 0.2f;
		public static readonly float TurnThresholdMax = 0.9f;
		public static readonly float BoostThreshold = 0.3f;

		public static BikeInputSomatosensory Ins { get; private set; }

		IEnumerator Start() {
			while (Orbbec.OrbbecSensingManager.instance == null) {
				yield return null;
			}
//			Orbbec.OrbbecSensingManager.instance.leftAtkAction = () => {
//				bikeInput.OnAttack(true);
//			};
//			Orbbec.OrbbecSensingManager.instance.rightAtkAction = () => {
//				bikeInput.OnAttack(false);
//			};

			Orbbec.OrbbecSensingManager.instance.leftHandRaiseAction = () => {
				if (GamePlayBoard.Ins != null && GamePlayBoard.Ins.GameControl.BtnEquipProp != null) {
					GamePlayBoard.Ins.GameControl.BtnEquipProp.OnClick();
				}
			};
			Orbbec.OrbbecSensingManager.instance.rightHandRaiseAction = () => {
				if (GamePlayBoard.Ins != null && GamePlayBoard.Ins.GameControl.BtnGameProp != null) {
					GamePlayBoard.Ins.GameControl.BtnGameProp.OnClick();
				}
			};
		}

		public override void Awake() {
			base.Awake();
			Ins = this;
		}

		protected virtual void OnDestroy() {
			Ins = null;
		}

		void Update() {
			KeyboardUpdate();
			//orbec
			if (Orbbec.OrbbecSensingManager.instance != null) {
				bikeInput.Horizontal = Orbbec.OrbbecSensingManager.instance.horizontalValue * TurnMul;
				if (Mathf.Abs(bikeInput.Horizontal) < TurnThresholdMin) {
					bikeInput.Horizontal = 0f;
				}
				//			bikeInput.Horizontal = Mathf.Pow(bikeInput.Horizontal, 2);
				if (Mathf.Abs(bikeInput.Horizontal) > TurnThresholdMax && !bikeControl.Drifting) {
					this.bikeInput.OnDrift();
				}
				if (Orbbec.OrbbecSensingManager.instance.verticalValue > BoostThreshold) {
					bikeInput.OnBoost();
				}
			}
		}


	}
}