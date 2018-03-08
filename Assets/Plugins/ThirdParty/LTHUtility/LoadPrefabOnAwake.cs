using UnityEngine;
using System.Collections;
using XPlugin.Update;


namespace LTHUtility {


	public class LoadPrefabOnAwake : MonoBehaviour {
		[SerializeField]
		private bool _asParent;

		[Reorderable]
		public GameObject[] Prefabs;
		[Reorderable]
		public string[] PrefabsInRes;

		void Awake() {
			if (Prefabs != null) {
				foreach (var prefab in Prefabs) {
					GameObject g = Instantiate<GameObject>(prefab);
					if (_asParent) {
						g.transform.SetParent(transform, false);
					}
				}
			}
			if (PrefabsInRes != null) {
				foreach (var s in PrefabsInRes) {
					GameObject g = Instantiate<GameObject>(UResources.Load<GameObject>(s));
					if (_asParent) {
						g.transform.SetParent(transform, false);
					}
				}
			}
		}


	}
}
