//
// RotateController.cs
//
// Author:
// [WeiHuajian]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameUI{
	public class RotateController : MonoBehaviour
	{
		/// <summary>
		/// 旋转中心
		/// </summary>
		public Transform center;
		/// <summary>
		/// 鼠标滑动与旋转速度调整参数
		/// </summary>
		public float adjust = 1f;
				
		private float lastTime = 0, lastX = 0, maxTime = 0.5f;

		/// <summary>
		/// 当前在最前面的对象
		/// </summary>
		public RotateItem Front
		{
			set
			{
				if (front != value)
				{
					if (front != null)
					{
						front.SetFront(false);
					}
				}
				value.SetFront(true);
				front = value;
				doCheck = true;
			}
			get
			{
				if (front == null)
				{
					front = GetFrontItem(0);
				}
				return front;
			}
		}
		private RotateItem front = null;

		/// <summary>
		/// 是否执行位置匹配
		/// </summary>
		protected bool doCheck = false;
		private float checkSpeed = 0;

		/// <summary>
		/// 是否执行下一个
		/// </summary>
		protected bool doNext = false;

		/// <summary>
		/// 抬手后旋转速度
		/// </summary>
		public float duration = 0.1f;

		/// <summary>
		/// 鼠标按下
		/// </summary>
		/// <param name="isButtonDown"></param>
		public void OnButton(bool isButtonDown)
		{
			if (!doNext)
			{
				if (isButtonDown)
				{
					lastTime = Time.time;
					doCheck = false;
					lastX = Input.mousePosition.x;
				}
				else
				{
					if (Mathf.Abs(Input.mousePosition.x - lastX) > 10 && Time.time - lastTime < maxTime)
					{						
						ShowNextOne(Input.mousePosition.x - lastX < 0);

					}
					else
					{
						Front = GetFrontItem(0);
					}
				}
			}
		}

		/// <summary>
		/// 鼠标移动
		/// </summary>
		/// <param name="v"></param>
		public void OnMove(Vector3 v)
		{
			center.SetLocalEulerAngleY(center.transform.localEulerAngles.y - v.x * adjust);
		}

		/// <summary>
		/// 获取根据Z轴前后排序的列表
		/// </summary>
		/// <returns></returns>
		List<RotateItem> GetSortList()
		{
			List<RotateItem> iList = new List<RotateItem>();
			foreach (var item in RotateItem.rList)
			{
				if (iList.Count <= 0)
				{
					iList.Add(item);
				}
				else
				{
					for (int i = 0; i < iList.Count; i++)
					{
						if (iList[i].posZ < item.posZ)
						{
							iList.Insert(i, item);
							break;
						}
						else if (i == iList.Count - 1)
						{
							iList.Add(item);
							break;
						}
					}
				}
			}
			return iList;
		}

		/// <summary>
		/// 获取相应排名对象（最前-0）
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		RotateItem GetFrontItem(int index)
		{
			List<RotateItem> iList = GetSortList();
			if (iList.Count <= 0 || index >= iList.Count)
			{
				return null;
			}
			else
			{
				return iList[index];
			}
		}

		void LateUpdate()
		{
			CheckFrontPos();
		}
		
		/// <summary>
		/// 将选定对象显示至最前
		/// </summary>
		void CheckFrontPos()
		{
			if (doCheck || doNext)
			{
				if (Mathf.Abs(center.transform.localEulerAngles.y - Front.frontRot.y) < 0.1f)
				{
					center.transform.localEulerAngles = Front.frontRot;
					doCheck = false;
					doNext = false;
				}
				center.transform.SetLocalEulerAngleY(Mathf.SmoothDampAngle(center.transform.localEulerAngles.y, Front.frontRot.y, ref checkSpeed, duration));
			}
		}

		/// <summary>
		/// 显示下一个（左右）
		/// </summary>
		/// <param name="right">true-右，false-左</param>
		public void ShowNext(bool right)
		{
			doNext = true;
			ShowNextOne(right);
		}

		void ShowNextOne(bool right) {
			List<RotateItem> iList = GetSortList();
			if (iList.Count > 2)
			{
				if (iList[1].transform.position.x > 0)
				{
					Front = right ? iList[1] : iList[2];
				}
				else
				{
					Front = right ? iList[2] : iList[1];
				}
			}
			else if (iList.Count > 1)
			{
				Front = iList[1];
			}
			else
			{
				doNext = false;
			}
		}

		public void ShowItem(RotateItem item) {
			Front = item;
		}

		public void ShowItemImmediately(RotateItem item)
		{
			Front = item;
			center.transform.localEulerAngles = item.frontRot;
			if (!item.IsInit) {
				item.onInit += () => { center.transform.localEulerAngles = item.frontRot; };
			}
		}
	}
}
