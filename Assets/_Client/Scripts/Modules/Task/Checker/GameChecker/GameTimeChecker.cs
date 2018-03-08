//
// GameTimeChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 指定时间内完成比赛
	/// </summary>
	public class GameTimeChecker : TaskLessChecker {
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
				var value = GetStat<int>("GameTime");
				if (value == 0) {
					value = TaskGoal + 1;
					SetStat("GameTime", value);
				}
				return value;
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			var finish = (bool)args[9];
			if (finish) {
				SetStat("GameTime", (int)args[7]);
			}
			base.OnEvent(eventType, args);
		}
	}
}
