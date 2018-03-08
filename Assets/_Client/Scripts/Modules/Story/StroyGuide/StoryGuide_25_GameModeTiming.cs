//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using Game;
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_25_GameModeTiming : StoryGuideBase {


		public override void Enable() {
			base.Enable();
			Client.EventMgr.AddListener(EventEnum.Game_CountDownFinish, OnEvent);
		}

		public override void Disable() {
			base.Disable();
			Client.EventMgr.RemoveListener(EventEnum.Game_CountDownFinish, OnEvent);
		}

		public override void OnEvent(EventEnum eventType, params object[] args) {
			base.OnEvent(eventType, args);
			if (RaceManager.Ins.RaceMode == RaceMode.Timing) {
                var id = int.Parse(GetType().Name.Split('_')[1]);
                Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
                GameModeBase.Ins.IsAllowPause = false;
				StartGuide();
			}
		}

		public void StartGuide() {

			GameControlMode mode = ModGame.GetControlMode();
			string continueTip = "继续";
			switch (mode) {
				case GameControlMode.Btn:
				case GameControlMode.GravitySwipe:
					continueTip = "点击屏幕继续";
					break;
				case GameControlMode.RemoteControl:
					continueTip = "点击遥控器OK键继续";
					break;
				case GameControlMode.Somatosensory:
					continueTip = "点击遥控器OK键继续";
					break;
			}


			Task.Reset();

			Task.Append(() => {
				Client.Guide.Log("指向计时赛");

				Time.timeScale = 0f;
				Guide.ActiveBlackBack(true);
				Guide.SetWordPos(null, new Vector3(0.5f, 0.5f, 0), (LString.GAMECLIENT_STORYGUIDE_25_GAMEMODETIMING_STARTGUIDE).ToLocalized(),continueTip);

				ClearScreenClick();
			});

			Task.Append(() => {
				Guide.ActiveBlackBack(true);
				Guide.SetWordPos(null, new Vector3(0.5f, 0.5f, 0), (LString.GAMECLIENT_STORYGUIDE_25_GAMEMODETIMING_STARTGUIDE_1).ToLocalized(),continueTip);

				ClearScreenClick();
			});

			Task.End += () => {
				Guide.Close();
				Time.timeScale = 1f;
				GameModeBase.Ins.IsAllowPause = true;
				Disable();
			};

			Task.Excute();
		}
	}
}

