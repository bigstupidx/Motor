using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class ChampionshipRewardItem : MonoBehaviour
	{
		public Text Condition;
		public Image Icon;
		public Text Amount;
		public GameObject CompletedFlag;
		public Image Bg;
		public Text Desc;

		public void SetData(ChampionshipRewardData data)
		{
			var currentInfo = ChampionshipDetailBoard.Ins.currentInfo;
			bool isCompleted = false;

			if (false)
			{
				Condition.text = (LString.GAMEUI_CHAMPIONSHIPRANKREWARDITEM).ToLocalized() + data.Rank;
				isCompleted = (currentInfo.Rank != -1 && currentInfo.Rank <= data.Rank);
			}
			else
			{
				isCompleted = currentInfo.TaskResults[data.Condition - 1];
			}

			if (data.Condition == 3)
			{
				Condition.text = CommonUtil.GetFormatTime(currentInfo.Data.TimeLimit);
			}
			if (CompletedFlag != null) {
				CompletedFlag.SetActive(isCompleted);
			}
            if(data.Reward.Data == null)
            {
                Debug.LogError(data.Condition);
            }
            else
            {
                Icon.sprite = data.Reward.Data.Icon.Sprite;
            }
			Amount.text = "X " + data.Reward.Amount.ToString();
		}
	}

}

