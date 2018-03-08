#if UNITY_EDITOR && UNITY_IOS
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor.iOS.Xcode;
using XPlugin.AutoBuilder;

public class ModifyXCodeProject : SingleBuildBehaviour {
	[System.Serializable]
	public struct BuildProperty {
		public string Key;
		public string Value;

		public BuildProperty(string key, string value) {
			this.Key = key;
			this.Value = value;
		}
	}

	public string[] AddFrameworks = new string[] {
		"AdSupport.framework",
		"CoreTelephony.framework",
		"Security.framework",
		"SystemConfiguration.framework",
	};

	public string[] Other_LDFlags = new[] { "-ObjC", "-w" };

	public string[] AddLibs = new string[] { "libz.tbd" };

	public BuildProperty[] SetBuildProperties = new BuildProperty[] {
		new BuildProperty("DEBUG_INFORMATION_FORMAT","dwarf"),
		new BuildProperty("ENABLE_BITCODE","NO"),
		new BuildProperty("IPHONEOS_DEPLOYMENT_TARGET","6.0"),
	};


	public override void OnSingleBuildFinish(bool success, SingleBuild arg) {
		string buildPath = arg.TargetPath;
		PBXProject proj = new PBXProject();
		var path = PBXProject.GetPBXProjectPath(buildPath);
		proj.ReadFromFile(path);
		string target = proj.TargetGuidByName("Unity-iPhone");

		foreach (var f in this.AddFrameworks) {
			proj.AddFrameworkToProject(target, f, false);
		}

		foreach (var lib in this.AddLibs) {
			proj.AddFileToBuild(target, proj.AddFile("usr/lib/" + lib, "Framework/" + lib, PBXSourceTree.Sdk));
		}

		foreach (var buildProperty in this.SetBuildProperties) {
			proj.SetBuildProperty(target, buildProperty.Key, buildProperty.Value);
		}

		foreach (var ldFlag in this.Other_LDFlags) {
			proj.AddBuildProperty(target, "OTHER_LDFLAGS", ldFlag);
		}

		proj.WriteToFile(path);
	}
}
#endif
