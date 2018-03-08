//
// ModAnalytics.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameClient {
	public class ModAnalytics : ClientModule {
		void OnEvent(EventEnum eventType, object[] param) {
			if (enabled) {
				Analytics(eventType, param);
			}
		}

		void Awake() {
			AnalyticsMgrBase.Init(this);
		}

		public void StartAnalytics() {
			AnalyticsMgrBase.Ins.OnStart();
		}

		protected override void Update() {
			base.Update();
			AnalyticsMgrBase.Ins.OnUpdate();
		}

		void OnDestroy() {
			AnalyticsMgrBase.Ins.OnEnd();
		}

		void OnApplicationPause(bool pause) {
			if (!Client.IsInited) {
				return;
			}

			if (!pause) {
				AnalyticsMgrBase.Ins.OnStart();
			} else {
				AnalyticsMgrBase.Ins.OnEnd();
			}
		}

		public void OnEvent(string eventID) {
			AnalyticsMgrBase.Ins.Event(eventID);
		}

		void Analytics(EventEnum eventType, object[] param) {
			switch (eventType) {
				case EventEnum.System_ClientInited:
					AnalyticsMgrBase.Ins.SetAccountID(SDKManager.Instance.GetUID());
					AnalyticsMgrBase.Ins.SetAccountName(Client.User.UserInfo.Setting.Nickname);
					return;
			}

			if (!Client.Ins.Inited) {
				return;
			}

			try {
				switch (eventType) {
					#region System

					case EventEnum.System_SignToday:
						AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("Schedule", (int)param[0]));
						break;

					#endregion

					#region Player

					case EventEnum.Player_ChangeNickName:
						AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("NickNmae", param[0].ToString()));
						break;

					#endregion

					#region Hero

					case EventEnum.Hero_Unlock: {
							var heroID = (int)param[0];
							var heroData = Client.Hero[heroID];
							AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("HeroID", heroID + " : " + heroData.Name));
						}

						break;
					case EventEnum.Hero_Upgrade: {
							var heroID = (int)param[0];
							var heroData = Client.Hero[heroID];
							AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("HeroID", heroID + " : " + heroData.Name));
						}
						break;

					#endregion

					#region Bike

					case EventEnum.Bike_Unlock: {
							var bikeID = (int)param[0];
							var bikeData = Client.Bike[bikeID];
							AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("BikeID", bikeID + " : " + bikeData.Name));
						}
						break;
					case EventEnum.Bike_Upgrade: {
							var bikeID = (int)param[0];
							var bikeData = Client.Bike[bikeID];
							AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param(
								"BikeID", bikeID + " : " + bikeData.Name,
								"UpgradeType", ((BikeUpgradeType)param[1]).ToString()));
						}
						break;

					#endregion

					#region Item

					case EventEnum.Item_ChangeAmount: {
							var itemData = Client.Item[(int)param[0]];
							var changeAmount = (int)param[1];
							var itemName = itemData.Name;
							if (changeAmount > 0) {
								AnalyticsMgrBase.Ins.Event("Item_Get", Param("ItemID", itemData.ID + " : " + itemName), changeAmount);
							} else {
								AnalyticsMgrBase.Ins.Event("Item_Use", Param("ItemID", itemData.ID + " : " + itemName), Mathf.Abs(changeAmount));
							}
						}
						break;

					#endregion

					#region Game

					case EventEnum.Game_End:
						var matchID = (int)param[1];
						var isOnline = (bool)param[2];
						var matchNmae = Client.Game.MatchInfo.GetMatchName();

						AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param(
							"GameMode", ((GameMode)param[0]).ToString(),
							"MatchID", matchID + " :" + matchNmae,
							"IsOnline", isOnline.ToString(),
							"IsChampionship", param[3].ToString(),
							"HeroID", param[4].ToString(),
							"BikeID", param[5].ToString(),
							"Rank", param[6].ToString(),
							"RunTime", param[7].ToString(),
							"Crash", param[8].ToString(),
							"IsComplete", param[9].ToString(),
							"IsChallenge", param[10].ToString()
							));
						break;

					#endregion

					#region Task

					case EventEnum.Task_GetReward:
						var taskMode = (TaskMode)param[0];
						var taskID = (int)param[1];
						var taskData = Client.Task[taskMode, taskID];
						AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param(
							"TaksMode", taskMode.ToString(),
							"TaskID", taskID + " : " + taskData.Name
							));
						break;

					#endregion

					#region Shop

					case EventEnum.Item_Buy: {
							var itemData = Client.Item[(int)param[0]];
							var changeAmount = (int)param[1];
							var itemName = itemData.Name;
							AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("ItemID", itemData.ID + " : " + itemName), changeAmount);
						}

						break;
                    #endregion
                    #region Guide
                    case EventEnum.Guide_Start: {
                            AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("GuideID", param[0].ToString()));
                        }
                        break;
                    case EventEnum.Guide_Operation: {
                            AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param(new object[] { "GuideID", param[0].ToString(), "GuideOperation", param[1].ToString() }));
                        }
                        break;
                    case EventEnum.Guide_Finish: {
                            AnalyticsMgrBase.Ins.Event(eventType.ToString(), Param("GuideID", param[0].ToString()));
                        }
                        break;
                    #endregion
                    default:
						break;
				}
			} catch (Exception e) {
				string s = "";
				for (int i = 0; i < param.Length; i++) {
					if (i > 0) {
						s += ",";
					}
					s += param[i];
				}
				Debug.LogError("[Analytics] Error! Event=" + eventType + ", param=" + (param.Length > 0 ? s : "null"));
				Debug.LogException(e);
				AnalyticsMgrBase.Ins.Event("Syetem_AnalyticsException", Param("Exception", e.GetType().ToString(), "EventType", eventType.ToString()));
			}
		}

		void OnEnable() {
			Client.EventMgr.AddSysListener(EventEnum.AllEvent, OnEvent);
		}

		void OnDisable() {
			Client.EventMgr.RemoveSysListener(EventEnum.AllEvent, OnEvent);
		}

		Dictionary<string, object> Param(params object[] p) {
			if (p.Length % 2 != 0) {
				Debug.LogError("Param should be pair!");
			}

			Dictionary<string, object> dict = new Dictionary<string, object>();
			for (int i = 0; i + 1 < p.Length; i += 2) {
				dict.Add(p[i].ToString(), p[i + 1]);
			}
			return dict;
		}
	}
}
