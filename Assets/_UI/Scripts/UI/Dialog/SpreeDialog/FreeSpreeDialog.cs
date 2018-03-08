
using GameClient;

namespace GameUI
{
	public class FreeSpreeDialog : SpreeDialog {

		#region base
		public const string UIPrefabPath = "UI/Dialog/SpreeDialog/FreeSpreeDialog";

		public static string[] UINames =
		{
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath
		};

		public static void Show(SpreeData data) {
			FreeSpreeDialog ins = ModMenu.Ins.Overlay(UINames, "")[1].Instance.GetComponent<FreeSpreeDialog>();
			ins._data = data;
			ins.Init();
		}
		#endregion
		
		public void OnBtnOkClick()
		{
			Client.Item.GetRewards(_data.AwardList,true);
			BuySuccessTip.Show(_data.AwardList);
			ModMenu.Ins.Back();
			Client.Spree.OnSpreeShowOver(true, SpreeShowState.OverFree);
		}

		public void OnBtnBackClick() {
			ModMenu.Ins.Back();
			Client.Spree.OnSpreeShowOver(true, SpreeShowState.OverFree);
		}
	}

}

