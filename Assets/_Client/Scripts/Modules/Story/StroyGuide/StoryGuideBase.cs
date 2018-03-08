//
// StoryGuideBase.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using GameUI;

namespace GameClient {
	public class StoryGuideBase {
		public virtual int ID {
			get { return -1; }
		}

		public bool Active { get; private set; }

		public StoryGuideBoard Guide {
			get {
				if (StoryGuideBoard.Ins == null) {
					StoryGuideBoard.Show();
				}
				return StoryGuideBoard.Ins;
			}
		}

		public StoryGuideBase() {
			Active = false;
		}

		public virtual void Enable() {
			Active = true;
		}

		public virtual void Disable() {
			Active = false;
			var id = int.Parse(GetType().Name.Split('_')[1]);
			Client.Guide.FinishGuide(id);
		}

		public virtual void OnEvent(EventEnum eventType, params object[] args) {
		}

		public void ClearWithDelay(float time, Action onFinish = null) {
			Guide.DelayInvoke(() => {
				StoryTaskQueue.IsWaitForNext = false;
				Guide.HelpOnClickFinger.Clear();
				if (onFinish != null) onFinish();
			}, time);
		}

		public void ClearFingerClick(GameObject target = null, Action onFinish = null) {
			Guide.HelpOnClickFinger.ActClick += () => {
				if (!Guide.CheckEnd()) {
					return;
				}

				StoryTaskQueue.IsWaitForNext = false;
				Guide.HelpOnClickFinger.Clear();

				if (target != null) {
					HelpOnClick.SendOnClick(target);
				}
				if (onFinish != null) {
					onFinish();
				}
			};
		}

		public void ClearScreenClick(GameObject target = null, Action onFinish = null) {
			Guide.HelpOnClickScreen.ActClick += () => {
				if (!Guide.CheckEnd()) {
					return;
				}

				StoryTaskQueue.IsWaitForNext = false;
				Guide.HelpOnClickScreen.Clear();

				if (target != null) {
					HelpOnClick.SendOnClick(target);
				}
				if (onFinish != null) {
					onFinish();
				}
			};
		}
	}
}

