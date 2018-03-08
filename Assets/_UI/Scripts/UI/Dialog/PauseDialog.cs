using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class PauseDialog : MonoBehaviour {

		#region base

		public const string UIPrefabPath = "UI/Dialog/PauseDialog";

		public static string[] UINames =
		{
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath
		};

		public static void Show(bool destroyBefore = false) {
			PauseDialog ins = ModMenu.Ins.Cover(UINames, "PauseDialog", destroyBefore)[1].Instance.GetComponent<PauseDialog>();
			ins.Init();
		}

		void OnUISpawned() {
		}
		//
		//		void OnUIDespawn() {
		//			
		//		}

		#endregion

		public Text Name;
		public Text Desc;
		public Image[] TaskStars;
		public Text[] TaskDescs;
		public Text[] TaskValues;
		public Text RewardCoin;
		public GameObject NormalReward;
		public GameObject ChallengeReward;
		public GameObject TaskList;
		public GameObject OnlineTask;
		public Text OnlineTaskTxt;

		public void Init() {
			if (Client.Game.MatchInfo.MatchMode == MatchMode.Online || Client.Game.MatchInfo.MatchMode == MatchMode.OnlineRandom) {
				Name.text = DataDef.OnlineGameName;
				Desc.text = DataDef.GameModeName(Client.Game.MatchInfo.Data.GameMode);
				OnlineTask.SetActive(true);
				TaskList.SetActive(false);
				OnlineTaskTxt.text = DataDef.GameModeDesc(Client.Game.MatchInfo.Data.GameMode);
			} else {
				Name.text = Client.Game.MatchInfo.Data.Name;
				Desc.text = DataDef.GameModeName(Client.Game.MatchInfo.Data.GameMode) + "\n" + DataDef.GameModeDesc(Client.Game.MatchInfo.Data.GameMode);
				OnlineTask.SetActive(false);
				TaskList.SetActive(true);
				SetTasks();
				SetReward();
			}

		}

		public void SetTasks() {
			var tasks = GameModeBase.Ins.Tasks;
			for (var i = 0; i < tasks.Count; i++) {
				var task = tasks[i];
				TaskStars[i].SetGreyMaterail(task.State != TaskState.Completed);
				TaskDescs[i].text = task.Data.Description;
				TaskValues[i].text = task.ProgressStr;
			}

			var tasksSave = Client.Game.MatchInfo.TaskResults;
			for (int i = 0; i < tasksSave.Count; i++) {
				if (tasksSave[i]) {
					TaskStars[i].SetGreyMaterail(false);
					TaskValues[i].text = (LString.GAMEUI_GAMETASKBOARD_SETTASKS).ToLocalized();
				}
			}
		}
		public void SetReward() {
			if (Client.Game.MatchInfo.MatchMode == MatchMode.Challenge) {
				NormalReward.SetActive(false);
				ChallengeReward.SetActive(true);
			} else {
				NormalReward.SetActive(true);
				ChallengeReward.SetActive(false);
			}
			//BikeManager.Ins.CurrentBike.info.GetStat<int>("PropType" + (int)PropType.Coin).ToString();
		}

		public void OnBtnSettingClick() {
			UserInfoBoard.Show();
		}

		public void OnBtnResumeClick() {
			ModMenu.Ins.Back();
			GameModeBase.Ins.Resume();
		}

		public void OnBtnExitClick() {
			switch (Client.Game.MatchInfo.MatchMode) {
				case MatchMode.Championship:
					GameUIInterface.Ins.ExitGame(ChampionshipDetailBoard.GroupIns);
					break;
				case MatchMode.Challenge:
					GameUIInterface.Ins.ExitGame(ChallengeBoard.GroupIns);
					break;
				case MatchMode.Online:
					GameUIInterface.Ins.ExitGame(ChooseOnlineMode.GroupIns, () => {
						ChooseOnlineMode.BackAndEnterLobby();
					});
					break;
				case MatchMode.OnlineRandom:
					GameUIInterface.Ins.ExitGame(NetMatchBoard.GroupIns);
					break;
				case MatchMode.Normal:
					GameUIInterface.Ins.ExitGame(LevelChooseBoard.GroupIns);
					break;
			}

			GameModeBase.Ins.Resume();
		}
	}

}

