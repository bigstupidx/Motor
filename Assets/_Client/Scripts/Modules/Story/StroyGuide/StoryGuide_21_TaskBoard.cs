//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_21_TaskBoard : StoryGuideBase {
		public override void Enable() {
			base.Enable();
			Client.EventMgr.AddListener(EventEnum.UI_EnterMenu, OnEvent);
		}

		public override void Disable() {
			base.Disable();
			Client.EventMgr.RemoveListener(EventEnum.UI_EnterMenu, OnEvent);
		}

		public override void OnEvent(EventEnum eventType, params object[] args) {
			base.OnEvent(eventType, args);
			if ((string)args[0] == "MainBoard" && Client.Task.TaskStatCount(TaskState.Completed) > 0) {
                var id = int.Parse(GetType().Name.Split('_')[1]);
                Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
                StartGuide();
			}
		}

		public void StartGuide() {
			Task.Reset();
			Task.Append(() => {
				Guide.ActiveDarkHole(true);
				Guide.ActiveFinger(true);

				Client.Guide.Log("指向任务按钮");
				var pathTaskBtn = "UI/Modules/Menu/MainBoard(Clone)/Bottom/Left/GameObject/Task/Task";
				var taskList = GameObject.Find(pathTaskBtn);
				//				Guide.SetFingerPos(taskList.transform);
				Guide.SetFingerPos(null, new Vector3(0.1876f, 0.0806f, 0.0000f));
				Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_21_TASKBOARD_STARTGUIDE).ToLocalized());

				ClearFingerClick(taskList);
			});

			Task.Append(() => {
				Client.Guide.Log("指向领取按钮");
				var pathGetBtn = "UI/Modules/Menu/Group/TaskBoard(Clone)/BG/List/Container/taskItem0/Btn";
				var getBtm = GameObject.Find(pathGetBtn);
				Guide.SetFingerPos(getBtm.transform);
				Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_21_TASKBOARD_STARTGUIDE_1).ToLocalized());

				ClearFingerClick(getBtm);
			});

			Task.End += () => {
				Guide.Close();
				Disable();
			};

			Task.Excute();
		}
	}
}

