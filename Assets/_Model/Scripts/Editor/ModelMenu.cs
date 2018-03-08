using UnityEngine;
using System.Collections;
using CruncherPlugin;
using UnityEditor;

public class ModelMenu {

	[MenuItem("Tools/移除减面组件")]
	public static void RemoveCruncher() {
		GameObject prefab = Selection.activeGameObject;
		_RemoveCruncher(prefab.transform);
	}

	static void _RemoveCruncher(Transform transform) {
		CruncherRoot[] root = transform.GetComponentsInChildren<CruncherRoot>();
		if (root != null) {
			foreach (var r in root) {
				Object.DestroyImmediate(r, true);
			}
		}

		SimpleCruncherTarget[] sct = transform.GetComponentsInChildren<SimpleCruncherTarget>();
		if (sct != null) {
			foreach (var s in sct) {
				Object.DestroyImmediate(s, true);
			}
		}

	}

}
