//
// GameHeroChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 使用指定英雄完成指定次数的游戏
	/// </summary>
	public class GameHeroChecker : TaskChecker {
		private int _heroID;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override void SetTaskParam(string paramStr) {
			_heroID = int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GameHero" + (int)_heroID);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			var finish = (bool)args[9];
			if (finish) {
				if ((int)args[4] != (int)_heroID) return;
				SetStat("GameHero" + (int)_heroID, TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}