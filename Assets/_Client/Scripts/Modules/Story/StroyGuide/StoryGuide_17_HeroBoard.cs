//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_17_HeroBoard : StoryGuideBase {
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
			if ((string)args[0] == "OnHeroBoardInited") {
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

				Client.Guide.Log("指向人物列表，其他人物");
				var pathHeroList = "UI/Modules/Menu/Group/HeroBoard(Clone)/HeroList/List/List/Container/heroItem1";
				var heroList = GameObject.Find(pathHeroList);
				Guide.SetFingerPos(heroList.transform, new Vector3(0.74f, 0.5f, 0f));
				Guide.SetWordPos(heroList.transform, new Vector3(1f, 0.3f, 0f), (LString.GAMECLIENT_STORYGUIDE_17_HEROBOARD_STARTGUIDE).ToLocalized());

				ClearFingerClick(heroList);
			});

			Task.End += () => {
				Guide.Close();
				Disable();
			};

			Task.Excute();
		}
	}
}

