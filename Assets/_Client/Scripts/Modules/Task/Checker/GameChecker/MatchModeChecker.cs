//
// GameModeChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 完成指定数量的指定模式游戏
	/// </summary>
	public class MatchModeChecker : TaskChecker {
		private MatchMode _mode;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override void SetTaskParam(string paramStr) {
			_mode = (MatchMode)int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("MatchMode" + (int)_mode);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			var finish = (bool)args[9];
			if (finish) {
				if ((int)args[10] != (int)_mode) {
					return;
				}
				SetStat("MatchMode" + (int)_mode, TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}