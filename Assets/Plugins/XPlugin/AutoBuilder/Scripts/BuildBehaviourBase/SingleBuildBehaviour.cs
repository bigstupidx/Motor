#if UNITY_EDITOR //must have
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace XPlugin.AutoBuilder {


	public class SingleBuildBehaviour : SortableBuildBehaviour {

		[ContextMenu("TestStart")]
		public void TestStart() {
			OnSingleBuildStart(GetComponent<SingleBuild>());
		}

		[ContextMenu("TestFinish")]
		public void TestFinish() {
			OnSingleBuildFinish(true, GetComponent<SingleBuild>());
		}


		public virtual void OnSingleBuildStart(SingleBuild arg) {
		}

		public virtual void OnSingleBuildFinish(bool success, SingleBuild arg) {
		}

	}
}
#endif