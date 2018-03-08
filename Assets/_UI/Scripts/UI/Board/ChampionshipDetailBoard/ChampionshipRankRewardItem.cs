using GameClient;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace GameUI
{

    public class ChampionshipRankRewardItem : EnhancedScrollerCellView
    {
        public Text Condition;
        public Image Icon;
        public Text Amount;
        public Text Dec;
        public GameObject CompletedFlag;
        public Image Bg;
        public TweenCanvasGroupAlpha tweenalpha;
        public int DataIndex { get; private set; }
        public void SetData(int index, ChampionshipRewardData data)
        {
            tweenalpha.ResetToBeginning();
            tweenalpha.PlayForward();
            var currentInfo = ChampionshipDetailBoard.Ins.currentInfo;
            bool isCompleted = false;
            DataIndex = index;
                Condition.text = (LString.GAMEUI_CHAMPIONSHIPRANKREWARDITEM).ToLocalized() + data.Rank;
                Dec.text = (LString.GAMEUI_CHAMPIONSHIPRANKREWARDITEM_1).ToLocalized() + data.Rank + (LString.GAMEUI_CHAMPIONSHIPRANKREWARDITEM_2).ToLocalized();
                isCompleted = (currentInfo.Rank != -1 && currentInfo.Rank <= data.Rank);

            if (data.Condition == 3)
            {
                Condition.text = CommonUtil.GetFormatTime(currentInfo.Data.TimeLimit);
                Dec.text = (LString.GAMEUI_CHAMPIONSHIPRANKREWARDITEM_3).ToLocalized();
            }
			if (CompletedFlag != null){
				CompletedFlag.SetActive(isCompleted);
			} 
            if (data.Reward.Data == null)
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
