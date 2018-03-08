
using System;
using System.Collections.Generic;
using System.Linq;
using GameUI;
using XPlugin.Data.Json;

namespace GameClient {
	public class ModChampionship : ClientModule {
		public int CurrentId;//玩家当前正在玩的赛事ID

		public string Rule {
			get {
				return (LString.GAMECLIENT_MODCHAMPIONSHIP).ToLocalized();//TODO 从数据库读取
																		  //return Client.System.GetMiscValue("ChampionshipRule"); 
			}
		}

		/// <summary>
		/// 获取赛事列表
		/// </summary>
		/// <returns></returns>
		public List<ChampionshipInfo> GetChampionshipList() {
			List<ChampionshipInfo> ret = new List<ChampionshipInfo>(Client.User.UserInfo.ChampionshipInfoList.Values);
			ret.Sort((l, r) => l.ChampionshipData.Sort - r.ChampionshipData.Sort);
			return ret;
		}

		/// <summary>
		/// 更新赛事信息
		/// </summary>
		/// <param name="newList"></param>
		public void UpdateList(Dictionary<int, ChampionshipInfo> newList) {
			Dictionary<int, ChampionshipInfo> infoList = Client.User.UserInfo.ChampionshipInfoList;

			//清除过期赛事
			var keyList = infoList.Keys.ToList();
			foreach (var key in keyList) {
				if (!newList.ContainsKey(key)) {
					infoList.Remove(key);
				}
			}

			foreach (var info in newList.Values) {
				if (!infoList.ContainsKey(info.ChampionshipData.Id)) {
					infoList.Add(info.ChampionshipData.Id, info);
				} else {
					infoList[info.ChampionshipData.Id].ChampionshipData = info.ChampionshipData;
					infoList[info.ChampionshipData.Id].Data = info.Data;
					infoList[info.ChampionshipData.Id].MatchMode = MatchMode.Championship;
				}
			}

			SaveData();
		}

		/// <summary>
		/// 是否有赛事正在进行
		/// </summary>
		/// <returns></returns>
		public void HaveChampionship(Action<bool> onDone) {
			Interface.GetCompetitionInfo((b, list) => {
				if (b) {
					if (list.Count > 0) {
						UpdateList(list);
						onDone(true);
					} else {
						onDone(false);
					}
				} else {
					onDone(false);
				}
			});

		}

		public ChampionshipInfo GetChampionshipInfo(int id) {
			ChampionshipInfo ret;
			Client.User.UserInfo.ChampionshipInfoList.TryGetValue(id, out ret);
			return ret;
		}

		public ChampionshipInfo GetCurrentChampionshipInfo() {
			return GetChampionshipInfo(CurrentId);
		}

		/// <summary>
		/// 提交成绩
		/// </summary>
		public void UploadResult(float time) {
			int id = Client.User.UserInfo.Setting.UserId;
			int hero = Client.User.UserInfo.ChoosedHeroID;
			int bike = Client.User.UserInfo.ChoosedBikeID;
			string region = Client.User.UserInfo.Setting.Region.ToString();
			Interface.UpdateCompetitionInfo(id, CurrentId, hero, bike, time, region, (b) => {
				if (!b) {
					CommonDialog.Show((LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT).ToLocalized(), (LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT_1).ToLocalized(), (LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT_2).ToLocalized(), (LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT_3).ToLocalized(), () => {
						UploadResult(time);
					}, null);
				}
			});
		}

		/// <summary>
		/// 获取赛事排行榜
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public void GetRank(int championId, Action<bool, List<ChampionshipRankInfo>, ChampionshipRankInfo> onDone) {
			Interface.GetRanking(Client.User.UserInfo.Setting.UserId, championId, (b, list, self) => {
				if (self != null) {
					GetChampionshipInfo(championId).Rank = self.Rank;
					SaveData();
				}
				onDone(b, list, self);
			});
		}

		/// <summary>
		/// 领取赛事奖励
		/// </summary>
		public void GetRewards(int id, Action<bool, List<RewardItemInfo>> onDone) {

			var info = GetChampionshipInfo(id);
			if (info != null) {
				//检查可领取的奖励
				List<RewardItemInfo> rewardList = new List<RewardItemInfo>();
				for (int i = 0; i < info.TaskResults.Count; i++) {
					if (info.TaskResults[i]) {
						rewardList.Add(info.ChampionshipData.GameReward[i].Reward);
					}
				}

				var rankRewards = info.ChampionshipData.RankReward;
				for (int i = 0; i < rankRewards.Count; i++) {
					if (info.Rank != -1 && info.Rank <= rankRewards[i].Rank) {
						rewardList.Add(rankRewards[i].Reward);
					}
				}

				if (rewardList.Count > 0) {
					//领取物品
					Client.Item.GetRewards(rewardList, false);
					onDone(true, rewardList);
					info.IsGetReward = true;
					SaveData();
				} else {
					onDone(false, null);
				}

			} else {
				onDone(false, null);
			}
		}

		public override void InitInfo(string s) {
			if (!string.IsNullOrEmpty(s)) {
				var list = JsonMapper.ToObject<Dictionary<int, ChampionshipInfo>>(s);
				Client.User.UserInfo.ChampionshipInfoList = list;
			}
		}

		public override string ToJson(UserInfo user) {
			if (user != null) {
				return JsonMapper.ToJson(user.ChampionshipInfoList);
			}

			return JsonMapper.ToJson(Client.User.UserInfo.ChampionshipInfoList);
		}
	}

}
