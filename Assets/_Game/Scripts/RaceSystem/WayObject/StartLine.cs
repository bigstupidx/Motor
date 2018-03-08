//
// StartLine.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game {
	public class StartLine : Singleton<StartLine> {
		public GameObject Go;
		public GameObject Finish;
		public GameObject Lap;
		public List<GameObject> Nums;

		private int _lap;
		private int _turn;
		private bool inited = false;

		bool NeedShow {
			get { return RaceManager.Ins.RaceMode != RaceMode.Elimination; }
		}

		void Start() {
			HideAll();
			_lap = 1;
			_turn = RaceManager.Ins.Turn;
			// 小地图
			try {
				var go = GameObjectUtility.LoadAndIns("MiniMap_Tag_StartLine");
				go.transform.SetParent(MiniMapCamera.Ins.transform.parent);
				go.transform.position = transform.position;
				go.transform.SetPositionY(-53f);
				go.transform.SetEulerAngleY(transform.eulerAngles.y);
			} catch (Exception e) {
				Debug.LogException(e);
			}
		}


		void Update() {
			if (!this.inited) {
				if (BikeManager.Ins.CurrentBike != null) {
					this.inited = true;
					BikeManager.Ins.CurrentBike.racerInfo.OnPassFinishPoint += self => {
						HideAll();
						if (!NeedShow) {
							return;
						}
						_lap++;
						if (_lap <= _turn) {
							ShowLap(_lap);
						} else if (_turn > 0) {
							ShowFinish();
						}
					};
				}
			}
		}

		public void ShowGo() {
			HideAll();
			Go.SetActive(true);
		}

		public void ShowLap(int lap) {
			HideAll();
			Lap.SetActive(true);
			if (Nums[lap] != null) Nums[lap].SetActive(true);
		}

		public void ShowFinish() {
			HideAll();
			Finish.SetActive(true);
		}

		public void HideAll() {
			Go.SetActive(false);
			Finish.SetActive(false);
			Lap.SetActive(false);
			foreach (var num in Nums) {
				if (num == null) continue;
				num.SetActive(false);
			}
		}
	}

}
