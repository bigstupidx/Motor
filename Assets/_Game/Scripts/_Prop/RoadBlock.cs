using UnityEngine;

namespace Game {
	public class RoadBlock : MonoBehaviour {
		public NormalPrefab Effect;
		public GameObject Renderer;
		public Collider Collider;

		private void OnCollisionEnter(Collision collision) {
			if (collision.gameObject.CompareTag(Tags.Ins.Player) ||
				collision.gameObject.CompareTag(Tags.Ins.Enemy)) {
				var bike = collision.rigidbody.GetComponent<BikeBase>();
				bike.bikeControl.Boosting = false;
				ShowEffect();
				if (bike.bikeControl.Speed > 150) {
					bike.bikeState.Fsm.processEvent(BikeFSM.Event.Crash, BikeDeathType.Crash);
				} else {
					bike.bikeControl.Rigidbody.velocity *= 0.1f;
				}
				this.Collider.enabled = false;
			}
		}

		private void ShowEffect() {
			if (Effect.Prefab != null) {
				GetComponent<BoxCollider>().enabled = false;
				Effect.Spawn(this.transform);
				Renderer.SetActive(false);

				this.DelayInvoke(() => {
					Destroy(gameObject);
				}, 2f);
			}
		}
	}

}
