//
// ModTask.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModTask : ClientModule {
		/// <summary>
		/// 任务小红点
		/// </summary>
		[JsonIgnore]
		public NotifyMark TaskNotifyMark = new NotifyMark();

		/// <summary>
		/// 每日活动小红点
		/// </summary>
		[JsonIgnore]
		public NotifyMark DailyTaskNotifyMark = new NotifyMark();

		/// <summary>
		/// 每日积分
		/// </summary>
		[NonSerialized]
		public int DailyScoreGet = 0;

		//        /// <summary>
		//        /// 功能开关
		//        /// </summary>
		//        public SystemFunc GetFunc(TaskMode type) {
		//            return Client.FuncMgr.GetFunc("Task." + type);
		//        }

		/// <summary>
		/// 已完成的每日任务数量
		/// </summary>
		[System.NonSerialized]
		public int FinishedDailyTask = 0;

		private bool _isNeedSaveWhenGameFinish = false;

		public JObject LevelStatValue = null;

		protected Dictionary<TaskMode, Dictionary<int, TaskData>> taskDataList = new Dictionary<TaskMode, Dictionary<int, TaskData>>();
		protected Dictionary<TaskMode, Dictionary<int, TaskInfo>> taskInfoList = new Dictionary<TaskMode, Dictionary<int, TaskInfo>>();
		protected Dictionary<string, List<TaskData>> dailyTaskTypeDataList = new Dictionary<string, List<TaskData>>();
		protected Dictionary<int, TaskData> achieveTaskDataList = new Dictionary<int, TaskData>();
		protected Dictionary<int, TaskInfo> achieveTaskInfoList = new Dictionary<int, TaskInfo>();
		protected Dictionary<int, TaskData> dailyTaskDataList = new Dictionary<int, TaskData>();
		protected Dictionary<int, TaskInfo> dailyTaskInfoList = new Dictionary<int, TaskInfo>();
		protected Dictionary<int, TaskData> levelTaskDataList = new Dictionary<int, TaskData>();
		public Dictionary<int, TaskInfo> levelTaskInfoList = new Dictionary<int, TaskInfo>();

		/// <summary>
		/// 获取指定任务数据
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="taskId"></param>
		/// <returns></returns>
		public TaskData this[TaskMode mode, int taskId] {
			get {
				TaskData ret = null;
				Dictionary<int, TaskData> dic = null;
				if (this.taskDataList.TryGetValue(mode, out dic)) {
					dic.TryGetValue(taskId, out ret);
				}
				return ret;
			}
		}

		/// <summary>
		/// 获取指定任务信息
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="taskId"></param>
		/// <returns></returns>
		public TaskInfo GetTaskInfo(TaskMode mode, int taskId) {
			TaskInfo ret = null;
			Dictionary<int, TaskInfo> dic = null;
			if (this.taskInfoList.TryGetValue(mode, out dic)) {
				dic.TryGetValue(taskId, out ret);
			}
			return ret;
		}

		/// <summary>
		/// 任务进度统计
		/// </summary>
		/// <returns></returns>
		public int TaskStatCount(TaskState state) {
			var count = 0;
			foreach (var task in achieveTaskInfoList.Values) {
				if (task.State == state) count++;
			}
			foreach (var task in dailyTaskInfoList.Values) {
				if (task.State == state) count++;
			}
			return count;
		}

		/// <summary>
		/// 获取显示的任务列表
		/// </summary>
		/// <param name="mode">任务模式</param>
		/// <returns></returns>
		public List<TaskInfo> GetDisplayTask(TaskMode mode) {
			var gotList = new List<TaskInfo>();
			var notgotList = new List<TaskInfo>();
			var rewardedList = new List<TaskInfo>();
			var dic = taskInfoList[mode];
			// 更新任务状态
			foreach (var info in dic.Values) {
				info.CheckState();
			}
			// 获取需要显示的任务并排序
			foreach (var info in dic.Values) {
				if (info.Data.TaskMode == mode && info.ShouldDisplay) {
					info.TaskChecker.Check();
					// 只取系列任务最高级别的
					var nextInfo = GetNextAchieveTaskInfo(info);
					if (info.State == TaskState.Rewarded && nextInfo != null) {
						var state = nextInfo.State;
						if (state != TaskState.Doing && state != TaskState.Completed)
							continue;
					}

					if (info.State == TaskState.Completed) {
						gotList.Add(info);
					} else if (info.State == TaskState.Rewarded) {
						rewardedList.Add(info);
					} else if (info.State == TaskState.Doing) {
						notgotList.Add(info);
					}
				}
			}
			// 排序，拼接
			gotList.Sort((t1, t2) => t1.Data.ID - t2.Data.ID);
			notgotList.Sort((t1, t2) => t1.Data.ID - t2.Data.ID);
			rewardedList.Sort((t1, t2) => t1.Data.ID - t2.Data.ID);
			gotList.AddRange(notgotList);
			gotList.AddRange(rewardedList);
			return gotList;
		}

		/// <summary>
		/// 获取后继成就任务
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		private TaskInfo GetNextAchieveTaskInfo(TaskInfo info) {
			foreach (var item in achieveTaskInfoList.Values) {
				if (item.RequireTaskInfo == info) return item;
			}
			return null;
		}

		/// <summary>
		/// 获取全部的任务列表
		/// </summary>
		/// <param name="mode">任务模式</param>
		/// <returns></returns>
		public List<TaskInfo> GetTask(TaskMode mode) {
			var list = new List<TaskInfo>();
			var dic = taskInfoList[mode];
			foreach (var info in dic.Values) {
				if (info.Data.TaskMode == mode) {
					info.TaskChecker.Check();
					list.Add(info);
				}
			}
			return list;
		}

		public bool HaveReward() {
			var list = GetTask(TaskMode.DailyTask);
			foreach (var info in list) {
				if (info.State == TaskState.Completed) {
					return true;
				}
			}
			list = GetTask(TaskMode.AchieveTask);
			foreach (var info in list) {
				if (info.State == TaskState.Completed) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 成就任务总积分
		/// </summary>
		public int AchieveTotalScore {
			get {
				return GetTask(TaskMode.AchieveTask).Sum(task => task.Data.ScoreGet);
			}
		}

		public override void InitData(DbAccess db) {
			//            TaskNotifyMark.SetParentNotify(Client.System.NotifyMarkTopToolBar);
			//            DailyTaskNotifyMark.SetParentNotify(Client.System.NotifyMarkTopToolBar);

			taskDataList.Clear();

			var reader = db.ReadFullTable("Task");
			while (reader.Read()) {
				var data = new TaskData(reader);
				switch (data.TaskMode) {
					case TaskMode.DailyTask:
						dailyTaskDataList.Add(data.ID, data);
						// 按任务类型分类
						var list = dailyTaskTypeDataList.GetValue(data.TaskType);
						if (list == null) {
							list = new List<TaskData>();
							dailyTaskTypeDataList.Add(data.TaskType, list);
						}
						list.Add(data);
						break;
					case TaskMode.AchieveTask:
						achieveTaskDataList.Add(data.ID, data);
						break;
					case TaskMode.LevelTask:
						levelTaskDataList.Add(data.ID, data);
						break;
				}
			}
			taskDataList.Add(TaskMode.DailyTask, dailyTaskDataList);
			taskDataList.Add(TaskMode.AchieveTask, achieveTaskDataList);
			taskDataList.Add(TaskMode.LevelTask, levelTaskDataList);

		}

		public override void ResetData() {
			TaskNotifyMark = new NotifyMark();
			DailyTaskNotifyMark = new NotifyMark();
			taskDataList.Clear();
			dailyTaskDataList.Clear();
			achieveTaskDataList.Clear();
			levelTaskDataList.Clear();
			TaskChecker.ClearRegistedChecker();
		}

		public override void InitInfo(string str) {
			// 添加信息字典映射
			taskInfoList.Add(TaskMode.DailyTask, dailyTaskInfoList);
			taskInfoList.Add(TaskMode.AchieveTask, achieveTaskInfoList);
			taskInfoList.Add(TaskMode.LevelTask, levelTaskInfoList);


			if (string.IsNullOrEmpty(str)) {
				ResetDailyTask();
			} else {
				var strList = JsonMapper.ToObject<List<string>>(str);
				var user = Client.User.UserInfo;
				user.LastResetTaskTime = long.Parse(strList[0]);
				user.AchieveStatValue = JToken.OptParse(strList[3]) as JObject;
				user.AchieveRewardedList = JsonMapper.ToObject<List<int>>(strList[5]);
				if (NeedDailyReset()) {
					ResetDailyTask();
				} else {
					user.DailyTasks = JsonMapper.ToObject<List<int>>(strList[1]);
					user.DailyStatValue = JToken.OptParse(strList[2]) as JObject;
					user.DailyRewardedList = JsonMapper.ToObject<List<int>>(strList[4]);

					// 加载已领取的每日任务并启用
					if (Client.User.UserInfo.DailyTasks != null && Client.User.UserInfo.DailyTasks.Count > 0) {
						foreach (var id in Client.User.UserInfo.DailyTasks) {
							var info = new DailyTaskInfo(dailyTaskDataList[id]);
							info.Enable();
							dailyTaskInfoList.Add(id, info);
						}
					}
				}
			}

			// 加载成就任务数据
			foreach (var data in achieveTaskDataList.Values) {
				var info = new AchieveTaskInfo(data);
				if (info.TaskChecker != null) {
					achieveTaskInfoList.Add(info.Data.ID, info);
				}
			}

			// 读取奖励领取状态
			foreach (var id in Client.User.UserInfo.DailyRewardedList) {
				var info = GetTaskInfo(TaskMode.DailyTask, id);
				if (info != null) {
					info.SetState(TaskState.Rewarded);
				}
			}
			foreach (var id in Client.User.UserInfo.AchieveRewardedList) {
				var info = GetTaskInfo(TaskMode.AchieveTask, id);
				if (info != null) {
					info.SetState(TaskState.Rewarded);
					Debug.Log("保存的成就" + info.Data.Name + "  " + info.State);
				}
			}
			Debug.Log("保存的成就结束");

			// 启用成就任务
			foreach (var info in achieveTaskInfoList.Values) {
				info.Enable();
			}

			Client.EventMgr.AddListener(EventEnum.System_OverDay, (e, args) => ResetDailyTask());
		}

		public override void ResetInfo() {
			foreach (var infoDic in taskInfoList.Values) {
				foreach (var info in infoDic.Values) {
					info.Disable();
				}
				infoDic.Clear();
			}
			FinishedDailyTask = 0;
			DailyScoreGet = 0;
			Client.User.UserInfo.DailyStatValue = new JObject();
			Client.User.UserInfo.AchieveStatValue = new JObject();
			Client.User.UserInfo.DailyTasks.Clear();
		}

		public bool NeedDailyReset() {
			var now = Client.System.DateOnLocal;
			var last = Client.User.UserInfo.LastResetTaskTime.ToDateTime();
			return last.Year != now.Year || last.Month != now.Month || last.Day != now.Day;
		}

		public void ResetLevelTask() {
			foreach (var task in levelTaskInfoList.Values) {
				task.Disable();
			}
			levelTaskInfoList.Clear();
		}

		/// <summary>
		/// 重设每日任务<para/>
		/// </summary>
		public void ResetDailyTask() {
			Client.User.UserInfo.LastResetTaskTime = Client.System.TimeOnLocal;
			// 清除
			FinishedDailyTask = 0;
			DailyScoreGet = 0;
			foreach (var info in dailyTaskInfoList.Values) {
				info.Disable();
			}
			dailyTaskInfoList.Clear();
			Client.User.UserInfo.ClearDaily();
			//取出所有任务 测试用 
			//			foreach (var list in dailyTaskTypeDataList.Values) {
			//				foreach (var data in list) {
			//					var info = new DailyTaskInfo(data);
			//					dailyTaskInfoList.AddOrReplace(data.ID, info);
			//					Client.User.UserInfo.DailyTasks.AddNotRepeat(data.ID);
			//					info.Enable();
			//				}
			//			}
			// 每种类型各随机创建一个
			foreach (var list in dailyTaskTypeDataList.Values) {
				var data = list.Random();
				var info = new DailyTaskInfo(data);
				dailyTaskInfoList.AddOrReplace(data.ID, info);
				Client.User.UserInfo.DailyTasks.AddNotRepeat(data.ID);
				info.Enable();
			}
			if (TaskBoard.Ins != null) {
				TaskBoard.Ins.Reload();
			}
		}

		public void SaveWhenGameFinish() {
			_isNeedSaveWhenGameFinish = true;
		}

		protected override void Update() {
			// 游戏中不保存任务数据
			if (Client.Game.IsGaming) {
				return;
			}
			// 游戏结束后立即存储数据
			if (_isNeedSaveWhenGameFinish) {
				SaveDataNow();
				_isNeedSaveWhenGameFinish = false;
			}
			base.Update();
		}

		public override string ToJson(UserInfo info) {
			if (info == null) {
				info = Client.User.UserInfo;
			}

			var strList = new List<string>(){
				info.LastResetTaskTime.ToString(),
				JsonMapper.ToJson(info.DailyTasks),
				info.DailyStatValue.ToString(),
				info.AchieveStatValue.ToString(),
				JsonMapper.ToJson(info.DailyRewardedList),
				JsonMapper.ToJson(info.AchieveRewardedList)
			};
			return JsonMapper.ToJson(strList);
		}
	}

}