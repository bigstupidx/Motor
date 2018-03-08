//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_22_ShopBoard : StoryGuideBase {
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
			if ((string)args[0] == "ShopBoard") {
                var id = int.Parse(GetType().Name.Split('_')[1]);
                Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
                StartGuide();
			}
		}

		public void StartGuide() {
			Task.Reset();
			Task.Append(() => {
				Client.Guide.Log("指向商城");

				Guide.ActiveBlackBack(true);
				Guide.SetWordPos(null, new Vector3(0.5f, 0.5f, 0), (LString.GAMECLIENT_STORYGUIDE_22_SHOPBOARD_STARTGUIDE).ToLocalized());

				ClearScreenClick();
			});

			Task.End += () => {
				Guide.Close();
				Disable();
			};

			Task.Excute();
		}
	}
}

