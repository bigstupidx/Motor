using UnityEngine;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine.UI;

namespace GameUI
{
	public class SpreeListItem : EnhancedScrollerCellView {

		public Text Name;
		public Image Icon;
		public Image CostIcon;
		public Text Cost;
		public GameObject Hot;
		private SpreeData _data;
        public TweenCanvasGroupAlpha tweenalpha;
        public void SetData(int index, SpreeData data) {
            tweenalpha.ResetToBeginning();
            tweenalpha.PlayForward();
            this._data = data;
			Name.text = data.Name;
			//CostIcon.sprite = Client.Item.RMBData.Icon.Sprite;
			CostIcon.gameObject.SetActive (false);
			float cost = data.PayValue / 100f;
			Cost.text = "￥" + cost.ToString();

			Hot.SetActive(data.IsHot);
			if (data.Icon != null)
			{
				Icon.sprite = data.Icon.Sprite;
			}
		}

		public void OnBtnBuyClick()
		{
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Open);
			CommonDialog.Show ((LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK).ToLocalized(), (LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK_1).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, null, null);
			return;
			RMBSpreeDialog.Show(_data, false);
		}
	}

}
