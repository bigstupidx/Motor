using System.Collections.Generic;
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class UIRaceInfoBase : Singleton<UIRaceInfoBase> {
		public virtual void OnUISpawned() {
			Init();
		}

		public virtual void OnUIDespawn() {
		}

		public UIRaceTime RaceTime;
		public Text Speed;

		[SerializeField]
		[HideInInspector]
		public int currentCoinCount;


		public void OnEnable() {
			BikeManager.Ins.CurrentBike.racerInfo.OnFinish += OnGameOver;
		}

		public void OnDisable() {
			if (RaceManager.Ins != null && BikeManager.Ins.CurrentBike != null) {
				BikeManager.Ins.CurrentBike.racerInfo.OnFinish -= OnGameOver;
			}
		}

		public virtual void Update() {
			UpdateRaceTime();
			UpdateSpeed();
			UpdateRaceProgress();
			UpdateDistance();
		}

		public virtual void Init() {
			if (GamePlayBoard.Inited) {
				return;
			}
			if (GamePlayBoard.Ins == null) {
				Debug.LogError("==>GamePlayBoard.Ins  null");
			}
			GamePlayBoard.Ins.RaceInfo = this;
			// Debug.Log("==>GamePlayBoard.Ins.RaceInfo: " + GamePlayBoard.Ins.RaceInfo);

			currentCoinCount = 0;
			RaceTime.SetTime(0);
			Speed.text = "0";
			InitRaceProgress();
		}

		/// <summary>
		/// 初始化比赛进度条
		/// </summary>
		protected void InitRaceProgress() {
		}

		/// <summary>
		/// 更新比赛进度条
		/// </summary>
		public virtual void UpdateRaceProgress() {
		}

		/// <summary>
		/// 更新比赛时间
		/// </summary>
		public virtual void UpdateRaceTime() {
			RaceTime.SetTime(BikeManager.Ins.CurrentBike.racerInfo.RunTime);
		}

		/// <summary>
		/// 更新速度
		/// </summary>
		public virtual void UpdateSpeed() {
			Speed.text = ((int)BikeManager.Ins.CurrentBike.bikeControl.Speed).ToString();
		}

		/// <summary>
		/// 更新获得的金币数量
		/// </summary>
		/// <param name="value"></param>
		public virtual void UpdateCoin(int value) {
			currentCoinCount += value;
		}

		/// <summary>
		/// 更新车距
		/// </summary>
		public virtual void UpdateDistance() {
		}

		public void OnGameOver(BikeBase bikeBase) {
			gameObject.SetActive(false);
		}
	}
}
