//
// SpawnpointManager.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using System.Collections.Generic;

public class SpawnpointManager : MonoBehaviour {

	public List<SpawnpointNode> SpawnpointList = new List<SpawnpointNode>();

	void Reset() {
		foreach (Transform child in transform) {
			DestroyImmediate(child.gameObject);
		}
		var node = new GameObject().AddComponent<SpawnpointNode>();
		node.transform.SetParent(transform, false);
		this.SpawnpointList = new List<SpawnpointNode>();
		this.SpawnpointList.Add(node);
		ReCaculate();
	}

	[ContextMenu("Recaculate")]
	public void ReCaculate() {
		for (int i = 0; i < SpawnpointList.Count; i++) {
			var n = SpawnpointList[i];
			n.transform.SetSiblingIndex(i);
			n.gameObject.name = "sp " + i;
		}
	}

	[ContextMenu("Align Road")]
	public void Align() {
		foreach (var sp in SpawnpointList) {
			sp.Position = WaypointMath.AttachRoadPoint(sp.Position);
		}
	}

}
