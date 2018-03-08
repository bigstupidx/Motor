//
// PassAccelerateFieldChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 通过加速带达到指定次数
	/// </summary>
	public class PassAccelerateFieldChecker : TaskChecker {
		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_PassAccelerateField };
			}
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("PassAccelerateField");
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 1) {
				SetStat("PassAccelerateField", TaskProgress + (int)args[0]);
				base.OnEvent(eventType, args);
			}
		}
	}
}

