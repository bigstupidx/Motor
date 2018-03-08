//
// GainPropChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
namespace GameClient {
	/// <summary>
	/// 获得指定数量的道具
	/// </summary>
	public class GainPropChecker : TaskChecker {
		private PropType _propType;

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_GainProp };
			}
		}

		public override void SetTaskParam(string paramStr) {
			_propType = (PropType)int.Parse(paramStr);
		}

		public override int TaskProgress {
			get {
				return GetStat<int>("GainProp" + (int)_propType);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 2) {
				if ((int)args[0] != (int)_propType) return;
				SetStat("GainProp" + (int)_propType, TaskProgress + (int)args[1]);
				base.OnEvent(eventType, args);
			}
		}
	}
}