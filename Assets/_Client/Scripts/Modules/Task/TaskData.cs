//
// TaskData.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System.Collections.Generic;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public enum TaskMode {
		/// <summary>
		/// 游戏内任务
		/// </summary>
		LevelTask = 1,
		/// <summary>
		/// 每日任务
		/// </summary>
		DailyTask = 2,
		/// <summary> 
		/// 成就任务
		/// </summary>
		AchieveTask = 3,
	}

	/// <summary>
	/// 活动子类型
	/// </summary>
	public enum ChildTaskMode {
		/// <summary>
		/// 冒险类
		/// </summary>
		Adventure = 0,
		/// <summary>
		/// 养成类
		/// </summary>
		Culture = 1,
		/// <summary>
		/// 活动类
		/// </summary>
		Activity = 2
	}

	public class TaskData {
		/// <summary>
		/// 任务ID
		/// </summary>
		public int ID;

		/// <summary>
		/// 名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 图标
		/// </summary>
		public IconData Icon;

		/// <summary>
		/// 描述
		/// </summary>
		public string Description;

		/// <summary>
		/// 任务模式
		/// </summary>
		public TaskMode TaskMode;

		/// <summary>
		/// 子类型
		/// </summary>
		public ChildTaskMode ChildMode;

		/// <summary>
		/// 需求玩家等级
		/// </summary>
		public int RequireLevel;

		/// <summary>
		/// 前置任务ID
		/// </summary>
		public int? RequireTaskID;

		/// <summary>
		/// 任务类型
		/// </summary>
		public string TaskType;

		/// <summary>
		/// 任务参数
		/// </summary>
		public string TaskParam;

		/// <summary>
		/// 任务目标
		/// </summary>
		public int TaskGoal;

		/// <summary>
		/// 前往界面
		/// </summary>
		public string GotoViewName;

		/// <summary>
		/// 界面参数
		/// </summary>
		public string GotoViewArgs;

		/// <summary>
		/// 奖励列表
		/// </summary>
		public List<ItemInfo> RewardList;

		/// <summary>
		/// 任务积分
		/// </summary>
		public int ScoreGet;

		public TaskData() {

		}

		public TaskData(SqliteDataReader reader) {
			ID = (int)reader.GetValue("ID");
			Name = (string)reader.GetValue("Name");
			Icon = Client.Icon[(int)reader.GetValue("Icon")];
			Description = (string)reader.GetValue("Description");

			TaskMode = (TaskMode)reader.GetValue("TaskMode");
			//            int? childMode = reader.GetNullable<int>("ChildTaskMode");
			//            if (childMode != null) {
			//                ChildMode = (ChildTaskMode)childMode;
			//            }
			RequireLevel = (int)reader.GetValue("RequireLevel");

			int? requireTaskID = reader.GetNullable<int>("RequireTask");
			if (requireTaskID != null) {
				RequireTaskID = (int)requireTaskID;
			}

			TaskType = (string)reader.GetValue("TaskType");
			TaskParam = (string)reader.GetValue("TaskParam");
			TaskGoal = (int)reader.GetValue("TaskGoal");
			GotoViewName = (string)reader.GetValue("GotoViewName");
			GotoViewArgs = (string)reader.GetValue("GotoViewArgs");

			//score of task
			int? scoreGet = reader.GetNullable<int>("TaskScore");
			ScoreGet = scoreGet == null ? 0 : (int)scoreGet;

			RewardList = new List<ItemInfo>();
			for (int i = 1; i <= 4; i++) {
				int? rewardID = reader.GetNullable<int>("Reward" + i + "_ID");
				if (rewardID != null) {
					ItemInfo reward = new ItemInfo((int)rewardID, (int)reader.GetValue("Reward" + i + "_Amount"));
					RewardList.Add(reward);
				}
			}
		}

		public JObject ToJson() {
			JObject root = new JObject();

			root["ID"] = ID;
			root["Name"] = Name;
			root["Icon"] = Icon.ID;
			root["Description"] = Description;

			root["TaskMode"] = (int)TaskMode;
			root["RequireLevel"] = RequireLevel;
			root["RequireTask"] = RequireTaskID;
			root["TaskType"] = TaskType;
			root["TaskParam"] = TaskParam;
			root["TaskGoal"] = TaskGoal;
			root["GotoViewName"] = GotoViewName;
			root["GotoViewArgs"] = GotoViewArgs;

			JArray reward = new JArray();
			root["Reward"] = reward;

			foreach (ItemInfo info in RewardList) {
				JObject item = new JObject();
				item["ItemID"] = info.Data.ID;
				item["Amount"] = (int)info.Amount;
				reward.Add(item);
			}

			return root;
		}
	}
}