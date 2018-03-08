//
// RotateItem.cs
//
// Author:
// [WeiHuajian]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameUI{
	public class RotateItem : MonoBehaviour
	{
		public static List<RotateItem> rList = new List<RotateItem>();
		public bool IsFront = false, IsInit = false;
		public Action onInit = null;

		/// <summary>
		/// 1在最前面，-1在最后面
		/// </summary>
		public float posZ {
			get {return - transform.position.z / r;}
		}

		public float r = 1f;
		public Transform Center;
		public Vector3 frontRot = Vector3.zero;

		public virtual void Init() {
			rList.Add(this);
			r = Vector3.Distance(this.transform.position, Center.position);
			frontRot = new Vector3(0, Mathf.Acos(Vector3.Dot(this.transform.localPosition.normalized, new Vector3(0, 0, -1).normalized))* (180f/Mathf.PI), 0);
			if (this.transform.localPosition.x < 0) {
				frontRot = new Vector3(0,360,0) - frontRot;
			}
			if (onInit != null)
			{
				onInit();
				onInit = null;
			}
			IsInit = true;
		}

		public virtual void Reset() {
			rList.Remove(this);
		}

		public virtual void SetFront(bool isFront)
		{
			IsFront = isFront;
			
		}
	}
}
