using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XPlugin.Data.Json;
using XUI;

namespace GameUI {
	public class InputInviteCodeDialog : UIStackBehaviour {

		public static void Show() {
			ModMenu.Ins.Overlay(new string[] {
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				"UI/Dialog/InputInviteCodeDialog"
			});
		}

		public InputField Input;

		public void __OnConfirmClick() {
			string input = this.Input.text;
			if (string.IsNullOrEmpty(input)) {
				return;
			}
			WaittingTip.Show();
			StartCoroutine(Test(input));
		}

		IEnumerator Test(string code) {
			Debug.Log("验证邀请码");

			if (!Interface.isNetworkConnected()) {
				CommonTip.Show(LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT.ToLocalized());
				yield break;
			}

			var p = new JArray();
			p.Add(code);

			WebManager.Ins.AddItem(new WebItem() {
				M = "Invite",
				A = "CheckInviteCode",
				P = p,
				Callback = item => {
					WaittingTip.Hide();
					if (item.Success && item.CallBackType == WebCallBackType.Success) {

						JObject root = JObject.Parse(item.content);
						if (root["code"].AsInt() == 0) {
							PlayerPrefs.SetInt("AlreadyInvited", 1);
							PlayerPrefs.Save();
							ModMenu.Ins.Back(true);
						} else {
							CommonTip.Show("验证码错误");
						}

					} else {
						CommonTip.Show(LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT.ToLocalized());
					}
				}
			});

		}

	}
}