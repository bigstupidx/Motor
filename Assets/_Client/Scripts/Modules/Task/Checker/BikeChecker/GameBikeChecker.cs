//
// GameBikeChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 使用指定车辆完成指定次数的游戏
	/// </summary>
	public class GameBikeChecker : TaskChecker {
		private int _bikeID;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_End };
			}
		}

		public override void SetTaskParam(string paramStr) {
			_bikeID = int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GameBike" + (int)_bikeID);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			var finish = (bool)args[9];
			if (finish) {
				if ((int)args[5] != (int)_bikeID) return;
				SetStat("GameBike" + (int)_bikeID, TaskProgress + 1);
				base.OnEvent(eventType, args);
			}
		}
	}
}