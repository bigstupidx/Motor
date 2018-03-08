using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI {
	public class UIGameOverInfoOnline : UIGameOverInfoBase {
		#region base

		public const string UIPrefabName = "UI/Board/GameOverBoard/GameOverBoard_Online";

		public static void Show() {
			ModMenu.Ins.Overlay(new string[] { UIPrefabName }, "UIGameOverInfoOnline");
		}

		#endregion
		public Text RankText;
		public Text TotalTime;
		public Text[] RewardNum;
		public Image[] RewardIcon;
		public GameObject Reward;
		protected override void Init() {
			base.Init();
			RankText.text = string.Format(LString.GAMEUI_RANK.ToLocalized(), BikeManager.Ins.CurrentBike.racerInfo.Rank);
			TotalTime.text = CommonUtil.GetFormatTime(BikeManager.Ins.CurrentBike.racerInfo.RunTime);
			Reward.gameObject.SetActive(GameModeBase.Ins.GetWinOrFail());
			if (GameModeBase.Ins.GetWinOrFail()) {
				SetReward();
			}
		}

		public void SetReward() {
			var list = Client.Online.GetReward(Client.Game.MatchInfo.Data.GameMode);
			int count;
			if (list == null) {
				count = 0;
			} else {
				count = list.Count;
				for (int i = 0; i < count; i++) {
					RewardIcon[i].sprite = Client.Item[list[i].ID].Icon.Sprite;
					RewardNum[i].text = "x" + list[i].Amount.ToString();
					Client.Item.GainItem(Client.Item[list[i].ID], list[i].Amount, true);
				}
			}
			for (int i = RewardIcon.Length; i > count; i--) {
				RewardIcon[i - 1].gameObject.SetActive(false);
				RewardNum[i - 1].gameObject.SetActive(false);
			}
		}
	}

}

