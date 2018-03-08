//
// NotifyMark.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;

namespace GameClient
{
	/// <summary>
	/// 通知标志（小红点）<para/>
	/// 可主动查询状态，通过IsNotify属性，或者直接把类当bool用<para/>
	/// 也可被动通知，通过注册OnChange事件来获得状态改变时的回调<para/>
	/// 支持层次结构，底层红点状态发生变化，上层跟着自动改变
	/// </summary>
	public class NotifyMark
	{
		/// <summary>
		/// 是否显示通知标志
		/// </summary>
		/// <param name="mark"></param>
		/// <returns></returns>
		public static implicit operator bool(NotifyMark mark)
		{
			return mark != null && mark.IsNotify;
		}

		/// <summary>
		/// 是否显示通知标志
		/// </summary>
		public bool IsNotify { get; protected set; }

		/// <summary>
		/// 状态更改代理<para/>
		/// 参数：bool 是否显示通知状态
		/// </summary>
		public event Action<bool> OnChange
		{
			add
			{
				_OnChange += value;
				value(IsNotify);
			}
			remove
			{
				_OnChange -= value;
				//value(false);
			}
		}

		/// <summary>
		/// 父节点
		/// </summary>
		protected NotifyMark ParentMark = null;

		protected event Action<bool> _OnChange = null;

		/// <summary>
		/// 子节点显示通知数量
		/// </summary>
		protected int SubNotifyCount = 0;

		public NotifyMark()
		{
			IsNotify = false;
		}

		/// <summary>
		/// 注册父节点
		/// </summary>
		/// <param name="parent"></param>
		public void SetParentNotify(NotifyMark parent)
		{
			if (ParentMark != parent) {
				if (ParentMark != null) {
					ParentMark.OnSubNotifyChange(false);
				}

				ParentMark = parent;
				if (parent != null && IsNotify) {
					parent.OnSubNotifyChange(true);
				}
			}
		}

		/// <summary>
		/// 更改状态
		/// </summary>
		/// <param name="isNotify">是否显示通知</param>
		public void ChangeNotify(bool isNotify)
		{
			if (IsNotify != isNotify) {
				//Debug.Log ("ChangeNotify:" + isNotify);
				IsNotify = isNotify;

				if (ParentMark != null) {
					ParentMark.OnSubNotifyChange(IsNotify);
				}

				if (_OnChange != null) {
					_OnChange(IsNotify);
				}
			}
		}

		public void Reset()
		{
			ChangeNotify(false);
			ParentMark = null;
			_OnChange = null;
		}

		protected void OnSubNotifyChange(bool notify)
		{
			if (notify) {
				SubNotifyCount++;
			} else {
				SubNotifyCount--;
			}

			ChangeNotify(SubNotifyCount > 0);
		}
	}
}