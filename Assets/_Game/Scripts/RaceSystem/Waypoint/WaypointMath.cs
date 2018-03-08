//
// WaypointMath.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using Game;
using UnityEngine;

/// <summary>
/// 路径点数学计算辅助类
/// </summary>
public class WaypointMath {
	/// <summary>
	/// 当前位置投射到地面后的点（从上到下）
	/// </summary>
	/// <param name="pos">当前位置</param>
	/// <returns>对应地面的点(投射失败返回本身)</returns>
	public static Vector3 AttachRoadPoint(Vector3 pos) {
		RaycastHit hit;
		var temp = pos;
		temp.y += 1000f;
		if (Physics.Raycast(temp, Vector3.down, out hit, 100000f, LayerMask.GetMask("29_Road"))) {
			return hit.point;
		}

		return pos;
	}

	public static RaycastHit? AttachRoadPointMesh(Vector3 pos) {
		RaycastHit hit;
		var temp = pos;
		temp.y += 1000f;
		if (Physics.Raycast(temp, Vector3.down, out hit, 100000f, LayerMask.GetMask("29_Road"))) {
			return hit;
		}

		return null;
	}
	/// <summary>
	/// 当前位置投射到左右两边的距离
	/// </summary>
	/// <param name="pos">当前位置</param>
	public static float LengthLeftWallPoint(Transform pos) {
		RaycastHit hit;
		Vector3 pos1;
		var temp = pos.localPosition;
		pos1 = pos.localPosition;
		if (Physics.Raycast(temp, pos.forward, out hit, 100000f, LayerMask.GetMask("28_Wall"))) {
			pos1 = hit.point;
		}
		if (pos1 != pos.localPosition) {
			return Vector3.Distance(pos1, pos.localPosition);
		}
		return -1;
	}

	public static float LengthRightWallPoint(Transform pos) {
		RaycastHit hit;
		Vector3 pos1;
		var temp = pos.localPosition;
		pos1 = pos.localPosition;
		if (Physics.Raycast(temp, -pos.forward, out hit, 100000f, LayerMask.GetMask("28_Wall"))) {
			pos1 = hit.point;
		}
		if (pos1 != pos.localPosition) {
			return Vector3.Distance(pos1, pos.localPosition);
		}
		return -1;
	}

	/// <summary>
	/// 当前位置投射到地面后的点(离自己最近的，当地面有多层时需使用)
	/// </summary>
	/// <param name="pos">当前位置</param>
	/// <returns>对应地面的点(投射失败返回本身)</returns>
	public static Vector3 AttachRoadPointNearest(Vector3 pos) {
		var rays = Physics.RaycastAll(pos, Vector3.down, 100000f, LayerMask.GetMask("29_Road"));
		if (rays.Length == 0) {
			return pos;
		}
		var result = rays[0].point;
		for (var i = 1; i < rays.Length; i++) {
			if (rays[i].point.sqrMagnitude < result.sqrMagnitude) {
				result = rays[i].point;
			}
		}
		return result;
	}

	/// <summary>
	/// 检测是否通过某个点
	/// </summary>
	/// <param name="newPos">当前位置</param>
	/// <param name="nowNode">当前已经通过的点</param>
	/// <param name="next">即将通过的点</param>
	/// <returns>结果</returns>
	public static bool CheckPassWaypoint(Vector3 newPos, WaypointNode nowNode, WaypointNode next) {
		var cross = new Vector2();
		var result = WaypointMath.GetIntersection(nowNode.Position, newPos, next.Left, next.Right, ref cross);
		if (result == 1) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 点到直线距离
	/// </summary>
	/// <param name="point">点坐标</param>
	/// <param name="linePoint1">直线上一个点的坐标</param>
	/// <param name="linePoint2">直线上另一个点的坐标</param>
	/// <returns>距离</returns>
	public static float DisPoint2Line(Vector3 point, Vector3 linePoint1, Vector3 linePoint2) {
		var vec1 = point - linePoint1;
		var vec2 = linePoint2 - linePoint1;
		var vecProj = Vector3.Project(vec1, vec2);
		var dis = Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
		return dis;
	}

	/// <summary>
	/// 判断两线段(由向量投影到平面形成的)是否相交
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="c"></param>
	/// <param name="d"></param>
	/// <param name="intersection"></param>
	/// <returns></returns>
	public static int GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d, ref Vector2 intersection) {
		var v1 = new Vector2(a.x, a.z);
		var v2 = new Vector2(b.x, b.z);
		var v3 = new Vector2(c.x, c.z);
		var v4 = new Vector2(d.x, d.z);
		return GetIntersection(v1, v2, v3, v4, ref intersection);
	}

	/// <summary>
	/// 判断两条线是否相交
	/// </summary>
	/// <param name="a">线段1起点坐标</param>
	/// <param name="b">线段1终点坐标</param>
	/// <param name="c">线段2起点坐标</param>
	/// <param name="d">线段2终点坐标</param>
	/// <param name="intersection">相交点坐标</param>
	/// <returns>是否相交 0:两线平行  -1:不平行且未相交  1:两线相交</returns>
	public static int GetIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 intersection) {
		//判断异常
		if (Mathf.Abs(b.x - a.y) + Mathf.Abs(b.x - a.x) + Mathf.Abs(d.y - c.y) + Mathf.Abs(d.x - c.x) < 1e-6) {
//			if (Mathf.Abs(c.x - a.x) < 1e-6) {
//				//Debug.Log("ABCD是同一个点！");
//			} else {
//				//Debug.Log("AB是一个点，CD是一个点，且AC不同！");
//			}
			return 0;
		}

		if (Mathf.Abs(b.y - a.y) + Mathf.Abs(b.x - a.x) < 1e-6) {
//			if (Mathf.Abs((a.x - d.x) * (c.y - d.y) - (a.y - d.y) * (c.x - d.x)) < 1e-6) {
//				//Debug.Log("A、B是一个点，且在CD线段上！");
//			} else {
//				//Debug.Log("A、B是一个点，且不在CD线段上！");
//			}
			return 0;
		}
//		if (Mathf.Abs(Mathf.Abs(d.y - c.y) + Mathf.Abs(d.x - c.x)) < 1e-6) {
//			if (Mathf.Abs((d.x - b.x) * (a.y - b.y) - (d.y - b.y) * (a.x - b.x)) < 1e-6) {
//				//Debug.Log("C、D是一个点，且在AB线段上！");
//			} else {
//				//Debug.Log("C、D是一个点，且不在AB线段上！");
//			}
//		}

		if (Mathf.Abs((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y)) < 1e-6) {
			//Debug.Log("线段平行，无交点！");
			return 0;
		}

		intersection.x = ((b.x - a.x) * (c.x - d.x) * (c.y - a.y) - c.x * (b.x - a.x) * (c.y - d.y) + a.x * (b.y - a.y) * (c.x - d.x)) /
						 ((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y));
		intersection.y = ((b.y - a.y) * (c.y - d.y) * (c.x - a.x) - c.y * (b.y - a.y) * (c.x - d.x) + a.y * (b.x - a.x) * (c.y - d.y)) /
						 ((b.x - a.x) * (c.y - d.y) - (b.y - a.y) * (c.x - d.x));

		if ((intersection.x - a.x) * (intersection.x - b.x) <= 0 && (intersection.x - c.x) * (intersection.x - d.x) <= 0 &&
			(intersection.y - a.y) * (intersection.y - b.y) <= 0 && (intersection.y - c.y) * (intersection.y - d.y) <= 0) {
			//Debug.Log("线段相交于点(" + intersection.x + "," + intersection.y + ")！");
			return 1; //'相交
		} else {
			//Debug.Log("线段相交于虚交点(" + intersection.x + "," + intersection.y + ")！");
			return -1; //'相交但不在线段上
		}
	}
}
