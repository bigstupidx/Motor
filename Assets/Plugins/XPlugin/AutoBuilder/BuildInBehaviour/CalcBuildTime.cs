#if UNITY_EDITOR

using System.Diagnostics;
using XPlugin.AutoBuilder;
using Debug = UnityEngine.Debug;

public class CalcBuildTime : SingleBuildBehaviour {

	private Stopwatch sw;//编译后非序列化字段会丢失，所以必须使用static或序列化

	public override void OnSingleBuildStart(SingleBuild arg) {
		sw = new Stopwatch();
		sw.Start();
	}

	public override void OnSingleBuildFinish(bool success, SingleBuild arg) {
		sw.Stop();
		Debug.Log(" totalTime:" + sw.Elapsed);
	}

}

#endif