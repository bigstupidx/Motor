using UnityEngine;
using GameClient;
using UnityEngine.UI;

namespace GameUI
{
    public class TipItem : MonoBehaviour
    {
        public Text Txt;
        public TweenCanvasGroupAlpha tweAlpha;
        public TweenPosition twePos;

        public void Show(RewardItemInfo info)
        {
            Txt.text =(LString.GAMEUI_TIPITEM_SHOW).ToLocalized() + info.Data.Name + "x" + info.Amount;
            tweAlpha.ResetToBeginning();
            twePos.ResetToBeginning();
            tweAlpha.PlayForward();
            twePos.PlayForward();
        }
    }


}
