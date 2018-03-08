//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_18_BikeBoard : StoryGuideBase {
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
			if ((string)args[0] == "OnGarageBoardInited") {
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

				Client.Guide.Log("指向车辆列表，其他车辆");
				var pathBikeList = "UI/Modules/Menu/Group/GarageBoard(Clone)/SelectMotor/Left";
				var bikeList = GameObject.Find(pathBikeList);
				Guide.SetFingerPos(bikeList.transform, new Vector3(0.6f, 0.5f, 0f));
				Guide.SetWordPos(bikeList.transform, new Vector3(1f, 0.5f, 0), (LString.GAMECLIENT_STORYGUIDE_18_BIKEBOARD_STARTGUIDE).ToLocalized());

				ClearFingerClick(bikeList);
			});

			Task.End += () => {
				Guide.Close();
				Disable();
			};

			Task.Excute();
		}
	}
}

