//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_14_GameNormal : StoryGuideBase {
		public override void Enable() {
			base.Enable();
			Client.EventMgr.AddListener(EventEnum.UI_EnterMenu, OnEvent);
		}

		public override void Disable() {
			base.Disable();
			Client.EventMgr.RemoveListener(EventEnum.UI_EnterMenu, OnEvent2);
		}

		public override void OnEvent(EventEnum eventType, params object[] args) {
			base.OnEvent(eventType, args);
			if ((string)args[0] == "MainBoard") {
                var id = int.Parse(GetType().Name.Split('_')[1]);
                Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
                StartGuide1();
			}
		}
		public void OnEvent2(EventEnum eventType, params object[] args) {
			base.OnEvent(eventType, args);
			if ((string)args[0] == "ChapterChooseBoard") {
				StartGuide2();
			}
		}

		public void StartGuide1() {
			Task.Reset();
			Task.Append(() => {
				Guide.ActiveDarkHole(true);
				Guide.ActiveFinger(true);

				Client.Guide.Log("指向闯关模式");
				var pathMatch = "UI/Modules/Menu/MainBoard(Clone)/SelectedMode/StartMode";
				var matchBtn = GameObject.Find(pathMatch);
				Guide.SetFingerPos(matchBtn.transform, new Vector3(0.3f, 0.5f, 0f));
				Guide.SetDarkHoleRadius(350f);
				Guide.SetWordPos(matchBtn.transform, new Vector3(0f, 0.3f, 0f), (LString.GAMECLIENT_STORYGUIDE_14_GAMENORMAL_STARTGUIDE1).ToLocalized());

				ClearFingerClick(matchBtn);
			});

			Task.End += () => {
				Client.EventMgr.RemoveListener(EventEnum.UI_EnterMenu, OnEvent);
				Client.EventMgr.AddListener(EventEnum.UI_EnterMenu, OnEvent2);
			};

			Task.Excute();
		}

		public void StartGuide2() {

			Task.Append(() => {
				Client.Guide.Log("指向第一章");
				var pathChapter = "UI/Modules/Menu/Group/ChapterChooseBoard(Clone)/Content/ScrollView/Container/chapterItem0/Content";
				var chapterBtn = GameObject.Find(pathChapter);
				Guide.SetFingerPos(chapterBtn.transform);
				Guide.SetDarkHoleRadius(350f);
				Guide.SetWordPos(chapterBtn.transform, new Vector3(0.9f, 0.5f, 0f), (LString.GAMECLIENT_STORYGUIDE_14_GAMENORMAL_STARTGUIDE2).ToLocalized());

				ClearFingerClick(chapterBtn);
			});

			Task.Append(() => {
				Client.Guide.Log("指向第二关");
				var pathLevel = "UI/Modules/Menu/Group/LevelChooseBoard(Clone)/ScrollView/RawImage/levelItem0";
				var levelBtn = GameObject.Find(pathLevel);
				Guide.SetFingerPos(levelBtn.transform);
				Guide.SetWordPos(levelBtn.transform, new Vector3(0.9f, 0.5f, 0f), (LString.GAMECLIENT_STORYGUIDE_14_GAMENORMAL_STARTGUIDE2_1).ToLocalized());

				ClearFingerClick(levelBtn);
			});

			Task.Append(() => {

				Client.Guide.Log("指向开始游戏按钮");
				var pathStart = "UI/Modules/Menu/Group/GamePrepareBoard(Clone)/Bottom/Start";
				var startBtn = GameObject.Find(pathStart);
				Guide.SetFingerPos(null, new Vector3(0.7901f, 0.0823f, 0.0000f));
				Guide.SetWordPos(null, new Vector3(0.5901f, 0.323f, 0), (LString.GAMECLIENT_STORYGUIDE_14_GAMENORMAL_STARTGUIDE2_2).ToLocalized());

				ClearFingerClick(startBtn);
			});

			Task.End += () => {
				Guide.Close();
				Disable();
			};

			Task.Excute();
		}
	}
}

