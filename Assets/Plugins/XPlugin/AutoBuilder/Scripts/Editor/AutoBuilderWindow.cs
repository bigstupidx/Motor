
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace XPlugin.AutoBuilder {



	public class AutoBuilderWindow : EditorWindow {

		public List<SingleBuild> Builds;


		[MenuItem("File/AutoBuilder &b")]
		static void ShowBuildWindow() {
			GetWindow<AutoBuilderWindow>(true);
		}


		void FindAllBatchBuilder() {
			Builds = new List<SingleBuild>();
			var files =new List<string>(Directory.GetFiles("Assets/Plugins/XPlugin/AutoBuilder", "*.prefab"));
			files.AddRange(Directory.GetFiles("Assets/_Release", "*.prefab"));
			foreach (var file in files) {
				var build = AssetDatabase.LoadAssetAtPath<SingleBuild>(file);
				if (build != null) {
					Builds.Add(build);
				}
			}
		}

		Vector2 scroll;
		private void OnGUI() {
			FindAllBatchBuilder();

			scroll = GUILayout.BeginScrollView(scroll);
			foreach (var build in Builds) {
				GUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField(build, typeof(SingleBuild), false);
				if (GUILayout.Button("Build")) {
					build.Build();
					GUIUtility.ExitGUI();
				}

				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}


	}



}

