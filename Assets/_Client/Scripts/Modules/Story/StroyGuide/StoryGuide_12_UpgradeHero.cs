//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_12_UpgradeHero : StoryGuideBase {
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
			if ((string)args[0] == "MainBoard") {
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

				Client.Guide.Log("指向人物");
				var pathHero = "UI/Modules/Menu/MainBoard(Clone)/Bottom/Left/GameObject/Player/Player";
				var heroBtn = GameObject.Find(pathHero);
				Guide.SetFingerPos(null, new Vector3(0.6207f, 0.0562f, 0.0000f));
				Guide.SetWordPos(null, new Vector3(0.5669f, 0.2863f, 0.00f), (LString.GAMECLIENT_STORYGUIDE_12_UPGRADEHERO_STARTGUIDE).ToLocalized());

				ClearFingerClick(heroBtn);
			});

			Task.Append(() => {
				Client.Guide.Log("指向升级按钮");
				var pathUp = "UI/Modules/Menu/Group/HeroBoard(Clone)/Hero/Right/Update";
				var upBtn = GameObject.Find(pathUp);
				Guide.SetFingerPos(upBtn.transform, new Vector3(-0.05f, 0.5f, 0.0000f));
				Guide.SetWordPos(upBtn.transform, new Vector3(-0.05f, 0.7f, 0.0000f), (LString.GAMECLIENT_STORYGUIDE_12_UPGRADEHERO_STARTGUIDE_1).ToLocalized());
				ClearFingerClick(upBtn);
			});

			Task.Append(() => {
				Client.Guide.Log("指向返回按钮");
				var pathBack = "UI/Modules/Menu/Group/TopBoard(Clone)/BtnBack";
				var backBtn = GameObject.Find(pathBack);
				Guide.SetFingerPos(backBtn.transform);
				Guide.ActiveWord(false);

				Disable();
				ClearFingerClick(backBtn);
			});

			Task.End += () => {
				Guide.Close();
			};

			Task.Excute();
		}
	}
}

