using XUI;

namespace GameUI {
	public class QRCodeAdDialog :UIStackBehaviour {

		public const string PATH = "UI/Dialog/QRCodeAdDialog";

		public static void Show() {
			string[] paths=new string[]{PATH};
			ModMenu.Ins.Overlay(paths);
		}

		public void __OnClick() {
			ModMenu.Ins.Back(true);
		}

	}
}