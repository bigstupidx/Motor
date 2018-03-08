#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace XPlugin.AutoBuilder {
	public class AutoBuilderUtility {

		public static string[] GetBuildScenes() {
			List<string> names = new List<string>();
			foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes) {
				if (e == null)
					continue;
				if (e.enabled)
					names.Add(e.path);
			}
			return names.ToArray();
		}

		public static string GetExtensionByBuildTarget(BuildTarget buildTarget, BuildOptions buildOptions) {
			string result;
			if ((buildOptions & BuildOptions.AcceptExternalModificationsToPlayer) != 0) {//如果是android工程，扩展名为空
				result = "";
				return result;
			}
			var method = typeof(EditorUtility).Assembly.GetType("UnityEditor.PostprocessBuildPlayer").
				GetMethod("GetExtensionForBuildTarget", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			result = (string)method.Invoke(null, new object[] { buildTarget, buildOptions });
			Debug.Log(result);
			return result;
		}

	}
}
#endif