#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XPlugin.AutoBuilder {
	/// <summary>
	/// Unity构建的android工程会多添加一层已productName命名的目录，这个脚本将它移除
	/// </summary>
	public class MoveAndroidProject : SingleBuildBehaviour {
		public override void OnSingleBuildFinish(bool success, SingleBuild arg) {
			string srcDir = Path.Combine(arg.TargetPath, PlayerSettings.productName);
			if (Directory.Exists(srcDir)) {
				FileHelper.DirMove(srcDir, arg.TargetPath);
			} else {
				Debug.LogError("directory not found :" + srcDir);
			}


			//unity-android-resources不好搞。最好避免
			//			var unity_res_path = Path.Combine(arg.TargetPath, "unity-android-resources");
			//			if (Directory.Exists(unity_res_path)) {
			//				var target_unity_res_path = Path.Combine(Path.GetDirectoryName(arg.TargetPath), "unity-android-resources");
			//				FileHelper.DirMove(unity_res_path,target_unity_res_path);
			//			}
		}
	}
}

#endif