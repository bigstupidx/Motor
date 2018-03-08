//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_13_UpgradeBike : StoryGuideBase {
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
			Task.Reset();
			Task.Append(() => {
				Guide.ActiveDarkHole(true);
				Guide.ActiveFinger(true);

				Client.Guide.Log("指向车辆");
				var pathBike = "UI/Modules/Menu/MainBoard(Clone)/Bottom/Left/GameObject/Garage/Garage";
				var bikeBtn = GameObject.Find(pathBike);
				//				Guide.SetFingerPos(bikeBtn.transform);
				Guide.SetFingerPos(null, new Vector3(0.7682f, 0.0824f, 0.0000f));
				Guide.SetWordPos(bikeBtn.transform, new Vector3(-0.5f, 0.5f, 0), (LString.GAMECLIENT_STORYGUIDE_13_UPGRADEBIKE_STARTGUIDE).ToLocalized());

				ClearFingerClick(bikeBtn);
			});

			Task.Append(() => {
				Client.Guide.Log("指向改装按钮");
				var pathUp = "UI/Modules/Menu/Group/GarageBoard(Clone)/Right/Image/Update";
				var upBtn = GameObject.Find(pathUp);
				Guide.SetFingerPos(null, new Vector3(0.1164f, 0.0619f, 0));
				Guide.SetWordPos(null, new Vector3(0.3459f, 0.2592f, 0.0000f), (LString.GAMECLIENT_STORYGUIDE_13_UPGRADEBIKE_STARTGUIDE_1).ToLocalized());

				ClearFingerClick(upBtn);
			});

			Task.Append(() => {
				Client.Guide.Log("指向升级按钮");
				var pathUp = "UI/Modules/Menu/Group/MotorUpdateBoard(Clone)/Right/Bottom/Speed/Update";
				var upBtn = GameObject.Find(pathUp);
				if (Guide.IsScreen())
					Guide.SetFingerPos(null, new Vector3(0.9205f, 0.7582f, 0f));
				else Guide.SetFingerPos(null, new Vector3(0.9205f, 0.8259f, 0f));
				Guide.SetWordPos(upBtn.transform, new Vector3(0.1f, 0.7f, 0f), (LString.GAMECLIENT_STORYGUIDE_13_UPGRADEBIKE_STARTGUIDE_2).ToLocalized());

				ClearFingerClick(upBtn);
			});

			Task.Append(() => {
				Client.Guide.Log("指向返回按钮");
				var pathBack = "UI/Modules/Menu/Group/TopBoard(Clone)/BtnBack";

				var backBtn = GameObject.Find(pathBack);
				Guide.SetFingerPos(backBtn.transform);
				Guide.ActiveWord(false);
				ClearFingerClick(backBtn);
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

