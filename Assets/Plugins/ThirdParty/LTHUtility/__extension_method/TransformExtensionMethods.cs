using System;
using UnityEngine;
using System.Collections;

public static class TransformExtensionMethods {

	/// <summary>
	/// 重设缩放
	/// </summary>
	/// <param name="transform"></param>
	/// <returns></returns>
	public static Vector3 CounteractLocalScale(this Transform transform)
	{
		transform.localScale = CounteractLocalScaleRecursion(transform, Vector3.one);
		return transform.localScale;
	}

	private static Vector3 CounteractLocalScaleRecursion(Transform transform, Vector3 scale)
	{
		if (transform.parent != null)
		{
			scale = new Vector3(scale.x / transform.parent.localScale.x, scale.y / transform.parent.localScale.y, scale.z / transform.parent.localScale.z);
			return CounteractLocalScaleRecursion(transform.parent, scale);
		}
		return scale; 
	}

	/// <summary>
	/// 深度查找子节点
	/// Transform中的API只能查找第一层节点,使用该方法可以深度查找
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static Transform FindInAllChild(this Transform transform, string name) {
		foreach (var v in transform.GetComponentsInChildren<Transform>()) {
			if (v.gameObject.name == name) {
				return v;
			}
		}
		return null;
	}

	/// <summary>
	/// 深度查找子节点（模糊查询）
	/// Transform中的API只能查找第一层节点,使用该方法可以深度查找
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static Transform FindInAllChildFuzzy(this Transform transform, string name) {
		foreach (var v in transform.GetComponentsInChildren<Transform>())
		{
			if (v.gameObject.name.IndexOf(name, StringComparison.Ordinal) >= 0)
			{
				return v;
			}
		}
		return null;
	}

	/// <summary>
	/// 寻找指定子节点，不存在则创建
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static Transform FindOrCreate(this Transform transform, string name)
	{
		var trans = transform.FindChild(name);
		if (trans == null)
		{
			trans = new GameObject(name).transform;
			trans.SetParent(transform);
			trans.ResetLocal();
		}
		return trans;
	}

	public static void SetPositionX(this Transform transform, float x) {
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}

	public static void SetPositionY(this Transform transform, float y) {
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}

	public static void SetPositionZ(this Transform transform, float z) {
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
	}

	public static void SetLocalPositionX(this Transform transform, float x) {
		transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
	}

	public static void SetLocalPositionY(this Transform transform, float y) {
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}

	public static void SetLocalPositionZ(this Transform transform, float z) {
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
	}

	public static void SetLocalScaleX(this Transform transform, float x) {
		transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
	}

	public static void SetLocalScaleY(this Transform transform, float y) {
		transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
	}

	public static void SetLocalScaleZ(this Transform transform, float z) {
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
	}

	public static void SetLocalEulerAngleX(this Transform transform, float x) {
		transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}

	public static void SetLocalEulerAngleY(this Transform transform, float y) {
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
	}

	public static void SetLocalEulerAngleZ(this Transform transform, float z) {
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
	}

	public static void SetEulerAngleX(this Transform transform, float x) {
		transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
	}

	public static void SetEulerAngleY(this Transform transform, float y) {
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
	}

	public static void SetEulerAngleZ(this Transform transform, float z) {
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
	}

	public static void SetUniformLocalScale(this Transform transform, float uniformScale) {
		transform.localScale = Vector3.one * uniformScale;
	}


	public static void ResetLocal(this Transform transform) {
		transform.localPosition=Vector3.zero;
		transform.localRotation=Quaternion.identity;
		transform.localScale=Vector3.one;
	}

	public static Vector2 ScreenPos(this Transform transform) {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		return (new Vector2(screenPos.x, screenPos.y));
	}

	public static Vector2 ScreenPosRate(this Transform transform) {
		Vector2 screenPos = transform.ScreenPos();
		Vector2 rate = new Vector2(screenPos.x / (float)Screen.width, screenPos.y / (float)Screen.height);
		return rate;
	}
}
