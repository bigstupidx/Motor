using System.Linq;
using UnityEngine;

namespace Game {
	public class Ragdoll : MonoBehaviour {

		public Rigidbody MainRigidbody;
		public Rigidbody[] Rigidbodies;
		public Collider[] Colliders;


#if UNITY_EDITOR
		[ContextMenu("AUTO ASSIGN")]
		public void AutoAssign() {
			this.Rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
			var tmpList = this.Rigidbodies.ToList();
			tmpList.Remove(this.MainRigidbody);
			this.Rigidbodies = tmpList.ToArray();
			this.Colliders = transform.GetComponentsInChildren<Collider>();
		}
#endif

		public void Enable(bool enable) {
			foreach (var c in this.Colliders) {
				c.enabled = enable;
			}
			foreach (var r in this.Rigidbodies) {
				r.isKinematic = !enable;
			}
		}


	}
}