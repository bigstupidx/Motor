using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace XPlugin.Update {

	public class BuildABWindow : EditorWindow {

		public Object[] ToBuildCompressed;
		public Object[] ToBuildNoCompressed;

		private SerializedObject serializedObject;
		private SerializedProperty _toBuildComProperty;
		private SerializedProperty _toBuildNoComProperty;

		[MenuItem("XPlugin/更新/显示构建Bundle窗口")]
		static void ShowWindow() {
			GetWindow<BuildABWindow>().Show();
		}


		void OnEnable() {
			serializedObject = new SerializedObject(this);
			_toBuildComProperty = serializedObject.FindProperty("ToBuildCompressed");
			_toBuildNoComProperty = serializedObject.FindProperty("ToBuildNoCompressed");
		}


		private Vector2 scrollPos;
		void OnGUI() {
			serializedObject.Update();
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label("将要构建成Bundle的物体放到列表中");
			EditorGUILayout.PropertyField(_toBuildComProperty, true);
			EditorGUILayout.PropertyField(_toBuildNoComProperty, true);
			GUILayout.EndScrollView();
			serializedObject.ApplyModifiedProperties();


			if (GUILayout.Button("构建")) {
				foreach (var o in ToBuildCompressed) {
					bool result = BuildAssetBundle.SimpleBuild(o, true);
					if (!result) {
						EditorUtility.DisplayDialog("error", "构建Bundle时出错:" + o, "ok");
						return;
					}
				}
				foreach (var o in ToBuildNoCompressed) {
					bool result = BuildAssetBundle.SimpleBuild(o, false);
					if (!result) {
						EditorUtility.DisplayDialog("error", "构建Bundle时出错:" + o, "ok");
						return;
					}
				}
				EditorUtility.RevealInFinder(BuildAssetBundle.FullAbBuildOutPutDir);
			}



		}

	}
}