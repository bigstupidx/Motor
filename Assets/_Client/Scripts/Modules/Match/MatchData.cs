using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class MatchEnemyData {
		public string Nickname;
		public string Avatar;
		public BikeData Bike;
		public int BikeLv;
		public HeroData Hero;
		public int HeroLv;
		public int AI;
		public WeaponData Weapon;
		public PropData Prop;

		public MatchEnemyData()
		{
			
		}

		public MatchEnemyData(JObject json) {
			this.Nickname = json["Nickname"].OptString("");
			//TODO
			//this.Avatar = json ["Avatar"].OptString ("");
			this.Bike = Client.Bike[json["Bike"].AsInt()];
			this.BikeLv = json["BikeLv"].OptInt(0);
			this.Hero = Client.Hero[json["Hero"].AsInt()];
			this.HeroLv = json["HeroLv"].OptInt(0);
			this.AI = json["AI"].OptInt(1);
			this.Weapon = Client.Weapon[json["Weapon"].AsInt()];
			this.Prop = Client.Prop[json["Prop"].AsInt()];
		}
	}

	public class MatchData {

		public int ID;
		public ChapterData Chapter;

		/// <summary>
		/// 关卡在章节中的序号
		/// </summary>
		public int Index;

		/// <summary>
		/// 关卡名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 游戏模式
		/// </summary>
		public GameMode GameMode;

		/// <summary>
		/// 圈数
		/// </summary>
		public int Turn;
		/// <summary>
		/// 场景名
		/// </summary>
		public SceneData Scene;

		/// <summary>
		/// 行车路线
		/// </summary>
		public string RaceLine;

		/// <summary>
		/// 道具路线
		/// </summary>
		public string ObjLine;

		/// <summary>
		/// 通关时间限制（仅锦标赛使用）
		/// </summary>
		public float TimeLimit;

		/// <summary>
		/// 时间恢复倍率（仅计时模式使用）
		/// </summary>
		public float TimeRate;

		/// <summary>
		/// 敌人数量上限（仅战斗模式使用）
		/// </summary>
		public int EnemyCount;
		/// <summary>
		/// 单圈敌人密度(仅战斗模式使用，0-1)
		/// </summary>
		public float EnemyDensity;

		/// <summary>
		/// 关卡任务ID
		/// </summary>
		public TaskData[] LevelTasks;

		public int UnlockStarCount;

		public HeroData NeedHero;
		public BikeData NeedBike;
		/// <summary>
		/// 消耗的体力
		/// </summary>
		public int NeedStamina;
        /// <summary>
        /// 车辆级别(仅战斗模式使用)
        /// </summary>
        public int LevelBike;
        /// <summary>
        /// 赛事可玩的次数(仅战斗模式使用)
        /// </summary>
        public int MatchTimes;
        /// <summary>
        /// 赛事背景图(仅战斗模式使用)
        /// </summary>
        public string BG;

        internal List<MatchEnemyData> EnemyDatas;

		public MatchData() {
		}


		public MatchData(SqliteDataReader reader) {
			this.ID = (int)reader.GetValue("ID");
			int? chapterID = reader.GetNullable<int>("ChapterID");
			if (chapterID != null) {
				this.Chapter = Client.Match.GetChapter((int)chapterID);
				this.Index = (int)reader.GetValue("IndexInChapter");
			}
			this.GameMode = (GameMode)(int)reader.GetValue("GameMode");
			this.Name = (string)reader.GetValue("Name");
			this.Scene = Client.Scene[(int)reader.GetValue("SceneId")];
			this.Turn = (int)reader.GetValue("Turn");
			this.RaceLine = (string)reader.GetValue("RaceLine");
			this.ObjLine = (string)reader.GetValue("ObjLine");
			this.TimeLimit = (float)reader.GetValue("TimeLimit");
            this.TimeRate = (float)reader.GetValue("TimeRate");
            this.EnemyCount = (int)reader.GetValue("EnemyCount");
			this.EnemyDensity = (float)reader.GetValue("EnemyDensity");

			char[] c = "|".ToCharArray();

			var str = ((string)reader.GetValue("Task")).Split(c);
			this.LevelTasks = new TaskData[str.Length];
			int i = 0;
			foreach (var s in str) {
				this.LevelTasks[i] = Client.Task[TaskMode.LevelTask, int.Parse(s)];
				i++;
			}

			this.UnlockStarCount = (int)reader.GetValue("UnlockStarCount");
			this.NeedHero = Client.Hero[(int)reader.GetValue("NeedHero")];
			this.NeedBike = Client.Bike[(int)reader.GetValue("NeedBike")];
			this.NeedStamina = (int)reader.GetValue("NeedStamina");

			var enemyJson = JArray.Parse((string)reader.GetValue("Enemy"));
			EnemyDatas = new List<MatchEnemyData>();
			if (enemyJson != null)
			{	
				for (int j = 0; j < enemyJson.Count; j++)
				{
					var enemy = new MatchEnemyData(enemyJson[j].AsObject());
					EnemyDatas.Add(enemy);
				}
			}
		}


		public List<PlayerInfo> GetEnemys() {
			List<PlayerInfo> ret = new List<PlayerInfo>();
			for (var i = 0; i < this.EnemyDatas.Count; i++) {
				var data = this.EnemyDatas[i];
				var info = new PlayerInfo {
					ChoosedBike = new BikeInfo(data.Bike, data.BikeLv, data.BikeLv, data.BikeLv, data.BikeLv),
					ChoosedHero = new HeroInfo(data.Hero, data.HeroLv),
					AI = data.AI,
					ChoosedWeapon = new WeaponInfo(data.Weapon),
					EquipedProps = new List<PropInfo>() {new PropInfo(data.Prop)},
					NickName = data.Nickname,
					Avatar = data.Avatar,
					SpawnPos = i,
				};
				ret.Add(info);
			}
			return ret;
		}
	}

}