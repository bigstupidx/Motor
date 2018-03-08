using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using XPlugin.AutoBuilder;

[CustomEditor(typeof(BuildChannel))]
public class BuildChannelEditor : Editor {
	private BuildChannel _target;

	private ReorderableList _reorderList;

	void DrawListElement(Rect rect, int index, bool isActive, bool isFocused) {
		var channel = this._target.Channels[index];
		rect.width = 20;
		channel.Build = GUI.Toggle(rect, channel.Build, "");
		rect.x += rect.width;
		rect.width = 100;
		channel.Name = GUI.TextField(rect, channel.Name);
		rect.x += rect.width;
		rect.width = 35;
		if (GUI.Button(rect, "工程")) {
			this._target.GenerateSdkProject(this._target.GetComponent<SingleBuild>().TargetPath, channel.Name);
		}
		rect.x += rect.width;
		if (GUI.Button(rect, "编译")) {
			this._target.BuildProject(this._target.GetChannelProjPath(this._target.GetComponent<SingleBuild>().TargetPath, channel.Name));
		}
		rect.x += rect.width;
		if (GUI.Button(rect, "异步")) {
			this._target.BuildProject(this._target.GetChannelProjPath(this._target.GetComponent<SingleBuild>().TargetPath, channel.Name), true);
		}
	}


	void OnEnable() {
		this._target = this.target as BuildChannel;
		var channelsProperty = this.serializedObject.FindProperty("Channels");
		this._reorderList = new ReorderableList(this.serializedObject, channelsProperty, true, true, true, true);
		this._reorderList.drawElementCallback = DrawListElement;
		this._reorderList.drawHeaderCallback = DrawHeaderCallback;
	}

	private void DrawHeaderCallback(Rect rect) {
		GUI.Label(rect, "渠道列表");
	}


	public override void OnInspectorGUI() {
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("SDK目录")) {
			EditorUtility.OpenWithDefaultApp(this._target.ChannelSdkPath);
		}
		GUILayout.EndHorizontal();
		base.OnInspectorGUI();
		this.serializedObject.Update();
		this._reorderList.DoLayoutList();
		this.serializedObject.ApplyModifiedProperties();
	}
}
