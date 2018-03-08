#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using XPlugin.AutoBuilder;


public class CopyMainSdk : SingleBuildBehaviour {
	public string SdkPath = "/Build/Android/_Main";

	public override void OnSingleBuildFinish(bool success, SingleBuild arg) {
		FileHelper.DirCopy(this.SdkPath, arg.TargetPath);
	}
}
#endif