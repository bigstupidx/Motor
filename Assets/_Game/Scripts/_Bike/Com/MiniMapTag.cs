using UnityEngine;
namespace Game {
	public class MiniMapTag : MonoBehaviour {
		[System.NonSerialized]
		public Transform tagInstance;

		private float y;
		void Start() {
			if (MiniMapCamera.Ins == null) {
				return;
			}
			GameObject tag;
			if (!gameObject.CompareTag(Tags.Ins.Player)) {
				tag = Resources.Load("MiniMap_Tag_Ai") as GameObject;
				y = -52f;
			} else {
				tag = Resources.Load("MiniMap_Tag_Player") as GameObject;
				y = -50f;
			}
			GameObject g = Instantiate<GameObject>(tag);
			g.transform.SetParent(MiniMapCamera.Ins.transform.parent);
			if (gameObject.CompareTag(Tags.Ins.Player)) {
				MiniMapCamera.Ins.PlayerTag = g;
			}
			tagInstance = g.transform;
		}

		void Update() {
			if (tagInstance != null && this.tagInstance.gameObject.activeInHierarchy) {
				tagInstance.position = new Vector3(transform.position.x, y, transform.position.z);
				tagInstance.SetEulerAngleY(transform.eulerAngles.y);
			} else {
				enabled = false;
			}
		}
	}
}
