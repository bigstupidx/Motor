using UnityEngine;
using XPlugin.Update;

namespace Game {
	public class MiniMapCamera : Singleton<MiniMapCamera> {
		[HideInInspector]
		public GameObject PlayerTag;

		[HideInInspector]
		public GameObject MiniMapUi;

		public SpriteRenderer Minimap;

		void Start() {
			Minimap.sprite = UResources.Load<Sprite>(Application.loadedLevelName);
		}

		void Update() {
			if (PlayerTag != null) {
				var pos = PlayerTag.transform.position;
				transform.position = new Vector3(pos.x, transform.position.y, pos.z);
				if (MiniMapUi != null) {
					MiniMapUi.transform.SetEulerAngleZ(PlayerTag.transform.eulerAngles.y);
				}
			}
		}
	}
}
