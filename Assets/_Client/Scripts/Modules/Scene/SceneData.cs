using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.SQLite;

namespace GameClient {
	public class SceneData {

		public int ID;
		public string Name;
		public string Desc;
		public string Icon;
		public string SceneName;
		public string[] RaceLines;
		public string[] RaceObjLines;
		public string[] PropObjLines;
		public string[] TimingObjLines;
		public bool Forward;


		public SceneData(SqliteDataReader reader) {
			this.ID = (int)reader.GetValue("ID");
			this.Name = (string)reader.GetValue("Name");
			this.Icon = (string)reader.GetValue("Icon");
			this.Desc = (string)reader.GetValue("Description");

			this.SceneName = (string) reader.GetValue("SceneName");
			char[] c = "|".ToCharArray();
			this.RaceLines = ((string) reader.GetValue("RaceLine")).Split(c);
			this.RaceObjLines = ((string) reader.GetValue("RaceObjLine")).Split(c);
			this.PropObjLines = ((string) reader.GetValue("PropObjLine")).Split(c);
			this.TimingObjLines = ((string) reader.GetValue("TimingObjLine")).Split(c);

			this.Forward = (bool) reader.GetValue("Forward");
		}


		public string RandomRaceLine {
			get { return RandomArray(this.RaceLines); }
		}

		public string RandomRaceObjLine {
			get { return RandomArray(this.RaceObjLines); }
		}

		public string RandomPropObjLine {
			get { return RandomArray(this.PropObjLines); }
		}

		public string RandomTimingObjLine {
			get { return RandomArray(this.TimingObjLines); }
		}

		public static T RandomArray<T>(T[] s) {
			return s[UnityEngine.Random.Range(0, s.Length)];
		}


	}
}