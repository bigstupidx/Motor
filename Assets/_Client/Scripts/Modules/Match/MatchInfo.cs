//
// MatchInfo.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System.Collections.Generic;
using System.Text;
using XPlugin.Data.Json;

namespace GameClient {
	public enum MatchMode {
		Guide = -1,//新手教程 
		Normal = 0,//闯关模式
		Championship = 1,//锦标赛
		Challenge = 2,//挑战模式
		Online = 3,//网络对战
		OnlineRandom = 4,//随机匹配模式
	}

	public class MatchInfo {
		[JsonIgnore]
		public MatchData Data;

		//		public int BestTime;
		public List<bool> TaskResults = new List<bool>() { false, false, false };
		public bool IsStoryPlayed = false;

		public virtual void SetStoryPlayed() {
			IsStoryPlayed = true;
			Client.Match.SaveData();
		}

		[JsonIgnore]
		public MatchMode MatchMode = MatchMode.Normal;

		[JsonIgnore]
		public List<PlayerInfo> Enemys {
			get {
				if (this._enemys == null) {
					_enemys = Data.GetEnemys();
				}
				return _enemys;
			}
		}
		private List<PlayerInfo> _enemys;

		[JsonIgnore]
		public int PlayerNum {
			get { return Enemys.Count + 1; }
		}

		public MatchInfo() {
		}

		public MatchInfo(MatchData data) {
			InitData(data);
		}

		public MatchInfo(MatchData data, List<PlayerInfo> enemy, MatchMode mode) {
			InitData(data);
			this._enemys = enemy;
			this.MatchMode = mode;
		}

		public void InitData(MatchData data) {
			Data = data;
			this.TaskResults = new List<bool>() { false, false, false };
		}

		public bool IsUnlocked() {
			return Client.Match.GetTotalOwnedStar() >= this.Data.UnlockStarCount;
		}

		public int GetStarCount() {
			int ret = 0;
			foreach (var taskResult in this.TaskResults) {
				if (taskResult) {
					ret++;
				}
			}
			return ret;
		}

		public string GetMatchName() {
			var matchName = "";
			switch (Client.Game.MatchInfo.MatchMode) {
				case MatchMode.Guide:
					matchName = (LString.GAMECLIENT_MATCHINFO_GETMATCHNAME).ToLocalized();
					break;
				case MatchMode.Championship:
					matchName = (LString.GAMECLIENT_MATCHINFO_GETMATCHNAME_1).ToLocalized() + Client.Game.MatchInfo.Data.Name;
					break;
				case MatchMode.Online:
				case MatchMode.OnlineRandom:
					matchName = (LString.GAMECLIENT_MATCHINFO_GETMATCHNAME_2).ToLocalized();
					break;
				case MatchMode.Normal:
					matchName = string.Format((LString.GAMECLIENT_MATCHINFO_GETMATCHNAME_3).ToLocalized(), Data.Chapter.Index, Data.Index);
					break;
				case MatchMode.Challenge:
					matchName = (LString.GAMECLIENT_MATCHINFO_GETMATCHNAME_4).ToLocalized();
					break;
				default:
					matchName = "UnKnown";
					break;
			}
			return matchName;
		}
        public string GetMathSymbol() {
            var Symbol = "";
            switch (Client.Game.MatchInfo.MatchMode) {
                case MatchMode.Guide:
                    Symbol = MatchMode.Guide.ToString();
                    break;
                case MatchMode.Championship:
                    Symbol = MatchMode.Championship.ToString() + "_"+Client.Game.MatchInfo.Data.Name;
                    break;
                case MatchMode.Online:
                case MatchMode.OnlineRandom:
                    Symbol = MatchMode.Online.ToString();
                    break;
                case MatchMode.Normal:
                    Symbol = MatchMode.Normal.ToString()+ "_C"+Data.Chapter.Index+"_I"+ Data.Index;
                    break;
                case MatchMode.Challenge:
                    Symbol = MatchMode.Challenge.ToString();
                    break;
                default:
                    Symbol = "UnKnown";
                    break;
            }
            return Symbol;
        }


        public int GetRewardAmount(int index) {
			int amount = 0;
			for (int i = 0; i < 3; i++) {
				amount += Data.LevelTasks[i].RewardList[index].Amount;
			}
			return amount;
		}

		public static byte[] OnlineSerialize(object obj) {
			MatchInfo info = (MatchInfo)obj;
			var data = info.Data;
			JObject ret = new JObject();

			ret["SceneID"] = data.Scene.ID;
			ret["RaceLine"] = data.RaceLine;
			ret["Turn"] = data.Turn;
			ret["ObjLine"] = data.ObjLine;

			return Encoding.UTF8.GetBytes(ret.ToString());
		}

		public static object OnlineDeserialize(byte[] bytes) {
			JObject ret = JObject.Parse(Encoding.UTF8.GetString(bytes));
			var data = new MatchData {
				ID = -1,
				GameMode = GameMode.Racing,
				Scene = Client.Scene.SceneDatas[ret["SceneID"].AsInt()],
				RaceLine = ret["RaceLine"].AsString(),
				Turn = ret["Turn"].AsInt(),
				ObjLine = ret["ObjLine"].AsString()
			};
			List<PlayerInfo> list = new List<PlayerInfo>();

			return new MatchInfo(data, list, MatchMode.Online);
		}
	}
}

