//
// TaskChecker.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using System;
using System.Collections.Generic;
using XPlugin.Data.Json;

namespace GameClient {
	public abstract class TaskChecker {
		public Action OnTaskComplete;
		public Action OnTaskFail;
		public virtual ITaskStat TaskStat { get; set; }

		protected static Dictionary<string, Func<TaskChecker>> checkerMap = new Dictionary<string, Func<TaskChecker>>();
		protected bool enabled = false;
		protected bool completed = false;
		protected int goal = 0;

		public static void RegisterCheckerClass<T>() where T : TaskChecker, new() {
			string name = typeof(T).Name;
			name = name.Replace("Checker", "");
			RegisterChecker(name, () => { return new T(); });
		}

		public static void RegisterChecker(string name, Func<TaskChecker> func) {
			if (checkerMap.ContainsKey(name)) {
				checkerMap[name] = func;
			} else {
				checkerMap.Add(name, func);
			}
		}

		public static void ClearRegistedChecker() {
			checkerMap.Clear();
		}

		public static TaskChecker CreateDefaultChecker() {
			return new DefaultTaskChecker();
		}

		public static T CreateChecker<T>(string paramStr, int goal, ITaskStat taskstat) where T : TaskChecker, new() {
			T checker = new T();
			checker.SetTaskParam(paramStr);
			checker.TaskGoal = goal;
			checker.TaskStat = taskstat ?? new NullTaskStat();
			return checker;
		}

		public static TaskChecker CreateChecker(string taskCheckerName, string paramStr, int goal, ITaskStat taskstat) {
			TaskChecker checker = null;

			Func<TaskChecker> func;
			checkerMap.TryGetValue(taskCheckerName, out func);
			if (func != null) {
				checker = func();
			} else {
				if (!string.IsNullOrEmpty(taskCheckerName)) {
					Type t = typeof(TaskChecker);
					string fullName = t.Namespace + "." + taskCheckerName + "Checker";
					checker = t.Assembly.CreateInstance(fullName) as TaskChecker;
				}
			}

			if (checker == null) {
				Debug.LogError("TaskChecker " + taskCheckerName + " not found!");
				checker = new NullTaskChecker();
			}

			checker.SetTaskParam(paramStr);
			checker.TaskGoal = goal;
			checker.TaskStat = taskstat ?? new NullTaskStat();
			return checker;
		}

		/// <summary>
		/// 设置参数
		/// </summary>
		/// <param name="paramStr"></param>
		public virtual void SetTaskParam(string paramStr) {
		}

		/// <summary>
		/// 初始化TaskInfo时的回调
		/// </summary>
		/// <param name="json"></param>
		public virtual void OnInitTaskInfo(TaskInfo info, JObject json) {
		}

		/// <summary>
		/// 任务进度
		/// </summary>
		public abstract int TaskProgress {
			get;
		}

		/// <summary>
		/// 任务目标
		/// </summary>
		public virtual int TaskGoal {
			get {
				return goal;
			}
			set {
				goal = value;
			}
		}

		/// <summary>
		/// 进度字符串
		/// </summary>
		public virtual string ProgressStr {
			get {
				int progress = TaskProgress;
				if (progress > goal) {
					progress = goal;
				}
				return progress + "/" + goal;
			}
		}

		/// <summary>
		/// 是否完成
		/// </summary>
		public bool Completed {
			get {
				return completed;
			}
		}

		/// <summary>
		/// 开关
		/// </summary>
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				if (enabled != value) {
					enabled = value;
					if (value) {
						OnEnable();
						Check();
					} else {
						OnDisable();
					}
				}
			}
		}

		/// <summary>
		/// 是否展示
		/// </summary>
		public virtual bool IsShow(TaskInfo taskInfo) {
			//            return taskInfo.State == TaskState.Doing || taskInfo.State == TaskState.Completed;
			return taskInfo.State != TaskState.Locked;
		}

		/// <summary>
		/// 设置为已完成状态
		/// </summary>
		public virtual void SetAsCompleted() {
			completed = true;
			Enabled = false;
			if (OnTaskComplete != null) {
				OnTaskComplete();
			}
		}

		/// <summary>
		/// 检查进度
		/// </summary>
		/// <returns>是否完成</returns>
		public virtual bool Check() {
			int nowvalue = TaskProgress;
			if (nowvalue >= TaskGoal) {
				if (!completed) {
					SetAsCompleted();
				}
			} else {
				if (completed) {
					completed = false;
					if (OnTaskFail != null) {
						OnTaskFail();
					}
				}
			}
			return completed;
		}

		protected virtual void OnEvent(EventEnum eventType, params object[] args) {
			Check();
		}

		protected virtual void OnEnable() {
			foreach (EventEnum evtEnum in EventEnums) {
				Client.EventMgr.AddListener(evtEnum, OnEvent);
			}
		}

		protected virtual void OnDisable() {
			foreach (EventEnum evtEnum in EventEnums) {
				Client.EventMgr.RemoveListener(evtEnum, OnEvent);
			}
		}


		protected virtual EventEnum[] EventEnums {
			get {
				return new EventEnum[0];
			}
		}

		public void SetStat(string key, object value) {
			TaskStat.SetStat(key, value);
		}

		public object GetStat(string key) {
			return TaskStat.GetStat(key);
		}

		public T GetStat<T>(string key) {
			object va = GetStat(key);
			if (va != null) {
				return (T)Convert.ChangeType(va, typeof(T));
			} else {
				return default(T);
			}
		}
	}

	public class NullTaskChecker : TaskChecker {
		public override int TaskProgress {
			get {
				return 0;
			}
		}

		public override int TaskGoal {
			get {
				return 1;
			}
		}
	}

	public class DefaultTaskChecker : TaskChecker {
		public override int TaskProgress {
			get {
				return 1;
			}
		}

		public override int TaskGoal {
			get {
				return 1;
			}
		}
	}
}