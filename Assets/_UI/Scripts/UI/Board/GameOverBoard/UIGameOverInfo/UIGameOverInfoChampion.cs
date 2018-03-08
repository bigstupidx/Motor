
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class UIGameOverInfoChampion : UIGameOverInfoBase {

		public const string UIPrefabName = "UI/Board/GameOverBoard/GameOverBoard_Championship";

		public static void Show() {
			ModMenu.Ins.Overlay(new string[] { UIPrefabName }, "UIGameOverInfoChampion");
		}

		public Text RaceTime;
		public Text BestTime;
		public ReduceStamina ReduceStaminaEffect;
		public Transform Mask;

		protected override void Init() {
			base.Init();
			//BestTime.text = CommonUtil.GetFormatTime(Client.Championship.GetCurrentChampionshipInfo().BestTime);
			Mask.gameObject.SetActive(false);
			ReduceStaminaEffect.Init();
			RaceTime.text = CommonUtil.GetFormatTime(BikeManager.Ins.CurrentBike.racerInfo.RunTime);
			SetBestTime();
		}

		public void OnBtnAgainClick() {
			if (Client.Game.MatchInfo.Data.NeedStamina <= 0) {
				Client.Game.StartGame(Client.Game.MatchInfo);
				return;
			}
			//消耗体力
			if (Client.Stamina.ChangeStamina(-Client.Game.MatchInfo.Data.NeedStamina)) {
				Mask.gameObject.SetActive(true);
				ReduceStaminaEffect.Show(Client.Game.MatchInfo.Data.NeedStamina);

				this.DelayInvoke(() => {
					Client.Game.StartGame(Client.Game.MatchInfo);
				}, 0.5f);

			} else {
				CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE_3).ToLocalized());
			}

		}

		public void OnBtnRankClick() {
//			WaittingTip.Show();
//			Client.Championship.GetRank(Client.Championship.CurrentId, (b, list, self) => {
//				WaittingTip.Hide();
//				if (b) {
//					ChampionshipRankDialog.Show(list, self);
//				} else {
//					CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONBTNRANKCLICK_1).ToLocalized());
//				}
//			});
		}

		public void SetBestTime() {
			WaittingTip.Show();
			Client.Championship.GetRank(Client.Championship.CurrentId, (b, list, self) => {
				WaittingTip.Hide();
				if (b) {
					BestTime.text = CommonUtil.GetFormatTime(self.RunTime);
				} else {
					CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONBTNRANKCLICK_1).ToLocalized());
				}
			});
		}
	}


}
