//
// BikeProp.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections.Generic;
using GameClient;
using GameUI;
using RoomServerModel;

namespace Game {
	public class BikeProp : BikeBase {

		public List<PropBase> PropSlots = new List<PropBase>();
		public static readonly Dictionary<PropType, PropBase> PropDic = new Dictionary<PropType, PropBase>() {
			{PropType.Energy, new PropEnergy()},
			{PropType.Missile, new PropMissile()},
			{PropType.Shield, new PropShield()},
		};

		private int _propSlotsCount = 1;

		private void __Use(PropType type) {
			Debug.Log("->>>>----- Use prop"+this.bikeNetwork.Id);
			PropBase prop = null;
			switch (type) {
				case PropType.Missile:
					prop = new PropMissile();
					break;
				case PropType.Shield:
					prop = new PropShield();
					break;
				case PropType.Energy:
					prop = new PropEnergy();
					break;
				default:
					throw new ArgumentException("不支持的道具");
			}
			prop.Use(this);
		}

		public bool Use(int index = 0) {
			if (!bikeHealth.IsAlive) {
				return false;
			}
			if (PropSlots.Count < 1) {
				return false;
			}
			var prop = PropSlots[index];
			var result = prop.Use(this);
			if (result) {
				if (gameObject.CompareTag(Tags.Ins.Player)) {
					Client.EventMgr.SendEvent(EventEnum.Game_UseProp, prop.Type, 1);
					// 消耗道具
					var count = info.GetStat<int>("PropType" + (int)prop.Type);
					info.SetStat("PropType" + (int)prop.Type, count - 1);
				}
				PropSlots.RemoveAt(index);
			}
			bikeNetwork.Rpc(BroadcastType.Others, "__Use", prop.Type);
			return result;
		}

		public bool Use(PropType type) {
			if (!bikeHealth.IsAlive) {
				return false;
			}
			PropBase prop;
			var result = false;
			if (PropDic.TryGetValue(type, out prop)) {
				result = prop.Use(this);
			}

			if (result) {
				if (gameObject.CompareTag(Tags.Ins.Player)) {
					Client.EventMgr.SendEvent(EventEnum.Game_UseProp, type, 1);
				}
			}
			// else Debug.LogError(name + " Prop " + type + " use failed..");
			bikeNetwork.Rpc(BroadcastType.Others, "__Use", type);
			return result;
		}

		public void Gain(PropType type) {
			PropBase prop;
			if (!PropDic.TryGetValue(type, out prop)) {
				Debug.LogError(name + " Prop " + type + " not found.");
				return;
			}
			// 覆盖
			if (PropSlots.Count >= _propSlotsCount) {
				PropSlots.Remove(PropSlots[PropSlots.Count - 1]);
			}
			PropSlots.Add(prop);


			if (gameObject.CompareTag(Tags.Ins.Player)) {
				GamePlayBoard.Ins.ShowPropGetFromGame(type);
				Client.EventMgr.SendEvent(EventEnum.Game_GainProp, type, 1);
				// 获得
				var count = info.GetStat<int>("PropType" + (int)prop.Type);
				info.SetStat("PropType" + (int)prop.Type, count + 1);
			}

		}
	}
}

