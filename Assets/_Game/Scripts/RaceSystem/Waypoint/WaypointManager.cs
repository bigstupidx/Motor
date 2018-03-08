//
// WaypointManager.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using UnityEngine;
using System.Collections.Generic;
using Game;

public partial class WaypointManager : MonoBehaviour {

	/// <summary>
	/// 道路宽度(限制切线斜率)
	/// </summary>
	public float WayWidth = 50f;

	public string wpIconName = "icon_waypoint";
	public string spIconName = "icon_startpoint";
	public string fpIconName = "icon_finishpoint";

	public Color gizmosColor = Color.green;
	public bool alwaysGizmos = false;

	/// <summary>
	/// 起点
	/// </summary>
	public WaypointNode StartPoint;
	/// <summary>
	/// 终点
	/// </summary>
	public WaypointNode FinishPoint;

	/// <summary>
	/// 赛道总长
	/// </summary>
	public float WayLength {
		get {
			if (Math.Abs(_wayLength - (-1)) < 1e-6) {
				var len = 0f;
				for (var i = 0; i < WaypointList.Count; i++) {
					len += WaypointList[i].NextDistance;
				}
				_wayLength = len;
			}
			return _wayLength;
		}
	}
	private float _wayLength = -1;

	/// <summary>
	/// 反转
	/// </summary>
	[NonSerialized]
	public bool IsReverse;

	/// <summary>
	/// 路径点
	/// </summary>
	public List<WaypointNode> WaypointList = new List<WaypointNode>();

	private void OnEnable() {
		var start = false;
		var finish = false;
		// 找出起止点
		foreach (var point in WaypointList) {
			if (point.Type == WayPointType.Start) {
				StartPoint = point;
				start = true;
				continue;
			}
			if (point.Type == WayPointType.Finish) {
				FinishPoint = point;
				finish = true;
			}
		}
		if (!start) {
			this.StartPoint = this.WaypointList[0];
			Debug.LogError("[Waypoint Manager] No Start Point!");
		}
		if (!finish) {
			this.FinishPoint = this.WaypointList[1];
			Debug.LogError("[Waypoint Manager] No Finish Point!");
		}
	}

	/// <summary>
	/// 反转赛道数据
	/// </summary>
	[ContextMenu("Reverse")]
	public void Reverse() {
		// 逆序
		for (var i = 0; i < WaypointList.Count; i++) {
			var node = WaypointList[i];
			var temp = node.NextPoint;
			node.NextPoint = node.PrePoint;
			node.PrePoint = temp;
		}
		WaypointList.Reverse();
		ReCaculate();
		// 把逆序前的第1个点设为起点(出生点在0和1之间)
		var newList = new List<WaypointNode>();
		var index = WaypointList.Count - 2;
		newList.AddRange(WaypointList.GetRange(index, WaypointList.Count - index));
		newList.AddRange(WaypointList.GetRange(0, index));
		WaypointList = newList;
		ReCaculate();
		IsReverse = !IsReverse;
	}

	/// <summary>
	/// 寻找距离最近的路径点
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public WaypointNode FindNearestNode(Vector3 pos) {
		var dis = float.MaxValue;
		WaypointNode node = null;
		for (var i = 0; i < WaypointList.Count; i++) {
			var temp = Vector3.Distance(WaypointList[i].Position, pos);
			if (temp < dis) {
				dis = temp;
				node = WaypointList[i];
			}
		}
		return node;
	}

	/// <summary>
	/// 寻找当前位置所属的路径点
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public WaypointNode FindCurrentNode(Vector3 pos) {
		var node = FindNearestNode(pos);
		if (WaypointMath.CheckPassWaypoint(pos, node.NextPoint, node)) {
			node = node.PrePoint;
		}
		return node;
	}

	/// <summary>
	/// 计算车辆位置到达某点的距离
	/// </summary>
	/// <param name="bike"></param>
	/// <param name="targetNode"></param>
	/// <returns></returns>
	public float GetDisToWayPoint(BikeBase bike, WaypointNode targetNode) {
		if (targetNode == null) return 0;
		if (WaypointList.IndexOf(targetNode) < 0) return 0;
		var node = bike.racerInfo.ActuallyPoint;
		if (bike.racerInfo.ActuallyPoint == null) {
			node = RaceManager.Ins.raceLine.WaypointManager.StartPoint;
		}
		var dis = Vector3.Distance(bike.transform.position, node.NextPoint.Position);
		if (node.NextPoint == targetNode) return dis;
		do {
			dis += node.NextDistance;
			node = node.NextPoint;
		} while (node != targetNode);
		return dis;
	}

	/// <summary>
	/// 重置
	/// </summary>
	void Reset() {
		foreach (Transform child in transform) {
			DestroyImmediate(child.gameObject);
		}
		var node = new GameObject().AddComponent<WaypointNode>();
		node.transform.SetParent(this.transform, false);
		node.Manager = this;
		this.WaypointList = new List<WaypointNode>();
		this.WaypointList.Add(node);
		ReCaculate();
	}

	[ContextMenu("Recaculate")]
	public void ReCaculate() {
		for (int i = 0; i < WaypointList.Count; i++) {
			var n = WaypointList[i];
			n.Index = i;
			n.transform.SetSiblingIndex(i);
			n.gameObject.name = "wp " + i;
			n.Manager = this;
			if (i == 0) {
				n.PrePoint = this.WaypointList[WaypointList.Count - 1];
			} else {
				n.PrePoint = this.WaypointList[i - 1];
			}

			if (i == WaypointList.Count - 1) {
				n.NextPoint = this.WaypointList[0];
			} else {
				n.NextPoint = this.WaypointList[i + 1];
			}
			n.PreDistance = Vector3.Distance(n.Position, n.PrePoint.Position);
			n.NextDistance = Vector3.Distance(n.Position, n.NextPoint.Position);
			n.transform.LookAt(n.NextPoint.transform);
			n.OnValidate();
		}

	}

	#region 数据可视化
	void OnDrawGizmos() {
		if (this.alwaysGizmos) {
			OnDrawGizmosSelected();
		}
	}
	public void OnDrawGizmosSelected() {
		if (WaypointList.Count == 0) return;
		WaypointNode n1, n2;

		var startPoint = WaypointList[0];
		n1 = startPoint;
		do {
			n2 = n1.NextPoint;
			// 连线
			Gizmos.color = this.gizmosColor;
			Gizmos.DrawLine(n1.Position, n2.Position);
			n1 = n2;
		} while (n2 != startPoint);
	}
	#endregion
}

