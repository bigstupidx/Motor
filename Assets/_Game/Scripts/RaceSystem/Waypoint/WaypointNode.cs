//
// WaypointNode.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;

public enum WayPointType {
	Normal = 1,
	Start = 2,
	Finish = 3
}

public class WaypointNode : MonoBehaviour {
	public int Index;

	public float HalfWidth = 20;

	public WaypointManager Manager;

	public WaypointNode NextPoint;
	public float NextDistance;
	public WaypointNode PrePoint;
	public float PreDistance;

	public WayPointType Type = WayPointType.Normal;

	public Vector3 Left;

	public Vector3 Right;

	public virtual void OnValidate() {
		this.Left = this.Position + this.transform.right * -this.HalfWidth;
		this.Right = this.Position + this.transform.right * this.HalfWidth;
	}

	public Vector3 Position {
		get { return this.transform.position; }
		set { this.transform.position = value; }
	}

	private void OnDrawGizmos() {
		if (this.Manager != null) {
			if (this.Manager.alwaysGizmos) {
				OnDrawGizmosSelected();
			}
		}
	}

	private void OnDrawGizmosSelected() {
		if (this.Manager != null) {
			switch (Type) {
				case WayPointType.Start:
					Gizmos.DrawIcon(this.transform.position, this.Manager.spIconName);
					break;
				case WayPointType.Finish:
					Gizmos.DrawIcon(this.transform.position, this.Manager.fpIconName);
					break;
				default:
					Gizmos.DrawIcon(this.transform.position, this.Manager.wpIconName);
					break;
			}
			Gizmos.color = this.Manager.gizmosColor;
			Gizmos.DrawLine(this.Left, this.Right);
			this.Manager.OnDrawGizmosSelected();
		}
	}
}

