//
// GameCrashChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 在一定毁坏次数内完成比赛
	/// </summary>
	public class GameCrashChecker : TaskLessChecker {
		private bool _init = false;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override string ProgressStr {
			get { return TaskProgress <= TaskGoal ? (LString.GAMECLIENT_GAMECRASHCHECKER).ToLocalized() : (LString.GAMECLIENT_GAMECRASHCHECKER_1).ToLocalized(); }
		}

		public override int TaskProgress {
			get {
				var value = GetStat<int>("GameCrash");
				if (value == 0 && !_init) {
					_init = true;
					value = TaskGoal + 1;
					SetStat("GameCrash", value);
				}
				return value;
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 9) {
				SetStat("GameCrash", (int)args[8]);
				base.OnEvent(eventType, args);
			}
		}
	}
}
