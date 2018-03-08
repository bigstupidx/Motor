using GameClient;
using GameUI;

public class FirstSpreeDialog : SpreeDialog {

	#region base
	public const string UIPrefabPath = "UI/Dialog/SpreeDialog/FirstSpreeDialog";

	public static void Show(SpreeData data) {
		string[] UINames ={
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath
		};
		FirstSpreeDialog ins = ModMenu.Ins.Overlay(UINames)[1].Instance.GetComponent<FirstSpreeDialog>();
		ins._data = data;
		ins.Init();
	}

	void OnUIDeOverlay() {
		Init();
	}

	void OnUIDeCover() {
		Init();
	}
	#endregion

	public override void SetBtnName() {
		BtnName.text = Client.User.UserInfo.Spree.CanGetFirstSpree ? (LString.FIRSTSPREEDIALOG_SETBTNNAME).ToLocalized() : (LString.FIRSTSPREEDIALOG_SETBTNNAME_1).ToLocalized();
	}

	public void OnBtnClick() {
		if (Client.User.UserInfo.Spree.CanGetFirstSpree) {
			Client.Item.GetRewards(_data.AwardList, true);
			Client.User.UserInfo.Spree.IsGetFirstSpree = true;
			var list = _data.AwardList;
			ModMenu.Ins.Back();
			RewardDialog.Show(list);
		} else {
			ShopBoard.Show(IAPType.Diamond);
		}
	}
}
