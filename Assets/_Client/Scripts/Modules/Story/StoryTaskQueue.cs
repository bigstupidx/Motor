//
// StoryTaskQueue.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GameClient {
	public static class StoryTaskQueue {
		/// <summary>
		/// 是否正在等待,为false继续执行下一个事件，由外部改变值触发
		/// </summary>
		public static bool IsWaitForNext;
		/// <summary>
		/// 是否正在执行引导
		/// </summary>
		public static bool IsGuide { get; private set; }
		public static List<Action> Tasks = new List<Action>();

		public static Action End = delegate { };
		public static Coroutine NowCoroutine;
		/// <summary>
		/// 构造初始化
		/// </summary>
		static StoryTaskQueue() {
			Reset();
		}

		/// <summary>
		/// 将新事件加入队列
		/// </summary>
		/// <param name="action">事件</param>
		public static void Append(Action action) {
			Tasks.Add(action);
		}

		/// <summary>
		/// 执行事件队列
		/// </summary>
		public static void Excute() {
			if (IsGuide) {
				Client.Guide.Log("Excuting...");
				return;
			}
			if (!Tasks.Any()) {
				Client.Guide.Log("Task");
				return;
			}
			IsGuide = true;
			NowCoroutine = ExcuteNext(Tasks.First());
		}
		/// <summary>
		/// 停止
		/// </summary>
		public static void Reset() {
			Client.Guide.Log("Reset");
			End = null;
			End += () => {
				Client.Guide.Log("End");
			};
			IsWaitForNext = false;
			IsGuide = false;
			if (NowCoroutine != null) {
				Client.Story.StopCoroutine(NowCoroutine);
			}
			Tasks.Clear();
		}

		/// <summary>
		/// 执行下一个
		/// </summary>
		/// <param name="action">当前事件</param>
		/// <returns>当前协程</returns>
		private static Coroutine ExcuteNext(Action action) {
			Client.Guide.Log("Excute action ...");
			// 标记引导开始
			IsWaitForNext = true;
			// 执行，并移除队列
			action();
			Tasks.Remove(action);
			// ......等待其他模块触发继续执行的条件
			// 满足条件时执行下一个
			var corutine = Client.Story.ExecuteWhen(
				() => {
					if (Tasks.Any()) {
						NowCoroutine = ExcuteNext(Tasks.First());
					} else {
						End();
						Reset();
					}
				},
				() => !IsWaitForNext);
			return corutine;
		}

	}

}
