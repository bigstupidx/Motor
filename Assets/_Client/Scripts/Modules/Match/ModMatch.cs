using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using GameUI;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class ModMatch : ClientModule {
		// 章节列表
		protected Dictionary<int, ChapterData> chapterList = new Dictionary<int, ChapterData>();

		// 关卡列表
		protected Dictionary<int, MatchData> matchList = new Dictionary<int, MatchData>();

		public override void InitData(DbAccess db) {
			var reader = db.ReadFullTable("Chapter");
			while (reader.Read()) {
				ChapterData data = new ChapterData(reader);
				chapterList.Add(data.ID, data);
			}

			reader = db.ReadFullTable("Match");
			while (reader.Read()) {
				MatchData data = new MatchData(reader);
				matchList.Add(data.ID, data);
				if (data.Chapter != null) {
					data.Chapter.Matches.Add(data.ID, data);
				}
			}
		}

		public override void ResetData() {
			chapterList.Clear();
			matchList.Clear();
		}

		public ChapterData GetChapter(int id) {
			ChapterData data;
			chapterList.TryGetValue(id, out data);
			return data;
		}

		public List<ChapterData> GetSortedChapterDatas() {
			List<ChapterData> ret = new List<ChapterData>(this.chapterList.Values);
			ret.Sort((l, r) => {
				return l.Index - r.Index;
			});
			return ret;
		}

		/// <summary>
		/// 获取章节信息
		/// </summary>
		/// <param name="chapterId"></param>
		/// <returns></returns>
		public ChapterInfo GetChapterInfo(int chapterId) {
			ChapterInfo chapterInfo = null;
			if (!Client.User.UserInfo.ChapterInfoList.TryGetValue(chapterId, out chapterInfo)) {//如果没有找到章节信息则添加一个
				chapterInfo = new ChapterInfo(this.chapterList[chapterId]);
				Client.User.UserInfo.ChapterInfoList.Add(chapterId, chapterInfo);
			}
			return chapterInfo;
		}

		/// <summary>
		/// 获取关卡信息
		/// </summary>
		/// <param name="MatchId"></param>
		/// <returns></returns>
		public MatchInfo GetMatchInfo(int MatchId) {
			var data = this.matchList[MatchId];
			MatchInfo ret = null;
			ChapterInfo chapterInfo = GetChapterInfo(data.Chapter.ID);
			if (!chapterInfo.matchInfoList.TryGetValue(MatchId, out ret)) {//如果没有找到关卡信息则添加一个
				ret = new MatchInfo(data);
				chapterInfo.matchInfoList.Add(MatchId, ret);
			}
			return ret;
		}

		public MatchInfo GetMatchInfo(int chapterId, int index) {
			var list = GetChapterInfo(chapterId).GetSortedMatches();
			if (index>=list.Count || index<0)
			{
				Debug.LogError("[Client.Match]:Match index error");
			}
			MatchInfo ret = list[index];
			return ret;
		}

		/// <summary>
		/// 获取已拥有的总星星数
		/// </summary>
		/// <returns></returns>
		public int GetTotalOwnedStar() {
			int ret = 0;
			foreach (var chapter in Client.User.UserInfo.ChapterInfoList) {
				foreach (var match in chapter.Value.matchInfoList.Values) {
					ret += match.GetStarCount();
				}
			}
			return ret;
		}

		/// <summary>
		/// 总星数
		/// </summary>
		/// <returns></returns>
		public int GetTotalStar()
		{
			int ret = 0;
			for (int i = 0; i < chapterList.Count; i++)
			{
				foreach (var match in this.chapterList[i].Matches.Values)
				{
					ret += match.LevelTasks.Length;
				}
			}
			
			return ret;
		}

		/// <summary>
		/// 该章节总星数
		/// </summary>
		/// <returns></returns>
		public int GetChapterTotalStar(int Chapter) {
			int ret = 0;
			foreach (var match in this.chapterList[Chapter].Matches.Values) {
				ret += match.LevelTasks.Length;
			}
			return ret;
		}

		/// <summary>
		/// 该章节已获取的星星数
		/// </summary>
		/// <param name="Chapter"></param>
		/// <returns></returns>
		public int GetChapterOwnedStar(int Chapter) {
			int ret = 0;
			foreach (var match in Client.User.UserInfo.ChapterInfoList[Chapter].matchInfoList.Values) {
				ret += match.GetStarCount();
			}
			return ret;
		}

		/// <summary>
		/// 领取完美通关奖励
		/// </summary>
		/// <param name="chapterId"></param>
		public void GetPerfectReward(int chapterId, Action<bool> onDone = null)
		{
			Client.Item.GetRewards(chapterList[chapterId].RewardList,true);
			Client.User.UserInfo.ChapterInfoList[chapterId].IsRewarded = true;
			SaveData();
			if (onDone != null) onDone(true);
		}

		public override void InitInfo(string s) {
			// 创建原始数据
			foreach (var data in chapterList)
			{
				Client.User.UserInfo.ChapterInfoList.Add(data.Key, new ChapterInfo(data.Value));
			}
			// 恢复保存的关卡完成度
			var chapterInfos = JsonMapper.ToObject<Dictionary<int, ChapterInfo>>(s);
			if (chapterInfos != null)
			{
				foreach (var chapter in chapterInfos)
				{
					foreach (var match in chapter.Value.matchInfoList)
					{
						var matchUser = Client.User.UserInfo.ChapterInfoList[chapter.Key].matchInfoList[match.Key];
						for (var i = 0; i < 3; i++)
						{
							matchUser.TaskResults[i] = match.Value.TaskResults[i];
						}
						matchUser.IsStoryPlayed = match.Value.IsStoryPlayed;
					}
					Client.User.UserInfo.ChapterInfoList[chapter.Key].IsRewarded = chapter.Value.IsRewarded;//完美通关奖励是否已领
				}
			}
		}

		public override string ToJson(UserInfo user) {
			//大部分没有过关的关卡信息不需要写入，这边过滤掉
			Dictionary<int, ChapterInfo> copy = new Dictionary<int, ChapterInfo>();
			foreach (var chapter in user.ChapterInfoList)
			{
				ChapterInfo info = new ChapterInfo();
				foreach (var match in chapter.Value.matchInfoList) {
					// 有完成的任务则加入要保存的列表
					foreach (var taskResult in match.Value.TaskResults) {
						if (taskResult == true) {
							info.matchInfoList.TryAdd(match.Key, match.Value);
							break;
						} 
					}
					// 剧情已播放加入列表
					if (match.Value.IsStoryPlayed)
					{
						info.matchInfoList.TryAdd(match.Key, match.Value);
					}
				}
				info.IsRewarded = chapter.Value.IsRewarded;
				copy.Add(chapter.Key, info);
			}
			return JsonMapper.ToJson(copy);
		}


#if CLIENT_GM || UNITY_EDITOR
		#region GM
		protected GUIWindow Window = new GUIWindow("关卡", 450, 320);
		Vector2 _chapterScrollPos = Vector2.zero;
		int _chapterIndex = 0;
		List<ChapterData> _chapterList = null;
		string[] _chapterStr = null;
		Vector2 _matchScrollPos = Vector2.zero;
		int _selectMatch = 0;
		List<MatchInfo> _matchList = null;
		string[] _matchStr = null;


		void OnWinOpen()
		{

			_chapterList = GetSortedChapterDatas();
			_chapterStr = new string[_chapterList.Count];
			for (int i = 0; i < _chapterList.Count; i++)
			{
				string str = _chapterList[i].Name;
				for (int j = 5; j < str.Length; j += 6)
				{
					str = str.Insert(j, "\n");
				}
				_chapterStr[i] = str;
			}

			if (_chapterList.Count == 0)
			{
				_matchList = new List<MatchInfo>();
				_matchStr = new string[0];
			} else
			{
				if (_chapterIndex >= _chapterList.Count)
				{
					_chapterIndex = _chapterList.Count - 1;
				}
				_matchList = GetChapterInfo(_chapterIndex).GetSortedMatches();
				_matchStr = new string[_matchList.Count];
				for (int i = 0; i < _matchList.Count; i++)
				{
					string str = _matchList[i].Data.Name;
					for (int j = 6; j < str.Length; j += 7)
					{
						str = str.Insert(j, "\n");
					}
					_matchStr[i] = str;
				}
			}
		}

		void OnWinClose() {
			_chapterList = null;
			_chapterStr = null;
			_matchList = null;
			_matchStr = null;
		}

		void WinFunc() {
			

			GUILayout.BeginHorizontal();

			_chapterScrollPos = GUILayout.BeginScrollView(_chapterScrollPos, false, true, GUILayout.Width(130), GUILayout.Height(230));
			int sel = GUILayout.SelectionGrid(_chapterIndex, _chapterStr, 1);
			if (sel != _chapterIndex)
			{
				_chapterIndex = sel;
				OnWinOpen();
			}
			GUILayout.EndScrollView();

			GUILayout.BeginVertical();

			_matchScrollPos = GUILayout.BeginScrollView(_matchScrollPos, false, true, GUILayout.Width(300), GUILayout.Height(200));
			_selectMatch = GUILayout.SelectionGrid(_selectMatch, _matchStr, 3);
			GUILayout.EndScrollView();

			if (_selectMatch < _matchList.Count)
			{
				MatchInfo info = _matchList[_selectMatch];
				GUILayout.BeginHorizontal();
				GUILayout.Label("【" + info.Data.ID + "】 " + info.Data.Name);
				GUILayout.Label(info.GetStarCount().ToString(), GUI.skin.textField);

				if (GUILayout.Button("3星"))
				{
					GMConsole.SetMatchClearRank(info);
				}
//				if (GUILayout.Button("开始"))
//				{
//					if (ModMenu.Ins.LevelChooseBoard != null)
//					{
//						Client.Game.StartGame(info);
//					}
//				}
				GUILayout.EndHorizontal();
			}


			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();
			if (GUILayout.Button("关闭"))
			{
				ClientGUI.Ins.CloseWindow(this);
			}
		}
		#endregion
#endif

	}
}
