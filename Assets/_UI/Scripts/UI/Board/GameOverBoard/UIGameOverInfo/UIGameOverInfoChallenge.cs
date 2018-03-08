using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {

	public class UIGameOverInfoChallenge : UIGameOverInfoBase {
		#region base

		public const string UIPrefabName = "UI/Board/GameOverBoard/GameOverBoard_Changlling";

		public static void Show() {
			ModMenu.Ins.Overlay(new string[]{ UIPrefabName }, "UIGameOverInfoChampion");
		}

		#endregion
		public Text BikeName;
		public Text WeaponName;
		public Text KillName;
		public Image[] rewards;
		public Text[] rewardNum;

		protected override void Init() {
			base.Init();
			BikeName.text = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Name;
			WeaponName.text = Client.Weapon[Client.User.UserInfo.ChoosedWeaponID].Name;
			KillName.text = GameTip.Ins.KillEnermyTip.Num.ToString();
			setReward();
		}

		public void setReward() {
			int i;
			var tasks = GameModeBase.Ins.Tasks;
			int k = 0;
			foreach (var item in tasks[0].Data.RewardList) {
				rewards[k].sprite = item.Data.Icon.Sprite;
				rewardNum[k].text = "0";
				k++;
			}
			for (i = 0; i < tasks.Count; i++) {
				var task = tasks[i];
				if (task.State == TaskState.Completed) {
					int j = 0;
					foreach (var item in task.Data.RewardList) {
						rewardNum[j].text = item.Amount.ToString();
						j++;
					}
				} else {
					break;
				}
			}
		}
	}
}
