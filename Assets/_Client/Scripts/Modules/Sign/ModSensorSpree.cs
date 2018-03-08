using System;
using System.Collections.Generic;
using UnityEngine;
using XPlugin.Data.Json;

namespace GameClient {

	public class SensorSpreeInfo {
		/// <summary>
		/// 上一次获取时间
		/// </summary>
		public long LastRecieveTime {
			get { return this._lastRecieveTime; }
			set {
				if (this._lastRecieveTime != value) {
					this._lastRecieveTime = value;
					Client.SensorSpree.SaveData();
				}
			}
		}
		private long _lastRecieveTime = 0;
	}

	public class ModSensorSpree : ClientModule {

		public List<RewardItemInfo> rewardList = new List<RewardItemInfo>();

		public List<RewardItemInfo> Receive() {
			if (!CanReceive()) {
				return null;
			}
			Client.User.UserInfo.SensorSpreeInfo.LastRecieveTime = Client.System.TimeOnLocal;
			Client.Item.GetRewards(rewardList, false);
			return rewardList;
		}

		public bool CanReceive() {
			bool can = false;
			var now = Client.System.DateOnLocal;
			var last = Client.User.UserInfo.SensorSpreeInfo.LastRecieveTime.ToDateTime();
			if (last.Year != now.Year || last.Month != now.Month || last.Day != now.Day) {
				can = true;
			}
			return can;
		}

		public override void InitInfo(string s) {
			rewardList = new List<RewardItemInfo>() {
				new RewardItemInfo(1000,10),
				new RewardItemInfo(1001,600),
				new RewardItemInfo(6001,2),
			};//TODO 应该从数据库读取

			SensorSpreeInfo info = JsonMapper.ToObject<SensorSpreeInfo>(s);
			if (info == null) {
				info = new SensorSpreeInfo();
			}
			Client.User.UserInfo.SensorSpreeInfo = info;
		}

		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.SensorSpreeInfo);
		}
	}
}