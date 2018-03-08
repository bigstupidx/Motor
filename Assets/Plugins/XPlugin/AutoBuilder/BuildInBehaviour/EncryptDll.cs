#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace XPlugin.AutoBuilder {


	public class EncryptDll : SingleBuildBehaviour {
		public override void OnSingleBuildFinish(bool success, SingleBuild arg) {
			if (success) {
				DllEncrypt.DllEncrypt.EncryptProject(arg.TargetPath);
			}
		}
	}

}
#endif