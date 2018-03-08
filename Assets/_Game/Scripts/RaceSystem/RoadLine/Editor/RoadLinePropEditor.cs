//
// SpawnpointNodeEditor.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Game;
using UnityEditor;

[CustomEditor(typeof(PropSceneBase), true)]
[CanEditMultipleObjects]
public class RoadLinePropEditor : Editor {

	private PropSceneBase thiz;
	void OnEnable() {
		thiz = (PropSceneBase)target;
	}

	private void OnSceneGUI() {
		Handles.BeginGUI();

		GUILayout.Window(0, new Rect(10, 30, 100, 100), id => {
			if (GUILayout.Button("Align Surface 0m")) {
				thiz.transform.position = WaypointMath.AttachRoadPoint(thiz.transform.position) + Vector3.up * 0f;
			}
			if (GUILayout.Button("Align Surface 1m")) {
				thiz.transform.position = WaypointMath.AttachRoadPoint(thiz.transform.position) + Vector3.up * 1f;
			}
			if (GUILayout.Button("Align Surface 2m"))
			{
				thiz.transform.position = WaypointMath.AttachRoadPoint(thiz.transform.position) + Vector3.up * 2f;
			}
		}, "Prop");
		Handles.EndGUI();
	}
}
