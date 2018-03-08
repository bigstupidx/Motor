
using System.Collections.Generic;
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class UIGameOverInfoNormal : UIGameOverInfoBase {
		#region base

		public const string UIPrefabName = "UI/Board/GameOverBoard/GameOverBoard_Normal";

		public static void Show() {
			var @group = ModMenu.Ins.Overlay(new string[]{ UIPrefabName }, "UIGameOverInfoNormal");
			GameOverBoard.Ins.UIGameOverInfo = @group[0].Instance.GetComponent<UIGameOverInfoNormal>();
		}

		#endregion
		public GameObject[] Stars;
		public Image[] TaskStars;
		public Text[] TaskDescs;
		public Text[] TaskValues;
		public Text RewardCoin;
		public UIEffect_LoadOnStart[] StarEffects;
		public GameObject BtnRank;

		public GameObject NormalReward;
		public GameObject ChallengeReward;
		public float StarDelayTime = 0.6f;

		private List<Coroutine> showStarsCoroutines = new List<Coroutine>();
		private float _setStarTime = 0f;
		private int _rewardCoin = 0;
		private int _rewardCoinTemp = 0;

		[HideInInspector]
		public bool SetRewardStart = false;

		void OnDisable() {
			for (int i = 0; i < StarEffects.Length; i++) {
				StarEffects[i].gameObject.SetActive(false);
			}

			this.NormalReward.SetActive(false);
			this.ChallengeReward.SetActive(false);
		}

		protected override void Init() {
			Reset();

			base.Init();
			if (Client.Game.MatchInfo.MatchMode == MatchMode.Challenge) {
				NormalReward.SetActive(false);
				ChallengeReward.SetActive(true);
			} else {
				NormalReward.SetActive(true);
				ChallengeReward.SetActive(false);
			}
			SetTasks();

			this.DelayInvoke(SetStar, 0.5f);

			BtnRank.SetActive(Client.Game.MatchInfo.Data.GameMode != GameMode.Timing);
		}

		public void Reset() {
			SetRewardStart = false;
			RewardCoin.text = "0";
			_rewardCoinTemp = 0;
			for (int i = 0; i < StarEffects.Length; i++) {
				StarEffects[i].gameObject.SetActive(true);
			}
			for (int i = 0; i < showStarsCoroutines.Count; i++) {
				Stars[i].SetActive(false);
				StopCoroutine(showStarsCoroutines[i]);
			}
			showStarsCoroutines.Clear();
		}

		public void SetStar() {
			_setStarTime = 0f;
			_rewardCoin = 0;
			var count = 0;
			var tasks = GameModeBase.Ins.Tasks;
			for (var i = 0; i < tasks.Count; i++) {
				if (tasks[i].State == TaskState.Completed) {
					var index = count;
					showStarsCoroutines.Add(this.DelayInvoke(() => {
						Stars[index].SetActive(true);
						SfxManager.Ins.PlayOneShot(SfxType.SFX_Over_Star);
					}, StarDelayTime * (count)));
					count++;
					_rewardCoin += tasks[i].Data.RewardList[0].Amount;
				}
			}
			_setStarTime = StarDelayTime * (count - 1) + 0.5f;
			this.DelayInvoke(SetReward, _setStarTime);
		}

		public void SetTasks() {
			var tasks = GameModeBase.Ins.Tasks;
			for (var i = 0; i < tasks.Count; i++) {
				var task = tasks[i];
				TaskStars[i].SetGreyMaterail(task.State != TaskState.Completed);
				TaskDescs[i].text = task.Data.Description;
				TaskValues[i].text = task.ProgressStr;
			}
		}

		public void SetReward() {
			if (Client.Game.MatchInfo.MatchMode != MatchMode.Challenge) {
				SetRewardStart = _rewardCoin != 0;
			}
			//RewardCoin.text = BikeManager.Ins.CurrentBike.info.GetStat<int>("PropType" + (int)PropType.Coin).ToString();
		}

		public void SetRewardImmediately() {
			if (Client.Game.MatchInfo.MatchMode == MatchMode.Challenge) {
				return;
			}
			SetRewardStart = false;
			RewardCoin.text = _rewardCoin.ToString();
		}

		void Update() {
			if (Client.Game.MatchInfo.MatchMode != MatchMode.Challenge && SetRewardStart) {
				_rewardCoinTemp += 1;
				RewardCoin.text = _rewardCoinTemp.ToString();
				SetRewardStart = _rewardCoinTemp != _rewardCoin;
			}
		}
	}


}
