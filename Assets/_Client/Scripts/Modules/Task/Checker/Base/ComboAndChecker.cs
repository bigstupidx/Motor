//
// ComboAndChecker.cs
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
	public class ComboAndChecker : ComboChecker
	{
		public override int TaskProgress
		{
			get
			{
				foreach (TaskChecker checker in CheckerList) {
					if (!checker.Completed) {
						return 0;
					}
				}
				return 1;
			}
		}

		public override int TaskGoal
		{
			get
			{
				return 1;
			}
		}
	}
}