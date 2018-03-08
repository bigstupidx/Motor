using System.Collections.Generic;
using UnityEngine;

public partial class WaypointManager {

#if UNITY_EDITOR
	[ContextMenu("SetToAiWp")]
	void SetToAiWaypoint() {
		this.wpIconName = "icon_ai";
		this.gizmosColor = Color.green;
		List<WaypointNode> newList = new List<WaypointNode>();
		foreach (var waypointNode in this.WaypointList) {
			newList.Add(CopyComponent<WaypointNodeAi>(waypointNode, waypointNode.gameObject));
			DestroyImmediate(waypointNode);
		}
		this.WaypointList.Clear();
		this.WaypointList = newList;
		this.ReCaculate();
	}

	T CopyComponent<T>(Component original, GameObject destination) where T : Component {
		System.Type originType = original.GetType();
		T copy = destination.AddComponent<T>();
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = originType.GetFields();
		foreach (System.Reflection.FieldInfo field in fields) {
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}
#endif

}
