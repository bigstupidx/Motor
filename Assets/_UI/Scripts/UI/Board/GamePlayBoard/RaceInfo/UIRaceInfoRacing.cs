
using System.Text;
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class UIRaceInfoRacing : UIRaceInfoBase {
		public Text rank;
		public Text Turn;//圈数

		protected int currentTurn;//当前圈数
		protected int currentRank;//当前排名

		public override void Init() {
			base.Init();
			if (GamePlayBoard.Inited) {
				return;
			}
			currentTurn = 0;
			rank.text = RaceManager.Ins.PlayerNum.ToStringFast() + "/" + RaceManager.Ins.PlayerNum.ToStringFast();
			if (Client.Game.MatchInfo.Data.Turn == 99) {
				Turn.text = "∞";
			} else {
				Turn.text = "1/" + Client.Game.MatchInfo.Data.Turn.ToStringFast();
			}
			GamePlayBoard.Inited = true;
		}

		/// <summary>
		/// 更新圈数
		/// </summary>
		public void UpdateTurn() {
			if (currentTurn != BikeManager.Ins.CurrentBike.racerInfo.Turn) {
				currentTurn = BikeManager.Ins.CurrentBike.racerInfo.Turn;
				if (Client.Game.MatchInfo.Data.Turn == 99) {
					Turn.text = "∞";
				} else {
					Turn.text = (currentTurn + 1).ToStringFast() + "/" + Client.Game.MatchInfo.Data.Turn.ToStringFast();
				}

			}
		}

		/// <summary>
		/// 更新排名
		/// </summary>
		public void UpdateRank() {
			currentRank = BikeManager.Ins.CurrentBike.racerInfo.Rank;
			rank.text = currentRank.ToStringFast() + "/" + RaceManager.Ins.PlayerNum.ToStringFast();
		}

		public override void Update() {
			if (Time.frameCount % 2 != 0) {
				return;
			}
			base.Update();
			UpdateRank();
			UpdateTurn();
		}

	}


}
