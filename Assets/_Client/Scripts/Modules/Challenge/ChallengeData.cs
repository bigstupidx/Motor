using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;
namespace GameClient
{
    public class ChallengeData : MatchData
    {
        public ChallengeData()
        {
        }

        public ChallengeData(SqliteDataReader reader)
        {
            this.ID = (int)reader.GetValue("ID");
            this.GameMode = (GameMode)(int)reader.GetValue("GameMode");
            this.Name = (string)reader.GetValue("Name");
            this.Scene = Client.Scene[(int)reader.GetValue("SceneId")];
            this.Turn = (int)reader.GetValue("Turn");
            this.RaceLine = (string)reader.GetValue("RaceLine");
            this.ObjLine = (string)reader.GetValue("ObjLine");
            this.TimeLimit = (float)reader.GetValue("TimeLimit");
            this.EnemyCount = (int)reader.GetValue("EnemyCount");
            this.EnemyDensity = (float)reader.GetValue("EnemyDensity");
            this.LevelBike = (int)reader.GetValue("LevelBike");
            this.MatchTimes = (int)reader.GetValue("MatchTimes");
            this.BG = (string)reader.GetValue("BG");
            char[] c = "|".ToCharArray();

            var str = ((string)reader.GetValue("Task")).Split(c);
            this.LevelTasks = new TaskData[str.Length];
            int i = 0;
            foreach (var s in str)
            {
                this.LevelTasks[i] = Client.Task[TaskMode.LevelTask, int.Parse(s)];
                i++;
            }
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

	        this.TimeRate = 1f;
        }
    }
}
