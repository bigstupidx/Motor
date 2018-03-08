using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{

    public class RewardItem : MonoBehaviour
    {
        public Image Icon;
        public Text Count;
        public Text IconName;
        public UITweener[] Tweeners;

        public void SetData(ItemData img, int val)
        {
            if (img.ID == 1001)
            {
                Icon.sprite = AtlasManager.GetSprite("Icon", "Icon_Coin_1");
            }
            else if (img.ID == 1000)
            {
                Icon.sprite = AtlasManager.GetSprite("Icon", "Icon_Diamond_2");
            }
            else
            {
                Icon.sprite = img.Icon.Sprite;
            }
            if (IconName != null)
            {
                IconName.text = (LString.GAMEUI_REWARDITEM_SETDATA).ToLocalized() + img.Name;
            }
            Count.text = "x" + val.ToString();

        }
    }
}
