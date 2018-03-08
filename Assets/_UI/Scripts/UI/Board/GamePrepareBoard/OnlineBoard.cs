using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {

	public class OnlineBoard : MonoBehaviour {
		public Text Turn;
		public Text ModeName;
		public Text Enemy;
		public Text Time;
		public GameObject Turnob;
		public GameObject Enmeyob;
		public GameObject Timeob;

		public GameObject Desc;
		public GameObject Reward;
		public Text Reward0;
		public Text Reward1;
		public Text Reward2;

		public void SetData(MatchInfo info) {
			this.gameObject.SetActive(true);
			Desc.SetActive(true);
			Reward.SetActive(false);
			ModeName.text = DataDef.GameModeName(info.Data.GameMode);
			if (Turn != null)
				switch (info.MatchMode) {
					case MatchMode.Normal:
						if (info.Data.GameMode == GameMode.Elimination || info.Data.GameMode == GameMode.EliminationProp) {
							Turnob.SetActive(false);
							Enmeyob.SetActive(true);
							Timeob.SetActive(false);
							Enemy.text = (info.Enemys.Count + 1).ToString();
						} else if (info.Data.GameMode == GameMode.Timing) {
							Turnob.SetActive(false);
							Enmeyob.SetActive(false);
							Timeob.SetActive(false);
						} else {
							Turnob.SetActive(true);
							Enmeyob.SetActive(true);
							Timeob.SetActive(false);
							Turn.text = info.Data.Turn.ToString();
							Enemy.text = (info.Enemys.Count + 1).ToString();
						}
						break;
					case MatchMode.Online:
					case MatchMode.OnlineRandom:
					case MatchMode.Championship:
						Turnob.SetActive(true);
						Enmeyob.SetActive(true);
						Timeob.SetActive(false);
						Turn.text = info.Data.Turn.ToString();
						Enemy.text = (info.Enemys.Count + 1).ToString();
						break;
					case MatchMode.Challenge:
						Desc.SetActive(false);
						Reward.SetActive(true);
						Reward0.text = info.GetRewardAmount(0).ToString();
						Reward1.text = info.GetRewardAmount(1).ToString();
						Reward2.text = info.GetRewardAmount(2).ToString();
						break;
				}
		}
	}
}
