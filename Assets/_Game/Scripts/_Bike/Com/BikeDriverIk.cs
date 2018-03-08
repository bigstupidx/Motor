using UnityEngine;

namespace Game {
	public class BikeDriverIk : MonoBehaviour {
		public BikeControl bikeControl { get; set; }
		public Animator Animator { get; set; }

		public float LeftHandIK { get; set; }
		public float RightHandIK { get; set; }

		public void Init(BikeBase bike, Animator animator) {
			this.bikeControl = bike.bikeControl;
			this.Animator = animator;

			LeftHandIK = 1f;
			RightHandIK = 1f;
		}


		void OnAnimatorIK(int layerIndex) {

			if (!bikeControl.info.IsPlayer) {
				if (DeviceLevel.Score < 20f) {
					return;
				}
			}

			this.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, LeftHandIK);
			this.Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, LeftHandIK);
			this.Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, RightHandIK);
			this.Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, RightHandIK);
//			if (bikeControl.info.IsPlayer) {
				this.Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, RightHandIK);
				this.Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, RightHandIK);

				if (bikeControl.Speed < 20f) {
					this.Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
					this.Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
				} else {
					this.Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, LeftHandIK);
					this.Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, LeftHandIK);
				}
//			}


			var setting = this.bikeControl.bikeSetting;

			this.Animator.SetIKPosition(AvatarIKGoal.LeftHand, setting.LeftHandIK.position);
			this.Animator.SetIKRotation(AvatarIKGoal.LeftHand, setting.LeftHandIK.rotation);

			if (!bikeControl.Backward) {
				this.Animator.SetIKPosition(AvatarIKGoal.RightHand, setting.RightHandIK.position);
				this.Animator.SetIKRotation(AvatarIKGoal.RightHand, setting.RightHandIK.rotation);

//				if (bikeControl.info.IsPlayer) {
					this.Animator.SetIKPosition(AvatarIKGoal.RightFoot, setting.RightFootIK.position);
					this.Animator.SetIKRotation(AvatarIKGoal.RightFoot, setting.RightFootIK.rotation);
					if (bikeControl.Speed > 10.0f) {
						this.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, setting.LeftFootIK.position);
						this.Animator.SetIKRotation(AvatarIKGoal.LeftFoot, setting.LeftFootIK.rotation);
					}
//				}
			}
		}

	}
}