//
// AchieveTaskInfo.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using System.Collections.Generic;
using XPlugin.Data.Json;

namespace GameClient {

	public class AchieveTaskInfo : TaskInfo {

		public AchieveTaskInfo(TaskData data) : base(data) {
			CreateChecker();
		}

		protected override void CreateChecker() {
			TaskChecker = TaskChecker.CreateChecker(Data.TaskType, Data.TaskParam, Data.TaskGoal, AchieveTaskStat.Create());
		}

		public override void CheckState() {
			base.CheckState();
			//			bool isShowed = Client.Setting.GetBool("Achieve_" + Data.ID);
			//			if (State == TaskState.Completed && UI.Load.CurrentLoad == UIType.None && !isShowed) {
			//				AchieveTip.Show(Data.Name);
			//				Client.Setting.SetBool("Achieve_" + Data.ID, true);
			//			}
		}

		public override void Enable() {
			//			if (!enabled) {
			//				switch (Data.ChildMode) {
			//				case ChildTaskMode.Adventure:
			//					NotifyMark.SetParentNotify(Client.Achieve.AdventureNotifyMark);
			//					break;
			//				case ChildTaskMode.Culture:
			//					NotifyMark.SetParentNotify(Client.Achieve.CultureNotifyMark);
			//					break;
			//				case ChildTaskMode.Activity:
			//					NotifyMark.SetParentNotify(Client.Achieve.ActivityNotifyMark);
			//					break;
			//				}
			//			}
			base.Enable();
		}

		/// <summary>
		/// 领取奖励
		/// </summary>
		/// <param name="onDone"></param>
		//		public override void GetReward(Action<bool, List<RewardItemInfo>> onDone = null)
		//		{
		//			if (!CanGetReward) {
		//				if (onDone != null) {
		//					onDone(false, null);
		//				}
		//				return;
		//			}
		//		}

		public class AchieveTaskStat : ITaskStat {
			protected static AchieveTaskStat ins = null;

			public static AchieveTaskStat Create() {
				if (ins == null) {
					ins = new AchieveTaskStat();
				}
				return ins;
			}

			public JObject Root {
				//get { return Client.Achieve.StatValue; }
				get { return Client.User.UserInfo.AchieveStatValue; }
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
}