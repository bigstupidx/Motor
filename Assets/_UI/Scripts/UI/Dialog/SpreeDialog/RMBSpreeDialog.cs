
using System.Collections;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class RMBSpreeDialog : SpreeDialog {
		#region base
		public const string UIPrefabPath = "UI/Dialog/SpreeDialog/RMBSpreeDialog";

		public static string[] UINames =
		{
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath
	};

		public static void Show(SpreeData data, bool isPopups) {
			RMBSpreeDialog ins = ModMenu.Ins.Overlay(UINames, "")[1].Instance.GetComponent<RMBSpreeDialog>();
			ins._data = data;
			ins.isPopups = isPopups;
			ins.Init();
			if (Client.Spree.AutoBuy)
			{
				ins.AutoBuy();
			}
			
		}
		#endregion

		public Text Cost;

		private bool isPopups = false;

		public override void Init()
		{
			base.Init();
			Cost.text = _data.PayValue/100f + (LString.GAMEUI_SHOPLISTITEM_ONBTNBUYCLICK).ToLocalized();
		}

		private void AutoBuy()
		{
			StartCoroutine(DelayBuy());
		}

		private IEnumerator DelayBuy()
		{
			yield return new WaitForSeconds(0.7f);
			Buy();
		}

		private void Buy()
		{
			Client.Spree.BuySpree(_data, (b) => {
				if (b)
				{
					ModMenu.Ins.Back();
					if (isPopups)
					{
						Client.Spree.OnSpreeShowOver(false, SpreeShowState.BuyRMB);
					}
					
					ShowResult();
				}
				else
				{
					CommonTip.Show((LString.GAMEUI_RMBSPREEDIALOG_BUY).ToLocalized());
				}
			});
		}

		public void OnBtnOkClick()
		{
			float cost = 0f;
			cost = _data.PayValue/100f;

			if (Client.IAP.BuyConfirm)
			{
				CommonDialog.Show((LString.GAMEUI_SHOPLISTITEM_ONBTNBUYCLICK_1).ToLocalized(),
				(LString.GAMEUI_RMBSPREEDIALOG_ONBTNOKCLICK).ToLocalized() + cost + (LString.GAMEUI_RMBSPREEDIALOG_ONBTNOKCLICK_1).ToLocalized() + _data.Name + (LString.GAMEUI_RMBSPREEDIALOG_ONBTNOKCLICK_2).ToLocalized(),
				(LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_2).ToLocalized(),
				(LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_3).ToLocalized(),
				() => {
					Buy();

				}, null);
			}
			else
			{
				Buy();
			}

		}

		public void OnBtnBackClick() {
			ModMenu.Ins.Back();
			if (isPopups)
			{
				Client.Spree.OnSpreeShowOver(false, SpreeShowState.RefuseRMB);
			}
		}

		private void ShowResult()
		{
			//礼包中是否包含车或人
			bool bikeIn = false;
			bool heroIn = false;
			foreach (var award in _data.AwardList)
			{
				if (award.Data.Type == ItemType.Bike)
				{
					bikeIn = true;
				}

				if (award.Data.Type == ItemType.Hero)
				{
					heroIn = true;
				}

			}
			if (bikeIn)
			{
				RewardDialog.Show(_data.AwardList, GoToSee.GarageBoard);
				return;
			}
			if (heroIn)
			{
				RewardDialog.Show(_data.AwardList, GoToSee.HeroBoard);
				return;
			}

			//显示tip
			BuySuccessTip.Show(_data.AwardList);
		}
	}


}
