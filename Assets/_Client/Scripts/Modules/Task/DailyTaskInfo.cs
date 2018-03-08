//
// DailyTaskInfo.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using XPlugin.Data.Json;

namespace GameClient {
	public class DailyTaskInfo : TaskInfo {
		public DailyTaskInfo(TaskData data)
			: base(data) {
			CreateChecker();
		}

		public override void Enable() {
			if (!enabled) {
				NotifyMark.SetParentNotify(Client.Task.DailyTaskNotifyMark);
			}
			base.Enable();
		}

		protected override void CreateChecker() {
			TaskChecker = TaskChecker.CreateChecker(Data.TaskType, Data.TaskParam, Data.TaskGoal, DailyTaskStat.Create());
		}
	}

	public class DailyTaskStat : ITaskStat {
		protected static DailyTaskStat ins = null;

		public static DailyTaskStat Create() {
			if (ins == null) {
				ins = new DailyTaskStat();

			}
			return ins;
		}

		public JObject Root {
			get {
				// return Client.System.DailyValue["DailyTask"]["Stat"].AsObject();
				return Client.User.UserInfo.DailyStatValue;
			}
		}

		public void SetStat(string key, object value) {
			JToken token = JToken.Create(value);
			Root[key] = token;
			Client.Task.SaveData();
		}

		public object GetStat(string key) {
			JToken value = Root[key];
			if (value.IsValid) {
				if (value.IsValue) {
					return value.GetValue();
				} else {
					return value;
				}
			} else {
				return null;
			}
		}
	}
}