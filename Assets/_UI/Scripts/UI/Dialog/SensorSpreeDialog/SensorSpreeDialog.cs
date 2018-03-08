using GameClient;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class SensorSpreeDialog : UIStackBehaviour {
		public const string UIPrefabPath = "UI/Dialog/SensorSpreeDialog";
		public static void Show() {
			string[] UINames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath
			};
			ModMenu.Ins.Overlay(UINames);
		}


		public override void OnUISpawned() {
			base.OnUISpawned();
			for (var i = 0; i < Client.SensorSpree.rewardList.Count; i++) {
				var reward = Client.SensorSpree.rewardList[i];
				this.Count[i].text = reward.Amount.ToString();
			}

			this.Btn.interactable = Client.SensorSpree.CanReceive();
			if (!this.Btn.interactable) {
				this.BtnText.text = LString.SensorSpreeDialog_Received_Text.ToLocalized();// "已领取";
            } else {
                this.BtnText.text = LString.SensorSpreeDialog_Get_Text.ToLocalized();// "立即领取";
			}
		}

		public override void OnUIDeCover() {
			base.OnUIDeCover();
			OnUISpawned();
		}

		public override void OnUIDeOverlay() {
			base.OnUIDeOverlay();
			OnUISpawned();
		}

		public Text[] Count;

		public Button Btn;
		public Text BtnText;

		public void __GetClick() {
			if (!BikeOrbecController.HasOrbbecDevice()) {
				CommonDialog.Show(LString.GAMECLIENT_COMMON_ORBBEC_TITLE.ToLocalized(), LString.GAMECLIENT_COMMON_ORBBEC_CONTENT.ToLocalized(), LString.GAMECLIENT_COMMON_ORBBEC_CONFORM.ToLocalized(), () => QRCodeAdDialog.Show());
				return;
			}

			var list = Client.SensorSpree.Receive();
			if (list != null) {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Blink);
				RewardDialog.Show(list);
			} else {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
			}
		}

	}

}

