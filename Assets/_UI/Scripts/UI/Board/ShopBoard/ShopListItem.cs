using System;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class ShopListItem : EnhancedScrollerCellView {

		public Text Name;
		public Image Icon;
		public Image CostIcon;
		public Text Cost;
		public GameObject Hot;
		public Text Rebate;
		private IAPData _data;
		public TweenCanvasGroupAlpha tweenalpha;
		public void SetData(int index, IAPData data) {
			tweenalpha.ResetToBeginning();
			tweenalpha.PlayForward();
			this._data = data;
			Icon.sprite = data.Icon.Sprite;
			Name.text = data.Name + " x" + data.Amount;
			if (_data.Type == IAPType.Stamina) {
				//				Name.text = data.Amount + " " + data.Name;
				CostIcon.sprite = data.Currency.Icon.Sprite;
				CostIcon.gameObject.SetActive(true);
				Cost.text = data.CurrencyAmount.ToString();
			} else {
				CostIcon.gameObject.SetActive(false);
				if (SDKManager.Instance.IsSupport("getProductInfo")) {
					string s = SDKManager.Instance.GetProductInfo(data.PayCode);
					char[] c = "|".ToCharArray();
					string[] info = s.Split(c);
					//					Name.text = info[0];
					Cost.text = info[1];
				} else {
					//					Name.text = data.Amount + " " + data.Name;
					Cost.text = "￥" + data.CurrencyAmount.ToString();
				}
			}
			Hot.SetActive(data.IsHot);
			Rebate.gameObject.SetActive(true);
			if (data.Rebate == 0) {
				Rebate.text = LString.ShopListItem_BG_Top_Rebate_noSale.ToLocalized();
			} else {
				Rebate.text = LString.ShopListItem_BG_Top_Rebate_Text__1__Text.ToLocalized() + " " + data.Rebate + "%";
			}
		}

		public void OnBtnBuyClick() {
			if (Client.IAP.BuyConfirm) {
				CommonDialog.Show((LString.GAMEUI_SHOPLISTITEM_ONBTNBUYCLICK_1).ToLocalized(),
					String.Format(LString.GAMEUI_SHOPLISTITEM_ONBTNBUYCLICK_2.ToLocalized(), Cost.text, Name.text),
				(LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_2).ToLocalized(),
				(LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_3).ToLocalized(),
				() => {
					Client.IAP.Buy(_data);

				}, null);
			} else {
				Client.IAP.Buy(_data);
			}

		}

	}

}

