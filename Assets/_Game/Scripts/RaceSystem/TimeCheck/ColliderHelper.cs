//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderHelper : MonoBehaviour
{
	public Action<Collider> TriggerEnter;
	public Action<Collider> TriggerStay;
	public Action<Collider> TriggerExit;

	public Action<Collision> CollisionEnter;
	public Action<Collision> CollisionStay;
	public Action<Collision> CollisionExit;

	private void OnTriggerEnter(Collider other)
	{
		if (TriggerEnter != null) TriggerEnter(other);
	}
	private void OnTriggerStay(Collider other) {
		if (TriggerStay != null) TriggerStay(other);
	}
	private void OnTriggerExit(Collider other) {
		if (TriggerExit != null) TriggerExit(other);
	}

	private void OnCollisionEnter(Collision other) {
		if (CollisionEnter != null) CollisionEnter(other);
	}
	private void OnCollisionStay(Collision other)
	{
		if (CollisionStay != null) CollisionStay(other);
	}
	private void OnCollisionExit(Collision other) {
		if (CollisionExit != null) CollisionExit(other);
	}

	public void Claer()
	{
		TriggerEnter = null;
		TriggerStay = null;
		TriggerExit = null;

		CollisionEnter = null;
		CollisionStay = null;
		CollisionExit = null;
	}
}
