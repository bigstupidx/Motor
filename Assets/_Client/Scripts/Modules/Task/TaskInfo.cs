//
// TaskData.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using System.Collections.Generic;

namespace GameClient {
	public enum TaskState {
		/// <summary>
		/// 未开启
		/// </summary>
		Locked,

		/// <summary>
		/// 任务执行中
		/// </summary>
		Doing,

		/// <summary>
		/// 已完成
		/// </summary>
		Completed,

		/// <summary>
		/// 奖励已领取
		/// </summary>
		Rewarded
	}

	public class TaskInfo {
		public NotifyMark NotifyMark = new NotifyMark();
		public TaskData Data;

		/// <summary>
		/// 任务状态
		/// </summary>
		public TaskState State;

		/// <summary>
		/// 前置任务
		/// </summary>
		public TaskInfo RequireTaskInfo;

		/// <summary>
		/// 任务检查器
		/// </summary>
		public TaskChecker TaskChecker;

		/// <summary>
		/// 任务进度
		/// </summary>
		public int Progress {
			get {
				return TaskChecker.TaskProgress;
			}
		}

		/// <summary>
		/// 任务目标
		/// </summary>
		public int Goal {
			get {
				return TaskChecker.TaskGoal;
			}
		}

		/// <summary>
		/// 进度字符串
		/// </summary>
		public string ProgressStr {
			get {
				return TaskChecker.ProgressStr;
			}
		}

		/// <summary>
		/// 是否展示
		/// </summary>
		public bool ShouldDisplay {
			get {
				//				bool hideShow = TaskChecker is INotShowInTaskList;
				//				bool hideButShowRedPoint = TaskChecker is INotShowInTaskListButShowRedPoint;
				return TaskChecker.IsShow(this);// && !hideShow && !hideButShowRedPoint;
			}
		}

		/// <summary>
		/// 领取奖励的回调
		/// </summary>
		public Action OnGetReward = null;
		protected bool enabled = false;

		public TaskInfo(TaskData data) {
			Data = data;
			State = TaskState.Locked;
			CreateChecker();
		}

		protected virtual void CreateChecker() {
			TaskChecker = TaskChecker.CreateChecker(Data.TaskType, Data.TaskParam, Data.TaskGoal, new NullTaskStat());
		}

		public virtual void Enable() {
			if (!enabled) {
				enabled = true;

				if (TaskChecker != null) {
					TaskChecker.OnTaskComplete += CheckState;
					TaskChecker.OnTaskFail += CheckState;
				}

				if (Data.RequireTaskID != null) {

					switch (Data.TaskMode) {
						case TaskMode.LevelTask:
						case TaskMode.DailyTask:
						case TaskMode.AchieveTask:
							RequireTaskInfo = Client.Task.GetTaskInfo(Data.TaskMode, (int)Data.RequireTaskID);
							break;
					}

					if (RequireTaskInfo != null) {
						RequireTaskInfo.OnGetReward += CheckState;
					}
				}

				CheckState();
			}
		}

		public virtual void Disable() {
			if (enabled) {
				enabled = false;

				if (TaskChecker != null) {
					TaskChecker.OnTaskComplete -= CheckState;
					TaskChecker.OnTaskFail -= CheckState;
				}

				if (RequireTaskInfo != null) {
					RequireTaskInfo.OnGetReward -= CheckState;
					RequireTaskInfo = null;
				}

				State = TaskState.Locked;
				TaskChecker.Enabled = false;
				NotifyMark.Reset();
			}
		}

		public virtual void SetState(TaskState state) {
			if (State != state) {
				State = state;
				CheckState();
			}
		}

		/// <summary>
		/// 检查状态
		/// </summary>
		public virtual void CheckState() {
			if (!enabled) {
				return;
			}

			if (State == TaskState.Rewarded) {
				NotifyMark.ChangeNotify(false);
				TaskChecker.Enabled = false;
				if (!TaskChecker.Completed) {
					TaskChecker.SetAsCompleted();
				}
				return;
			}

			//if (Client.Me.Level < Data.RequireLevel || (RequireTaskInfo != null && RequireTaskInfo.State != TaskState.Rewarded)) {
			if ((RequireTaskInfo != null && RequireTaskInfo.State != TaskState.Rewarded)) {
				State = TaskState.Locked;
			} else {
				TaskChecker.Check();
				if (!TaskChecker.Completed) {
					State = TaskState.Doing;
				} else {
					State = TaskState.Completed;
				}
			}

			NotifyMark.ChangeNotify(State == TaskState.Completed && ShouldDisplay);
			TaskChecker.Enabled = State == TaskState.Doing || State == TaskState.Completed;
		}

		/// <summary>
		/// 是否可领取奖励
		/// </summary>
		public bool CanGetReward {
			get {
				return State == TaskState.Completed;
			}
		}

		/// <summary>
		/// 领取奖励
		/// </summary>
		/// <param name="onDone"></param>
		public virtual void GetReward(Action<bool, List<RewardItemInfo>> onDone = null) {
			var ret = false;
			List<RewardItemInfo> list = new List<RewardItemInfo>();
			if (CanGetReward) {

				foreach (var i in Data.RewardList) {
					Client.Item.GainItem(i.Data, i.Amount, true);
					list.Add(new RewardItemInfo(i.Data.ID, i.Amount));
				}
				State = TaskState.Rewarded;
				// 记录领取状态
				switch (Data.TaskMode) {
					case TaskMode.DailyTask:
						Client.User.UserInfo.DailyRewardedList.AddNotRepeat(Data.ID);
						break;
					case TaskMode.AchieveTask:
						Client.User.UserInfo.AchieveRewardedList.AddNotRepeat(Data.ID);
						break;
				}
				ret = true;
				Client.EventMgr.SendEvent(EventEnum.Task_GetReward, Data.TaskMode, Data.ID);
				Client.Task.SaveData();
			}
			if (onDone != null) {
				onDone(ret, list);
			}
		}

	}
}