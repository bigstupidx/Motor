//
// UsePropChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {

	/// <summary>
	/// 使用指定数量的道具
	/// </summary>
	public class UsePropChecker : TaskChecker {
		private PropType type;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_UseProp };
			}
		}

		public override void SetTaskParam(string paramStr) {
			type = (PropType)int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("UseProp" + (int)type);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 2) {
				if ((int)args[0] != (int)type) return;
				SetStat("UseProp" + (int)type, TaskProgress + (int)args[1]);
				base.OnEvent(eventType, args);
			}
		}
	}
}