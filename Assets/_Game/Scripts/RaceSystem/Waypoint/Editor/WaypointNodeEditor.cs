using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(WaypointNode), true)]
[CanEditMultipleObjects]
public class WaypointNodeEditor : Editor {
	private WaypointNode thiz;
	void OnEnable() {
		thiz = (WaypointNode)target;
	}

	private void OnSceneGUI() {
		Handles.BeginGUI();

		GUILayout.Window(0, new Rect(10, 30, 100, 150), id => {
			if (GUILayout.Button("Insert")) {
				Insert();
			}
			if (GUILayout.Button("Delete")) {
				var manager = thiz.Manager;
				manager.WaypointList.Remove(thiz);
				Selection.activeGameObject = thiz.PrePoint.gameObject;
				DestroyImmediate(thiz.gameObject);
				manager.ReCaculate();
			}
			if (GUILayout.Button("As Start"))
			{
				var manager = thiz.Manager;
				if (manager.StartPoint != null)
				{
					manager.StartPoint.Type = WayPointType.Normal;
				}
				thiz.Type = WayPointType.Start;
				manager.StartPoint = thiz;
				var newList = new List<WaypointNode>();
				newList.AddRange(manager.WaypointList.GetRange(thiz.Index, manager.WaypointList.Count - thiz.Index));
				newList.AddRange(manager.WaypointList.GetRange(0, thiz.Index));
				manager.WaypointList = newList;
				manager.ReCaculate();
			}
			if (GUILayout.Button("As Finish"))
			{
				var manager = thiz.Manager;
				if (manager.FinishPoint != null)
				{
					manager.FinishPoint.Type = WayPointType.Normal;
				}
				thiz.Type = WayPointType.Finish;
				manager.FinishPoint = thiz;
			}
			if (GUILayout.Button("Align Surface")) {
				var manager = thiz.Manager;
				foreach (var node in manager.WaypointList)
				{
					node.Position = WaypointMath.AttachRoadPoint(node.Position);
				}
				manager.ReCaculate();
			}
			if (GUILayout.Button("Recaculate")) {
				var manager = thiz.Manager;
				manager.ReCaculate();
			}
		}, "node");


		Handles.EndGUI();
	}

	private void Insert() {
		var manager = thiz.Manager;
		WaypointNode newNode = (WaypointNode)new GameObject().AddComponent(this.thiz.GetType());
		newNode.transform.SetParent(manager.transform, false);
		newNode.transform.position = thiz.transform.position + (thiz.PrePoint.Position - thiz.Position + thiz.transform.forward).normalized * -10;
		newNode.Manager = manager;
		manager.WaypointList.Insert(manager.WaypointList.IndexOf(thiz) + 1, newNode);
		Selection.activeGameObject = newNode.gameObject;
		manager.ReCaculate();
	}


}
