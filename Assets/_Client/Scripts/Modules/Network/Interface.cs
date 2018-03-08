using System;
using UnityEngine;
using System.Collections.Generic;
using GameClient;
using XPlugin.Data.Json;

public class Interface : Singleton<Interface> {
	public string FullApiUrl;
	public bool FakeNoConnection = false;

	public string ApiPath = "/index.php/Api/Index/index";

	void Start() {
		this.FullApiUrl = Client.Config.WebHost + this.ApiPath;
	}

	public static bool isNetworkConnected() {
		if (Ins.FakeNoConnection) {
			return false;
		} else {
			return SDKManager.Instance.isNetworkConnected();
		}
	}
	#region Server

	/// <summary>
	/// 获取配置信息
	/// </summary>
	public static void GetServerInfo(int channelId, int versionId, int provinceId, int cityId, int networkId,
		Action<bool> ondone = null) {
		if (!isNetworkConnected()) {
			if (ondone != null) {
				ondone(false);
			}
			return;
		}

		WebManager.Ins.AddItem(new WebItem() {
			M = "System",
			A = "GetServerInfo",
			P = new JArray(new[] { channelId, versionId, provinceId, cityId, networkId }),
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("" + callback.Success + " " + callback.CallBackType + " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					if (root["code"].AsInt() == 0) {
						JArray results = (JArray)root["result"];
						Client.Spree.UpdateSpreeDatas((JArray)results[0]);
						Client.Spree.UpdateSpreeShowDatas((JArray)results[1]);

						JArray configArray = (JArray)results[2];
						Client.Sign.IsSignOpen = ((int)configArray[0]) == 1 ? true : false;
						Client.Spree.ShowRedeemCode = ((int)configArray[1]) == 1 ? true : false;
						Client.Spree.ShowSpree = ((int)configArray[2]) == 1 ? true : false;
						Client.Spree.AutoBuy = ((int)configArray[3]) == 1 ? true : false;
						Client.IAP.BuyConfirm = ((int)configArray[4]) == 1 ? true : false;
						Client.Spree.ShowFirstSpree = ((int)configArray[5]) == 1 ? true : false;
						Client.Spree.FirstCostInServer = ((int)configArray[6]);
						Client.Prop.GamingBuy = ((int)configArray[7]) == 1 ? true : false;
						//						Client.System.IsShowAbout = ((int)configArray[8]) == 1 ? true : false;//关于从本地配置文件读取
						Client.Config.ShowService = ((int)configArray[9]) == 1 ? true : false;

						Client.System.UpdateServerTime((long)results[3]);

						if (ondone != null) {
							ondone(true);
						}
					} else {
						if (ondone != null) {
							ondone(false);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false);
					}
					Debug.LogError("GetServerInfo request fail " + callback.Success + " " + callback.CallBackType + " content:" +
								   callback.content);
				}
			}
		});

	}

	public static void GetServerTime(Action<bool> ondone = null) {
		if (!isNetworkConnected()) {
			if (ondone != null) {
				ondone(false);
			}
			return;
		}

		WebManager.Ins.AddItem(new WebItem() {
			A = "GetTime",
			P = new JArray(),
			M = "Activity",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("" + callback.Success + " " + callback.CallBackType + " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					if (root["code"].AsInt() == 0) {
						JObject result = (JObject)root["result"];

						Client.System.UpdateServerTime((long)result["Time"]);

						if (ondone != null) {
							ondone(true);
						}
					} else {
						if (ondone != null) {
							ondone(false);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false);
					}
					Debug.LogError("GetServerInfo request fail " + callback.Success + " " + callback.CallBackType + " content:" +
								   callback.content);
				}
			}
		});

	}

	#endregion

	#region Setting

	/// <summary>
	/// 获取位置配置信息
	/// </summary>
	/// <param name="channelId">Channel identifier.</param>
	/// <param name="versionId">Version identifier.</param>
	/// <param name="provinceId">Province identifier.</param>
	/// <param name="cityId">City identifier.</param>
	/// <param name="networkId">Network identifier.</param>
	/// <param name="ondone">Ondone.</param>
	public static void GetPlace(int channelId, int versionId, int provinceId, int cityId, int networkId,
		Action<bool> ondone = null) {

		if (!isNetworkConnected()) {
			if (ondone != null) {
				ondone(false);
			}
			return;
		}

		WebManager.Ins.AddItem(new WebItem() {
			A = "GetPlace",
			P = new JArray(new[] { channelId, versionId, provinceId, cityId, networkId }),
			M = "Spree",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("" + callback.Success + " " + callback.CallBackType + " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					if (root["code"].AsInt() == 0) {
						Client.Spree.UpdateSpreeShowDatas((JArray)root["result"]);
						if (ondone != null) {
							ondone(true);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false);
					}
					Debug.LogError("GetServerInfo request fail " + callback.Success + " " + callback.CallBackType + " content:" +
								   callback.content);
				}
			}
		});

	}

	#endregion

	#region Competition
	/// <summary>
	/// 获取锦标赛列表
	/// </summary>
	/// <param name="ondone"></param>
	public static void GetCompetitionInfo(Action<bool, Dictionary<int, ChampionshipInfo>> ondone = null) {
		WebManager.Ins.AddItem(new WebItem() {
			A = "GetCompetition",
			P = new JArray(new[] { Client.Config.VersionId }),
			M = "Activity",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("<color=yellow>[GetCompetitionInfo]</color>" + callback.Success + " " + callback.CallBackType + " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					var result = new Dictionary<int, ChampionshipInfo>();
					JObject root = JObject.Parse(callback.content);
					if (root["code"].AsInt() == 0) {
						var championList = (JArray)((JArray)root["result"])[0];
						for (int i = 0; i < championList.Count; i++) {
							var data = (JObject)championList[i];
							// 比赛基础数据
							var matachData = new MatchData {
								Name = data["Name"].AsString(),
								NeedHero = Client.Hero[data["LimitHero"].AsInt()],
								NeedStamina = data["NeedStamina"].AsInt(),
								Turn = data["Turn"].AsInt(),
								GameMode = (GameMode)data["GameMode"].AsInt(),
								Scene = Client.Scene[data["SceneID"].AsInt()],
								RaceLine = data["RaceLine"].AsInt().ToString(),
								ObjLine = data["ObjLine"].AsString(),
								TimeLimit = data["UseTime"].AsInt() == 0 ? 60 : data["UseTime"].AsInt()
							};
							// 敌人
							var enemies = new List<MatchEnemyData>();
							var enemiesData = (JArray)data["Enemies"];
							foreach (var item in enemiesData) {
								var matchEnemyData = new MatchEnemyData {
									Bike = Client.Bike[item["BikeID"].AsInt()],
									BikeLv = item["BikeLV"].AsInt(),
									Hero = Client.Hero[item["HeroID"].AsInt()],
									HeroLv = item["HeroLV"].AsInt(),
									Weapon = Client.Weapon[item["WeaponID"].AsInt()],
									Prop = Client.Prop[item["PropID"].AsInt()],
									AI = item["AI"].AsInt()
								};
								enemies.Add(matchEnemyData);
							}
							matachData.EnemyDatas = enemies;
							var info = new ChampionshipInfo {
								Data = matachData,
								MatchMode = MatchMode.Championship
							};
							// 赛事
							var championshipData = new ChampionshipData {
								Id = data["ID"].AsInt(),
								Icon = Client.Icon[data["IconID"].AsInt()],
								Description = data["Description"].AsString(),
								StartTime = data["StartTime"].AsLong(),
								FinishTime = data["FinishTime"].AsLong(),
								Sort = data["Sort"].AsInt()
							};
							// 奖励
							var gameReward = new List<ChampionshipRewardData>();
							var rankReward = new List<ChampionshipRewardData>();
							var rewardData = (JArray)data["Award"];
							foreach (var item in rewardData) {
								var condition = item["Condition"].AsInt();
								var reward = new ChampionshipRewardData {
									Rank = item["Ranking"].AsInt(),
									Reward = new RewardItemInfo(item["ItemID"].AsInt(), item["ItemNum"].AsInt()),
									Condition = condition
								};

								if (condition == 0) {
									rankReward.Add(reward);
								} else {
									gameReward.Add(reward);
								}
							}
							gameReward.Sort((r1, r2) => r1.Condition - r2.Condition);
							rankReward.Sort((r1, r2) => r1.Rank - r2.Rank);
							championshipData.GameReward = gameReward;
							championshipData.RankReward = rankReward;

							//车辆限制
							championshipData.LimitBikeType = (LimitBikeType)data["LimitBikeType"].AsInt();
							if (championshipData.LimitBikeType == LimitBikeType.ID) {
								info.Data.NeedBike = Client.Bike[Convert.ToInt32(data["LimitBikeValue"].AsString())];
							} else {
								championshipData.LimitBikeRank = data["LimitBikeValue"].AsString();
							}

							info.ChampionshipData = championshipData;
							result.Add(info.ChampionshipData.Id, info);
						}

						Client.System.UpdateServerTime((long)((JArray)root["result"])[1]);

						if (ondone != null) {
							ondone(true, result);
						}
					} else {
						if (ondone != null) {
							ondone(false, null);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false, null);
					}
					Debug.LogError("<color=red>[GetCompetitionInfo]</color> request fail " + callback.Success + " " + callback.CallBackType + " content:" +
								   callback.content);
				}
			}
		});
	}

	public static void IsInTouchVip(RegionEnum region, Action<bool, WebItem> ondone) {
		WebManager.Ins.AddItem(new WebItem {
			M = "Activity",
			A = "IsInTOuchVip",
			P = new JArray { (int)region },
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("<color=yellow>[IsInTouchVip]</color>" + callback.Success + " " + callback.CallBackType +
							  " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					var code = root["code"].AsInt();
					if (code == 0) {
						var array = root["result"].AsArray();
						ondone(array[0].OptBool(), callback);
					} else {
						if (ondone != null) {
							ondone(false, callback);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false, callback);
					}
				}
			}
		});
	}

	/// <summary>
	/// 更新玩家赛事数据
	/// </summary>
	/// <param name="playerID"></param>
	/// <param name="actID"></param>
	/// <param name="heroID"></param>
	/// <param name="bikeID"></param>
	/// <param name="time"></param>
	/// <param name="ondone"></param>
	public static void UpdateCompetitionInfo(int playerID, int actID, int heroID, int bikeID, float runTime, string region, Action<bool> ondone = null) {
		WebManager.Ins.AddItem(new WebItem() {

			A = "UpdateCompetitionInfo",
			P = new JArray { playerID, actID, heroID, bikeID, runTime, region },
			M = "Activity",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("<color=yellow>[UpdateCompetitionInfo]</color>" + callback.Success + " " + callback.CallBackType +
							  " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					var code = root["code"].AsInt();
					if (code == 0) {
						if (ondone != null) {
							ondone(true);
						}
					} else {
						if (ondone != null) {
							ondone(false);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false);
					}
				}
			}
		});
	}

	/// <summary>
	/// 获取排行榜
	/// </summary>
	/// <param name="playerID"></param>
	/// <param name="ondone"></param>
	public static void GetRanking(int playerID, int actID, Action<bool, List<ChampionshipRankInfo>, ChampionshipRankInfo> ondone = null) {
		WebManager.Ins.AddItem(new WebItem() {
			A = "GetRanking",
			P = new JArray(new[] { playerID, actID }),
			M = "Activity",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("<color=yellow>[GetRanking]</color>" + callback.Success + " " + callback.CallBackType +
							  " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					var code = root["code"].AsInt();
					if (code == 0) {
						var rankInfoList = new List<ChampionshipRankInfo>();
						var array = root["result"].AsArray();
						var list = array[0].OptArray();
						foreach (var item in list) {
							if (item.Count == 0) continue;
							var id = item["player_id"].AsInt();
							var rankInfo = new ChampionshipRankInfo {
								Rank = item["ranking"].AsInt(),
								PlayerID = id,
								NickName =
								id == Client.User.UserInfo.Setting.UserId
								? Client.User.UserInfo.Setting.Nickname
								: (item["nickname"].IsValid ? item["nickname"].AsString() : (LString.ISNETWORKCONNECTED).ToLocalized()),
								Hero = Client.Hero[item["hero_id"].AsInt()],
								Bike = Client.Bike[item["bike_id"].AsInt()],
								RunTime = item["record_time"].AsFloat()
							};
							rankInfoList.Add(rankInfo);
						}
						// 读取玩家排行
						JObject self = array[1].OptObject(new JObject());
						ChampionshipRankInfo selfInfo = null;
						if (self.Count > 0) {
							selfInfo = new ChampionshipRankInfo {
								Rank = self["ranking"].AsInt(),
								PlayerID = self["player_id"].AsInt(),
								NickName = Client.User.UserInfo.Setting.Nickname,
								Hero = Client.Hero[self["hero_id"].AsInt()],
								Bike = Client.Bike[self["bike_id"].AsInt()],
								RunTime = self["record_time"].AsFloat()
							};
						}
						if (ondone != null) {
							ondone(true, rankInfoList, selfInfo);
						}
					} else {
						if (ondone != null) {
							ondone(false, null, null);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false, null, null);
					}
				}
			}
		});
	}

	/// <summary>
	/// 获取当地排行榜
	/// </summary>
	/// <param name="playerID"></param>
	/// <param name="ondone"></param>
	public static void GetRankingByCountry(int playerID, int actID, string region, Action<bool, List<RankData>, RankData> ondone = null) {
		WebManager.Ins.AddItem(new WebItem() {
			A = "GetRankingByCountry",
			P = new JArray { playerID, actID, region },
			M = "Activity",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("<color=yellow>[GetRankingByCountry]</color>" + callback.Success + " " + callback.CallBackType +
							  " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					var code = root["code"].AsInt();
					if (code == 0) {
						var rankInfoList = new List<RankData>();
						var array = root["result"].AsArray();
						var list = array[0].OptArray();
						foreach (var item in list) {
							if (item.Count == 0) continue;
							var id = item["player_id"].AsInt();
							var rankInfo = new RankData {
								Rank = item["ranking"].AsInt(),
								PlayerID = id,
								NickName =
								id == Client.User.UserInfo.Setting.UserId
								? Client.User.UserInfo.Setting.Nickname
								: (item["nickname"].IsValid ? item["nickname"].AsString() : (LString.ISNETWORKCONNECTED).ToLocalized()),
								Hero = Client.Hero[item["hero_id"].AsInt()],
								Bike = Client.Bike[item["bike_id"].AsInt()],
								RunTime = item["record_time"].AsFloat(),
								Region = (RegionEnum)Enum.Parse(typeof(RegionEnum), item["country"].AsString(), true)
							};
							rankInfoList.Add(rankInfo);
						}
						// 读取玩家排行
						JObject self = array[1].OptObject(new JObject());
						RankData selfInfo = null;
						if (self.Count > 0) {
							selfInfo = new ChampionshipRankInfo {
								Rank = self["ranking"].AsInt(),
								PlayerID = self["player_id"].AsInt(),
								NickName = Client.User.UserInfo.Setting.Nickname,
								Hero = Client.Hero[self["hero_id"].AsInt()],
								Bike = Client.Bike[self["bike_id"].AsInt()],
								RunTime = self["record_time"].AsFloat(),
								Region = (RegionEnum)Enum.Parse(typeof(RegionEnum), self["country"].AsString(), true)
							};
						}
						if (ondone != null) {
							ondone(true, rankInfoList, selfInfo);
						}
					} else {
						if (ondone != null) {
							ondone(false, null, null);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false, null, null);
					}
				}
			}
		});
	}

	/// <summary>
	/// 获取排行榜
	/// </summary>
	/// <param name="playerID"></param>
	/// <param name="ondone"></param>
	public static void GetAllRanking(int playerID, Action<bool, JArray> ondone = null) {
		WebManager.Ins.AddItem(new WebItem() {
			A = "GetAllRanking",
			P = new JArray(new[] { playerID, Client.Config.VersionId }),
			M = "Activity",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("<color=yellow>[GetAllRanking]</color>" + callback.Success + " " + callback.CallBackType +
							  " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					var code = root["code"].AsInt();
					if (code == 0) {
						JArray array = root["result"].AsArray();
						if (ondone != null) {
							ondone(true, array);
						}

					} else {
						if (ondone != null) {
							ondone(false, null);
						}
					}
				} else {
					if (ondone != null) {
						ondone(false, null);
					}
				}
			}
		});
	}

	#endregion
}
