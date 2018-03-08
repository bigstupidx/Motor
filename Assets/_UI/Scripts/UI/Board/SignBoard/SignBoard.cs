using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class SignBoard : SingleUIStackBehaviour<SignBoard> {

		public static void Show() {
			string[] UIPrefabNames ={
				UICommonItem.MENU_BACKGROUND,
				"UI/Board/SignBoard/SignBoard",
				UICommonItem.TOP_BOARD_BACK
			};
			ModMenu.Ins.Cover(UIPrefabNames);
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			Init();
		}

		public override void OnUIDeOverlay() {
			Init();
		}

		public SignItem[] Items;

		public void Init() {
			int schedule = Client.Sign.Schedule;
			if (!Client.Sign.CanSign()) {
				schedule -= 1;
			}
			for (int i = 0; i < Items.Length; i++) {
				SignData data = Client.Sign[i];

				//极端情况判断
				if (schedule < 0 && i == 0) {
					Items[0].SetData(data, SignState.Current);
				} else if (schedule >= 6 && i == 6) {
					Items[6].SetData(data, SignState.Current);
				} else if (i == schedule + 1) {
					Items[i].SetData(data, SignState.Current);
				} else if (i <= schedule) {
					Items[i].SetData(data, SignState.Already);
				} else if (i > schedule + 1) {
					Items[i].SetData(data, SignState.Normal);
				}
			}
		}

		public void ShowRule(bool show) {
			if (show) {
				SignRuleDialog.Show(Client.Sign.SignHelp);
			} else {
				ModMenu.Ins.Back();
			}
		}

	}

}

