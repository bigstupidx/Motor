//
// NormalTaskInfo.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using XPlugin.Data.Json;

namespace GameClient {
	public class LevelTaskInfo : TaskInfo {
		public LevelTaskInfo(TaskData data)
			: base(data) {
			CreateChecker();
		}

		public override void Enable() {
			if (!enabled) {
				NotifyMark.SetParentNotify(Client.Task.TaskNotifyMark);
			}
			base.Enable();
		}

		protected override void CreateChecker() {
			TaskChecker = TaskChecker.CreateChecker(Data.TaskType, Data.TaskParam, Data.TaskGoal, LevelTaskStat.Create());
		}
	}

	public class LevelTaskStat : ITaskStat {
		protected static LevelTaskStat ins = null;

		public static LevelTaskStat Create() {
			if (ins == null) {
				ins = new LevelTaskStat();
			}
			return ins;
		}

		public JObject Root {
			get {
				return Client.Task.LevelStatValue;
			}
		}

		public void SetStat(string key, object value) {
			JToken token = JToken.Create(value);
			Root[key] = token;
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