using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModSystem : ClientModule {
		private double _timeOnServer { get; set; }

		public void UpdateServerTime(long time) {
			this._timeOnServer = time;
		}

		public long TimeOnServer {
			get { return (long)this._timeOnServer; }
		}

		public DateTime DateOnServer {
			get { return this.TimeOnServer.ToDateTime(); }
		}

		/// <summary>
		/// 当前时间戳（本地）
		/// </summary>
		public long TimeOnLocal {
			get { return DateTime.Now.ToTimeStamp(); }
		}

		/// <summary>
		/// 当前时间（本地）
		/// </summary>
		public DateTime DateOnLocal {
			get { return DateTime.Now; }
		}

		public override void InitInfo(string s) {
			base.InitInfo(s);
			StartCoroutine(CheckCrossDay());
		}

		IEnumerator CheckCrossDay() {
			DateTime last = DateOnLocal;
			while (true) {
				yield return new WaitForSeconds(2);
				var now = DateOnLocal;
				if (last.Year != now.Year || last.Month != now.Month || last.Day != now.Day) {
					Debug.Log("跨天了");
					Client.EventMgr.SendEvent(EventEnum.System_OverDay);
				}
				last = now;
			}
		}

		protected override void Update() {
			base.Update();
			_timeOnServer += Time.unscaledDeltaTime;
		}

		protected Dictionary<string, string> miscDict = new Dictionary<string, string>();

		public string GetMiscValue(string key) {
			if (miscDict.ContainsKey(key)) {
				return miscDict[key];
			} else {
				return "";
			}
		}

		public T GetMiscValue<T>(string key) {
			string str = GetMiscValue(key);
			try {
				return (T)Convert.ChangeType(str, typeof(T));
			} catch (Exception) {
				Debug.LogError("MiscValue : " + key + "not found!");
				return default(T);

			}
		}

		public override void InitData(DbAccess db) {
			var reader = db.ReadFullTable("Misc");
			while (reader.Read()) {
				miscDict.Add((string)reader.GetValue("Key"), (string)reader.GetValue("Value"));
			}
			Client.Log("[Client] Use Data Version = " + GetMiscValue<int>("Version"));
		}

		public override void ResetData() {
			miscDict.Clear();
		}

		/// <summary>
		/// 客服信息
		/// </summary>
		public string ServiceInfo {
			get { return GetMiscValue<string>("Service.Info"); }
		}

		public string AboutInfo {
			get { return GetMiscValue<string>("Service.About"); }
		}
	}

}

