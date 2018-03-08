//
// ComboChecker.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
	public abstract class ComboChecker : TaskChecker
	{
		public List<TaskChecker> CheckerList = new List<TaskChecker>();

		protected override void OnEnable()
		{
			foreach (TaskChecker checker in CheckerList) {
				checker.Enabled = true;
			}
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			foreach (TaskChecker checker in CheckerList) {
				checker.Enabled = false;
			}
			base.OnDisable();
		}

		public void Add(TaskChecker checker)
		{
			CheckerList.Add(checker);
			checker.OnTaskComplete += () => { Check(); };
		}
	}
}