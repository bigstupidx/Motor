//
// RoadLine.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System.Collections.Generic;
using UnityEngine;
using GameClient;

namespace Game {
	public class RaceLine : MonoBehaviour {
		public bool IsReverse = false;
		public WaypointManager WaypointManager;
		public WaypointManager WaypointManagerAI;
		public SpawnpointManager SpawnpointManager;
		public TimeCheckerManager TimeCheckerManager;

	}
}

