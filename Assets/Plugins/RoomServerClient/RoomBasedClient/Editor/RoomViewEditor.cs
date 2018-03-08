using UnityEditor;
using UnityEngine;

namespace RoomBasedClient {
	[CustomEditor(typeof(RoomView), true)]
	public class RoomViewEditor : Editor {

		void OnEnable() {

		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			RoomView view = (RoomView)this.target;
			bool isProjectPrefab = EditorUtility.IsPersistent(view.gameObject);
			if (view.Owner == null) {
				GUILayout.Label("ViewId: Not Set");
			} else {
				GUILayout.Label("ViewId:" + view.Id);
			}

		}
	}
}
