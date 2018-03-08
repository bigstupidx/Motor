//
// NotOpenChecker.cs
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
	public class NotOpenChecker : TaskChecker
	{
		public override int TaskProgress
		{
			get
			{
				return 0;
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