using UnityEngine;

namespace Game {
	/// <summary>
	/// Buff base.
	/// </summary>

	public class BuffBase {
		public BikeBase bike;
		public bool isAffect;
		public float time;
		public float timer;

		public virtual void Update() {
			if (isAffect) {
				UpdateWhenAffect();
				timer += Time.deltaTime;
				if (time >= 0f && timer > time) {
					time = 0f;
					Stop();
				}
			}
		}

		public virtual void FixedUpdate() {
			if (isAffect) {
				FixedUpdateWhenAffect();
			}
		}

		public virtual bool Start(float buffTime) {
			// if (character.characterHealth.isAlive)
			//			{
			this.isAffect = true;
			this.time = buffTime;
			this.timer = 0f;
			return true;
			//			}
			//			return false;
		}

		public virtual void UpdateWhenAffect() {

		}

		public virtual void FixedUpdateWhenAffect() {

		}

		public void Stop() {
			isAffect = false;
			OnBuffStop();
		}

		public virtual void OnBuffStop() {
		}

		public virtual void OnRemoveDebuff() {
		}

	}
}
