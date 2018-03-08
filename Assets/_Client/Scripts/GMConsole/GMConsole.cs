//
// GMConsole.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;

namespace GameClient
{
#if CLIENT_GM || UNITY_EDITOR
	public static class GMConsole
	{
		public static void ResetAll(Action<bool> onDone = null)
		{
			PlayerPrefs.DeleteAll();
//			Client.Ins.Reset(onDone);

			if(onDone != null)onDone(true);
		}

		public static void SetMatchClearRank(MatchInfo info)
		{
			int chapter = info.Data.Chapter.Index;
			for (int i = 0; i < chapter+1; i++)
			{
				var matchList = Client.Match.GetChapterInfo(i).GetSortedMatches();
				int count;
				if (i == chapter)
				{
					count = info.Data.Index+1;
				}
				else
				{
					count = matchList.Count;
				}
				for (int j = 0; j < count; j++)
				{
					matchList[j].TaskResults[0] = true;
					matchList[j].TaskResults[1] = true;
					matchList[j].TaskResults[2] = true;
				}
			}

			Client.Match.SaveData();
		}
	}
#endif
}