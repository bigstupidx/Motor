#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;


namespace XPlugin.AutoBuilder {

	public class TestAttr {

		[OnSingleBuildStartAttr]
		static void BeforeBuild(SingleBuild arg) {
			//		Debug.Log(string.Format("before build:{0},{1}", arg));
		}

		[OnSingleBuildFinishAttr]
		static void AfterBuild(SingleBuild arg, string result) {
			//		Debug.Log(string.Format("after build:{0},index:{1}result:{2}",arg, result));

		}
	}
}

#endif
