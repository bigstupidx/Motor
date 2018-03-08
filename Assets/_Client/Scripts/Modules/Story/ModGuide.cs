//
// ModGuide.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System.Collections.Generic;
using System.Linq;
using XPlugin.Data.Json;

namespace GameClient {
	public enum GuideType {
		StoryGuide_1_Game = 1,
		StoryGuide_12_UpgradeHero = 12,
		StoryGuide_13_UpgradeBike = 13,
		StoryGuide_14_GameNormal = 14,

		StoryGuide_17_HeroBoard = 17,
		StoryGuide_18_BikeBoard = 18,
		StoryGuide_19_WeaponBoard = 19,
		StoryGuide_20_PropBoard = 20,
		StoryGuide_21_TaskBoard = 21,
		StoryGuide_22_ShopBoard = 22,
		StoryGuide_23_OnlienBoard = 23,
		StoryGuide_24_ChampionshipBoard = 24,
		StoryGuide_25_GameModeTiming = 25,
	}

	public class ModGuide : ClientModule {
		/// <summary>
		/// 引导任务字典
		/// </summary>
		[JsonIgnore]
		protected Dictionary<int, StoryGuideBase> GuideDic = new Dictionary<int, StoryGuideBase>();

		public List<int> MainGuideList = new List<int>();
		public List<int> FreeGuideList = new List<int>();

		/// <summary>
		/// 已经完成的引导
		/// </summary>
		public List<string> CompletedGuide = new List<string>();

		public override void InitInfo(string str) {
			base.InitInfo(str);

			if (Client.Config.IgnoreStory) {
				return;
			}

			// 读取
			if (!string.IsNullOrEmpty(str)) {
				var list = JsonMapper.ToObject<List<string>>(str);
				MainGuideList = JsonMapper.ToObject<List<int>>(list[0]);
				FreeGuideList = JsonMapper.ToObject<List<int>>(list[1]);
			} else {
				ResetGuide();
			}

			// 主线教程
			GuideDic.Add((int)GuideType.StoryGuide_1_Game, new StoryGuide_1_Game());
			GuideDic.Add((int)GuideType.StoryGuide_12_UpgradeHero, new StoryGuide_12_UpgradeHero());
			GuideDic.Add((int)GuideType.StoryGuide_13_UpgradeBike, new StoryGuide_13_UpgradeBike());
			GuideDic.Add((int)GuideType.StoryGuide_14_GameNormal, new StoryGuide_14_GameNormal());

			// 自由教程
			GuideDic.Add((int)GuideType.StoryGuide_17_HeroBoard, new StoryGuide_17_HeroBoard());
			GuideDic.Add((int)GuideType.StoryGuide_18_BikeBoard, new StoryGuide_18_BikeBoard());
			GuideDic.Add((int)GuideType.StoryGuide_19_WeaponBoard, new StoryGuide_19_WeaponBoard());
			GuideDic.Add((int)GuideType.StoryGuide_20_PropBoard, new StoryGuide_20_PropBoard());
			GuideDic.Add((int)GuideType.StoryGuide_21_TaskBoard, new StoryGuide_21_TaskBoard());
			GuideDic.Add((int)GuideType.StoryGuide_22_ShopBoard, new StoryGuide_22_ShopBoard());
			GuideDic.Add((int)GuideType.StoryGuide_23_OnlienBoard, new StoryGuide_23_OnlienBoard());
			GuideDic.Add((int)GuideType.StoryGuide_24_ChampionshipBoard, new StoryGuide_24_ChampionshipBoard());
			GuideDic.Add((int)GuideType.StoryGuide_25_GameModeTiming, new StoryGuide_25_GameModeTiming());
			EnableGuide();
			// GuideDic[21].Enable();
		}

		public void ResetGuide() {
			// 教程进度初始化
			MainGuideList.AddRange(new[]{
				(int)GuideType.StoryGuide_1_Game,
				(int)GuideType.StoryGuide_12_UpgradeHero,
				(int)GuideType.StoryGuide_13_UpgradeBike,
				(int)GuideType.StoryGuide_14_GameNormal
			});
			FreeGuideList.AddRange(new[]{
				(int)GuideType.StoryGuide_17_HeroBoard,
				(int)GuideType.StoryGuide_18_BikeBoard,
				(int)GuideType.StoryGuide_19_WeaponBoard,
				(int)GuideType.StoryGuide_20_PropBoard,
				(int)GuideType.StoryGuide_21_TaskBoard,
				(int)GuideType.StoryGuide_22_ShopBoard,
				(int)GuideType.StoryGuide_23_OnlienBoard,
				(int)GuideType.StoryGuide_24_ChampionshipBoard,
				(int)GuideType.StoryGuide_25_GameModeTiming,
			});
		}

		/// <summary>
		/// 新手引导是否已全部完成
		/// </summary>
		public bool IsGuideFinished {
			get { return MainGuideList.Count + FreeGuideList.Count == 0; }
		}

		public bool CheckMainGuideFinished(int id) {
			return MainGuideList.IndexOf(id) == -1;
		}

		/// <summary>
		/// 强制引导是否结束
		/// </summary>
		public bool IsMainGuideFinished {
			get { return MainGuideList.Count == 0; }
		}

		public bool IsPropGuideFinished {
			get { return FreeGuideList.IndexOf(20) < 0; }
		}

		public void FinishGuide(GuideType type) {
			FinishGuide((int)type);
		}

		public void FinishGuide(int id) {
			// 如果结束的是主线教程，则继续开启任务，自由任务仅从队列移除即可
			if (MainGuideList.Contains(id)) {
				MainGuideList.Remove(id);
				EnableGuide();
			} else {
				if (FreeGuideList.Contains(id)) {
					var guide = GuideDic[id];
					if (guide.Active) guide.Disable();
					FreeGuideList.Remove(id);
				}
			}
			Client.EventMgr.SendEvent(EventEnum.Guide_Finish, id);
			SaveDataNow();
		}

		/// <summary>
		/// 开始教程
		/// </summary>
		/// <param name="id"></param>
		public void StartGuide(int id) {
			Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
		}

		public void EnableGuide() {
			// 按顺序启动主线教程，没有则启动全部自由教程
			if (MainGuideList.Any()) {
				var id = MainGuideList[0];
				GuideDic[id].Enable();
			} else {
				if (FreeGuideList.Any()) {
					foreach (var id in FreeGuideList) {
						GuideDic[id].Enable();
					}
				}
			}
		}

		public override string ToJson(UserInfo user) {
			var list = new List<string>{
				JsonMapper.ToJson(MainGuideList),
				JsonMapper.ToJson(FreeGuideList)
			};
			return JsonMapper.ToJson(list);

		}

		public void Log(string text) {
			Client.Log("<color=green>[Story]</color> " + text);
		}

	}
}
