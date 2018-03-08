//
// RoadLineManager.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Game {
	public class RaceLineManager : LineManagerBase<RaceLine, RaceLineManager> {
        public object GameMode { get; internal set; }

        public override RaceLine SpawnLine(string line) {
			var scene = SceneManager.GetActiveScene().name;
			scene = scene.Split('_')[0];
			return base.SpawnLine(scene + "/raceline/" + line);
		}
	}
}

