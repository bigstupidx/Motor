using UnityEngine;
using Game;
using GameClient;
using UnityEngine.UI;

namespace GameUI {
	public class SupplyDialog : MonoBehaviour {

		#region base
		public const string UIPrefabPath = "UI/Dialog/SupplyDialog";

		public static string[] UINames =
		{
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath
		};

		public static void Show(int id) {
			SupplyDialog ins = ModMenu.Ins.Overlay(UINames)[1].Instance.GetComponent<SupplyDialog>();
			ins.Init(id);
		}
		#endregion

		public Text Title;
		public Text Amount;
		public Text Cost;
		public Image Icon;
		public Image CostIcon;
		public Button BtnSubtract;

		private PropInfo info;
		private int buyAmount = 1;
		private int buyCost = 0;

		public void Init(int id) {
			buyAmount = 1;
			buyCost = 0;
			info = Client.Prop.GetPropInfo(id);
			Title.text = info.Data.Name + (LString.BIKEBOARD_BUY_1).ToLocalized();
			Icon.sprite = info.Data.Icon.Sprite;
			CostIcon.sprite = info.PropData.Currency.Icon.Sprite;
			UpdateCostInfo();
			BtnSubtract.enabled = false;
		}

		public void OnBtnSubtractClick() {
			buyAmount -= 1;
			UpdateCostInfo();

			if (buyAmount < 2) {
				BtnSubtract.enabled = false;
			}

		}

		public void OnBtnPlusClick() {
			buyAmount += 1;
			UpdateCostInfo();
			if (!BtnSubtract.enabled) {
				BtnSubtract.enabled = true;
			}
		}

		public void OnBtnOKClick() {
			ItemInfo costItem = Client.Item.GetItem(info.PropData.Currency.ID);
			if (costItem.Amount < buyCost) {
				CommonTip.Show(info.PropData.Currency.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
				Client.IAP.ShowShopBoardForSupply(info.PropData.Currency.ID);
				return;
			}
			costItem.ChangeAmount(-buyCost);
			info.ChangeAmount(buyAmount);
			CommonTip.Show((LString.GAMEUI_SUPPLYDIALOG_ONBTNOKCLICK).ToLocalized());

			int amount = info.Amount;
			ModMenu.Ins.Back();
			GameModeBase.Ins.Resume();

			GamePlayBoard.Ins.GameControl.BtnEquipProp.SetAmount(amount);
		}

		public void OnBtnCancelClick() {
			ModMenu.Ins.Back();
			GameModeBase.Ins.Resume();
		}

		private void UpdateCostInfo() {
			buyCost = info.PropData.CurrencyAmount * buyAmount;
			Amount.text = buyAmount.ToString();
			Cost.text = buyCost.ToString();
		}
	}


}
