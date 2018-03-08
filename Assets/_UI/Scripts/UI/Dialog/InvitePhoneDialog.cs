using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class InvitePhoneDialog : UIStackBehaviour {

		public Text Text;
		public RawImage QRCodeImage;

		public static void Show(string code, Texture2D qrTexture) {
			InvitePhoneDialog ins=ModMenu.Ins.Overlay(new string[] {
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				"UI/Dialog/InvitePhoneDialog"
			})[1].Instance.GetComponent<InvitePhoneDialog>();
			ins.Text.text = code;
			ins.QRCodeImage.texture = qrTexture;
		}
	}
}