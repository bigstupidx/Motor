
using System;
using System.Collections.Generic;
using GameUI;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;
using XPlugin.Update;

namespace GameClient {
	public enum SignState {
		None = -1,
		Normal = 0,
		Already = 1,
		Current = 2
	}

	public class ModSign : ClientModule {

		[NonSerialized]
		public bool IsSignOpen = true;

		protected Dictionary<int, SignData> SignDataList = new Dictionary<int, SignData>();

		public SignData this[int day] {
			get {
				if (day > 6) {
					day = 6;
				} else if (day < 0) {
					day = 0;
				}

				SignData data;
				SignDataList.TryGetValue(day, out data);
				return data;
			}
		}

		public Texture AdPic {
			get {
				string name = Client.System.GetMiscValue("Sign.AdPic");
				return UResources.Load<Texture>(name);
			}
		}

		public string SignHelp {
			get { return Client.System.GetMiscValue("Sign.Help"); }
		}

		public List<RewardItemInfo> SignToday() {
			if (!CanSign()) {
				return null;
			}
			Client.User.UserInfo.Sign.LastSignTime = Client.System.TimeOnLocal;
			Client.User.UserInfo.Sign.Schedule += 1;
			List<RewardItemInfo> list = new List<RewardItemInfo>() { this[Client.User.UserInfo.Sign.Schedule].Reward };
			Client.Item.GetRewards(list, false);
			Client.EventMgr.SendEvent(EventEnum.System_SignToday, Client.User.UserInfo.Sign.Schedule);
			return list;
		}

		public bool CanSign() {
			bool canSign = false;
			var now = Client.System.DateOnLocal;
			var last = Client.User.UserInfo.Sign.LastSignTime.ToDateTime();
			if (last.Year != now.Year || last.Month != now.Month || last.Day != now.Day) {
				canSign = true;
			}
			return canSign;
		}

		public bool IsSignToday() {
			bool isSign = false;
			var now = Client.System.DateOnLocal;
			var last = Client.User.UserInfo.Sign.LastSignTime.ToDateTime();
			if (last.Year == now.Year && last.Month == now.Month && last.Day == now.Day) {
				isSign = true;
			}
			return isSign;
		}

		public int Schedule {
			get {
				var now = Client.System.DateOnLocal;
				var last = Client.User.UserInfo.Sign.LastSignTime.ToDateTime();
				var deadLine = new DateTime(last.Year, last.Month, last.Day, 0, 0, 0).AddDays(2);
				if (now < last || now > deadLine) {
					Client.User.UserInfo.Sign.Schedule = -1;
				}
				Debug.Log("Sign Schedule "+ Client.User.UserInfo.Sign.Schedule);
				return Client.User.UserInfo.Sign.Schedule;
			}
		}

		public override void InitData(DbAccess db) {
			var reader = db.ReadFullTable("Sign");
			while (reader.Read()) {
				SignData data = new SignData(reader);
				SignDataList.Add(data.Day, data);
			}
		}

		public override void InitInfo(string s) {
			SignInfo info = JsonMapper.ToObject<SignInfo>(s);
			if (info == null) {
				info = new SignInfo();
			}
			Client.User.UserInfo.Sign = info;
			Client.EventMgr.AddListener(EventEnum.System_OverDay, (e, args) => {
				if (SignBoard.Ins != null) {
					SignBoard.Ins.Init();
				}
			});
		}

		public override string ToJson(UserInfo user) {
			return JsonMapper.ToJson(user.Sign);
		}
	}

}

