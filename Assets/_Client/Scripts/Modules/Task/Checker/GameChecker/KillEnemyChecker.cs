//
// KillEnemyChecker.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace GameClient {
	/// <summary>
	/// 击杀指定数量的敌人
	/// </summary>
	public class KillEnemyChecker : TaskChecker {
		public string Key = "";

		public static int KeyCount = 0;

		public KillEnemyChecker() {
			if (Client.Game.IsGaming) {
				Key = KeyCount++.ToString();
			} else Key = "";
		}

		protected override EventEnum[] EventEnums {
			get {
				return new EventEnum[] { EventEnum.Game_KillEnemy };
			}
		}


		public override int TaskProgress {
			get {
				return GetStat<int>("KillEnemy" + Key);
			}
		}

		protected override void OnEvent(EventEnum eventType, params object[] args) {
			if (args.Length >= 1) {
				SetStat("KillEnemy" + Key, TaskProgress + (int)args[0]);
				base.OnEvent(eventType, args);
			}
		}
	}
}

