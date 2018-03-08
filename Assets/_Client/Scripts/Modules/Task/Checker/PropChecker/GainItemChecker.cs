//
// GainItemChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
namespace GameClient {
	/// <summary>
	/// 获得指定数量的指定物品
	/// </summary>
	public class GainItemChecker : TaskChecker {
		private ItemType _itemType;

		public override void SetTaskParam(string paramStr) {
			_itemType = (ItemType)int.Parse(paramStr);
		}

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Item_Gain };
			}
		}


		public override int TaskProgress {
			get {
				return GetStat<int>("GainItem" + (int)_itemType);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 2) {
				if ((int)args[0] != (int)_itemType) return;
				SetStat("GainItem" + (int)_itemType, TaskProgress + (int)args[1]);
				base.OnEvent(eventType, args);
			}
		}
	}
}