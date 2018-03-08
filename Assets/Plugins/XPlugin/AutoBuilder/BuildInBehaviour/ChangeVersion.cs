#if UNITY_EDITOR

using UnityEditor;
using XPlugin.AutoBuilder;

public class ChangeVersion : SingleBuildBehaviour
{
	public string AppVersion;


	public override void OnSingleBuildStart(SingleBuild arg) {
		PlayerSettings.bundleVersion = AppVersion;
	}

}


#endif