using UnityEngine;
using System.Collections.Generic;
using GameUI;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;
namespace GameClient {
	public class ModChallenge : ClientModule {
		public string Rule {
			get {
				return (LString.GAMECLIENT_MODCHALLENGE).ToLocalized();//TODO 从数据库读取
			}
		}
		// 关卡列表
		protected Dictionary<int, ChallengeData> matchList = new Dictionary<int, ChallengeData>();

		public override void InitData(DbAccess db) {
			var reader = db.ReadFullTable("Challenge");
			while (reader.Read()) {
				ChallengeData data = new ChallengeData(reader);
				matchList.Add(data.ID, data);
			}
		}

		public List<ChallengeData> GetSortedMatchDatas() {
			List<ChallengeData> ret = new List<ChallengeData>(this.matchList.Values);
			ret.Sort((l, r) => {
				return l.LevelBike - r.LevelBike;
			});
			return ret;
		}

		/// <summary>
		/// 获取关卡信息
		/// </summary>
		/// <param name="MatchId"></param>
		/// <returns></returns>
		public ChallengeInfo GetChallengeMatchInfo(int MatchId) {
			ChallengeInfo ret = null;
			if (!Client.User.UserInfo.ChallengeInfoList.TryGetValue(MatchId, out ret)) {//如果没有找到关卡信息则添加一个
				Client.Log("未找到挑战关卡：" + MatchId);
				ret = new ChallengeInfo(this.matchList[MatchId]);
				Client.User.UserInfo.ChallengeInfoList.Add(MatchId, ret);
			}
			return ret;
		}

		public bool NeedDailyReset() {
			var now = Client.System.DateOnLocal;
			var last = Client.User.UserInfo.LastResetChallengeTime.ToDateTime();
			return last.Year != now.Year || last.Month != now.Month || last.Day != now.Day;
		}

		public override void InitInfo(string s) {
			if (string.IsNullOrEmpty(s)) {
				DailyReset();
			} else {
				JArray arr = JArray.Parse(s);
				var user = Client.User.UserInfo;
				user.LastResetChallengeTime = arr[0].AsLong();
				if (NeedDailyReset()) {
					DailyReset();
				} else {
					// 恢复保存的关卡完成度
					var ChallengeInfos = JsonMapper.ToObject<Dictionary<int, ChallengeInfo>>(arr[1].AsString());
					if (ChallengeInfos != null) {
						var list = Client.User.UserInfo.ChallengeInfoList;
						list.Clear();
						foreach (var challenge in ChallengeInfos) {
							var info = challenge.Value;
							info.Data = matchList[challenge.Key];
							info.MatchMode = MatchMode.Challenge;
							list.Add(challenge.Key, info);
						}
					}
				}

			}
			Client.EventMgr.AddListener(EventEnum.System_OverDay, (e, args) => DailyReset());
		}

		//每日更新
		public void DailyReset() {
			Client.Log("[Client] Challenge ： Reset Challenge.!!!!!!!!!!!!!!!");
			Client.User.UserInfo.LastResetChallengeTime = Client.System.TimeOnLocal;
			var list = Client.User.UserInfo.ChallengeInfoList;
			list.Clear();
			foreach (var data in this.matchList.Values) {
				ChallengeInfo info = new ChallengeInfo(data);
				info.LastTime = Client.System.TimeOnServer;
				list.Add(data.ID, info);
			}
			Client.Challenge.SaveData();

			if (ChallengeBoard.Ins != null) {
				ChallengeBoard.Ins.Reload();
			}
		}

		public override string ToJson(UserInfo user) {
			JArray arr = new JArray();
			arr.Add(user.LastResetChallengeTime);
			arr.Add(JsonMapper.ToJson(user.ChallengeInfoList));
			return arr.ToString();
		}
	}
}
