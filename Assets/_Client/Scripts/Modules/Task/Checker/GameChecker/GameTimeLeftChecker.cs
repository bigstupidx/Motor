//
// GameTimeLeftChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 剩余一定时间完成比赛
	/// 计时赛关卡任务专用！！！
	/// </summary>
	public class GameTimeLeftChecker : TaskChecker {
		private bool _finish = false;

		public GameTimeLeftChecker() {
			_finish = false;
		}

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override bool Check() {
			int nowvalue = TaskProgress;
			if (nowvalue >= TaskGoal && _finish) {
				if (!completed) {
					SetAsCompleted();
				}
			} else {
				if (completed) {
					completed = false;
					if (OnTaskFail != null) {
						OnTaskFail();
					}
				}
			}
			return completed;
		}

		public override string ProgressStr {
			get { return _finish && TaskProgress >= TaskGoal ? (LString.GAMECLIENT_GAMECRASHCHECKER).ToLocalized() : (LString.GAMECLIENT_GAMECRASHCHECKER_1).ToLocalized(); }
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GameTimeLeft");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			var finish = (bool)args[9];
			if (finish) {
				_finish = true;
				SetStat("GameTimeLeft", (int)args[7]);
			}
			base.OnEvent(eventType, args);
		}
	}
}
