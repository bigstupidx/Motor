//
// SpawnpointNode.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;

public class SpawnpointNode : MonoBehaviour {
	public Vector3 Position {
		get { return transform.position; }
		set { transform.position = value; }
	}


#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
//		Gizmos.DrawIcon(Position, "icon_startpoint");

		Gizmos.DrawRay(transform.position, transform.forward);
		UnityEditor.Handles.Label(transform.position,name);
	}
#endif
}
