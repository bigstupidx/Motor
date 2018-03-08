using System.Collections.Generic;
using XPlugin.Data.Json;
using System;
namespace GameClient {
	public class ModRank : ClientModule {
		public List<RankInfo> RankList = new List<RankInfo>();
		public int CurrentRankID;
		public bool IsLocal = false;

		public RankInfo GetRankInfo() {
			if (RankList == null || RankList.Count == 0) {
				return null;
			}
			CurrentRankID = RankList[0].ID;
			IsLocal = false;
			return RankList[0];

		}

		/// <summary>
		/// 获取排行版的数据
		/// </summary>
		/// <returns>The rank info.</returns>
		/// <param name="rankId">Rank identifier.</param>
		public RankInfo GetRankInfo(int rankId) {
			RankInfo info = null;
			CurrentRankID = rankId;
			foreach (RankInfo rInfo in RankList) {
				if (rInfo.ID == rankId) {
					info = rInfo;
				}
			}
			return info;
		}

		public void GetLocalRankInfo(int rankId, Action<RankInfo> onDone) {
			RankInfo info = null;
			CurrentRankID = rankId;
			foreach (RankInfo rInfo in RankList) {
				if (rInfo.ID == rankId) {
					info = rInfo;
				}
			}
			if (IsLocal && info.LocalListItems.Count == 0) {
				int userId = Client.User.UserInfo.Setting.UserId;
				string regionId = Client.User.UserInfo.Setting.Region.ToString();
				Interface.GetRankingByCountry(userId, CurrentRankID, regionId,
					(b, list, self) => {
						if (b) {
							info.LocalListItems = list;
							info.LocalSelfData = self;
							if (onDone != null) {
								onDone(info);
							}
						} else {
							if (onDone != null) {
								onDone(info);
							}
						}
					});
			} else {
				if (onDone != null) {
					onDone(info);
				}
			}
		}

		/// <summary>
		/// 获取排行版的标题信息
		/// </summary>
		/// <returns>The rank item list.</returns>
		public List<RankItemInfo> GetRankItemList() {
			List<RankItemInfo> itemInfoList = new List<RankItemInfo>();
			foreach (RankInfo info in RankList) {
				itemInfoList.Add(new RankItemInfo(info.ID, info.Name));
			}
			return itemInfoList;
		}

		public void UpdateRankData(JArray data) {

			RankList.Clear();
			CurrentRankID = -1;
			foreach (JArray jObj in data) {
				RankInfo rankInfo = new RankInfo(jObj);
				RankList.Add(rankInfo);
				if (CurrentRankID == -1) {
					CurrentRankID = rankInfo.ID;
				}
			}
		}

		public void GetRank(Action<bool> onDone) {
			Interface.GetAllRanking(Client.User.UserInfo.Setting.UserId, (b, list) => {
				if (list != null) {
					UpdateRankData(list);
				}
				if (onDone != null) {
					onDone(b);
				}
			});
		}

	}

}
