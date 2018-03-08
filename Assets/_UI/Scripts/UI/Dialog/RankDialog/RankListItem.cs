
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class RankListItem : MonoBehaviour {
		public Image RankMedal;
		public Text RankNum;
		public Text Name;
		public Text Bike;
		public Text RaceTime;
		public Image Bg;
		private RacerInfo racer;
		private int index;

		public void SetData(int index, RacerInfo info) {
			racer = info;
			this.index = index;
			RankNum.text = index.ToString();
			//名次
			if (index == 1) {
				RankMedal.color = UIDataDef.RankFirst;
			} else {
				RankMedal.color = UIDataDef.RankLast;
			}
//			if (Client.Game.MatchInfo.MatchMode == MatchMode.Online || Client.Game.MatchInfo.MatchMode == MatchMode.OnlineRandom) {
				Name.text = string.IsNullOrEmpty(info.info.NickName) ? info.info.ChoosedHero.Data.Name : info.info.NickName;
//			} else {
//				Name.text = info.info.ChoosedHero.Data.Name;
//			}
			Bike.text = info.info.ChoosedBike.Data.Name;
			RaceTime.text = CommonUtil.GetFormatTime(info.RunTime);
			if (info.gameObject.CompareTag(Tags.Ins.Player)) {
				Bg.color = UIDataDef.RankPlayer;
			} else {
				Bg.color = UIDataDef.RankOther;
			}
		}

		public void UpdateTime(RacerInfo info) {
			if (info != racer) {
				SetData(index, info);
			} else {
				RaceTime.text = CommonUtil.GetFormatTime(info.RunTime);
			}
		}

		public void SetText(RacerInfo info, string text) {
			UpdateTime(info);
			RaceTime.text = text;
		}

	}


}
