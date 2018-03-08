using UnityEngine;

namespace Game {
	public class BuffBaseWithEffect : BuffBase {

		protected GameObject effectObject;

		public Vector3 effectPos = Vector3.zero;

		public override void OnBuffStop() {
			base.OnBuffStop();
			if (effectObject != null) {
				Object.Destroy(effectObject);
			}
		}

		public bool Start(float buffTime, string effect) {
			bool startSuccess = base.Start(buffTime);
			if (startSuccess) {
				if (effectObject != null) {
					Object.Destroy(effectObject);
				}
				GameObject effectGameObject = null;
				if (!string.IsNullOrEmpty(effect)) {
					effectGameObject = GameObjectUtility.LoadAndIns(effect);
					//					BuffEffect buffEffect = effectGameObject.GetComponent<BuffEffect>();
					// GameObjectUtility.SetGameObjectParent(effectGameObject, character.characterSkill.mountPointsDic[buffEffect.mpType], true);
					GameObjectUtility.SetGameObjectParent(effectGameObject, bike.transform, true);
					effectGameObject.transform.position = effectPos;
				}
				this.effectObject = effectGameObject;

				//				if (this.effectObject) {
				//					this.effectObject.transform.SetUniformLocalScale(character.characterBuff.scaleFactor);
				//				}
			}
			return startSuccess;
		}

	}
}
