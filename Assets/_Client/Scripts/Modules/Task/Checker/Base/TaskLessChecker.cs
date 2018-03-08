//
// LessChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using XPlugin.Data.Json;

namespace GameClient 
{
	/// <summary>
	/// 反向任务检查器
	/// </summary>
	public class TaskLessChecker : TaskChecker 
	{
		public override int TaskProgress
		{
			get { return -1; }
		}

		/// <summary>
		/// 检查进度
		/// </summary>
		/// <returns>是否完成</returns>
		public override bool Check() {
			int nowvalue = TaskProgress;
			if (nowvalue <= TaskGoal)
			{
				if (!completed)
				{
					SetAsCompleted();
				}
			} else
			{
				if (completed)
				{
					completed = false;
					if (OnTaskFail != null)
					{
						OnTaskFail();
					}
				}
			}
			return completed;
		}
	}
}

