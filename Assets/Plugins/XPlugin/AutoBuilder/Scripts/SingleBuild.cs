#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace XPlugin.AutoBuilder {

	public sealed class SingleBuild : MonoBehaviour {

		public BuildTarget BuildTarget;
		public string BuildPath="Build";

		[HideInInspector]
		public BuildOptions[] BuildOptions;

		public BuildOptions BuildOption {
			get {
				var ret = UnityEditor.BuildOptions.None;
				if (this.BuildOptions != null) {
					foreach (var option in this.BuildOptions) {
						ret |= option;
					}
				}
				return ret;
			}
		}

		public string TargetPath {
			get {
				string ext = AutoBuilderUtility.GetExtensionByBuildTarget(this.BuildTarget, this.BuildOption);
				if (!string.IsNullOrEmpty(ext)) {
					ext = "." + ext;
				}
				return this.BuildDir + ext;
			}
		}

		public string BuildDir {
			get {
				string dir = Path.Combine("AutoBuilder", this.BuildPath);
				if (!Directory.Exists(dir)) {
					Directory.CreateDirectory(dir);
				}
				return dir;
			}
		}

		[ContextMenu("Build")]
		public void Build() {
			OnBuildStart();
			CallAttr.CallAllAttrMethodByOrder(typeof(OnSingleBuildStartAttr), this);
			if (File.Exists(this.TargetPath)) {
				File.Delete(this.TargetPath);
			}
			if (Directory.Exists(this.TargetPath)) {
				Directory.Delete(this.TargetPath, true);
			}
			string result = "";
			result = BuildPipeline.BuildPlayer(AutoBuilderUtility.GetBuildScenes(), this.TargetPath, this.BuildTarget, this.BuildOption);
			OnBuildFinish(result);
			CallAttr.CallAllAttrMethodByOrder(typeof(OnSingleBuildFinishAttr), this, result);
			EditorUtility.OpenWithDefaultApp(this.BuildDir);//show build result
		}


		private void OnBuildStart() {
			var singles = GetComponentsInChildren<SingleBuildBehaviour>();
			List<SingleBuildBehaviour> singleList = new List<SingleBuildBehaviour>(singles);
			singleList.Sort((x, y) => x.order - y.order);
			foreach (var s in singleList) {
				if (s.enabled) {
					s.OnSingleBuildStart(this);
				}
			}
		}

		private void OnBuildFinish(string result) {
			var singles = GetComponentsInChildren<SingleBuildBehaviour>();
			List<SingleBuildBehaviour> singleList = new List<SingleBuildBehaviour>(singles);
			singleList.Sort((x, y) => x.order - y.order);
			bool success = string.IsNullOrEmpty(result);
			foreach (var s in singleList) {
				if (s.enabled) {
					s.OnSingleBuildFinish(success, this);
				}
			}
		}

	}
}

#endif