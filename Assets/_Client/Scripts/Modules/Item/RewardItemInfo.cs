//
// ItemInfo.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XPlugin.Data.Json;

namespace GameClient {
	public class RewardItemInfo : ItemInfo {
		public ItemInfo OriginItem = null;

		public RewardItemInfo(int itemID, int amount = 0)
			: base(itemID, amount) {
		}

		public RewardItemInfo(ItemData data, int amount = 0)
			: base(data, amount) {
		}

	}
}
