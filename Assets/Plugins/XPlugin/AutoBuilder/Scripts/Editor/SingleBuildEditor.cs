// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace XPlugin.AutoBuilder {
	[CustomEditor(typeof(SingleBuild))]
	public class SingleBuildEditor : Editor {

		private SingleBuild singleBuild;


		private ReorderableList _reorderList;

		void DrawListElement(Rect rect, int index, bool isActive, bool isFocused) {
			var singleProperty = this._reorderList.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(rect, singleProperty);
		}


		void OnEnable() {
			this.singleBuild = this.target as SingleBuild;
			var optionsProperty = this.serializedObject.FindProperty("BuildOptions");
			this._reorderList = new ReorderableList(this.serializedObject, optionsProperty, true, true, true, true);
			this._reorderList.drawElementCallback = DrawListElement;
			this._reorderList.drawHeaderCallback = rect => {
				GUI.Label(rect, "Build Options");
			};
		}


		public override void OnInspectorGUI() {
			GUILayout.BeginHorizontal();
			GUI.color = Color.green;
			if (GUILayout.Button("Build")) {
				this.singleBuild.Build();
				GUIUtility.ExitGUI();
			}
			GUI.color = Color.white;

			if (GUILayout.Button("Open Build Dir")) {
				EditorUtility.OpenWithDefaultApp(this.singleBuild.BuildDir);
			}
			GUILayout.EndHorizontal();
			base.OnInspectorGUI();

			this.serializedObject.Update();
			this._reorderList.DoLayoutList();
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}