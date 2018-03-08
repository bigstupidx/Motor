using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using GameUI;
using UnityEngine.UI;
using XPlugin.Data.Json;
using XUI;

public class RedeemCodeDialog : MonoBehaviour, IUIPoolBehaviour {

	#region base
	public const string UIPrefabPath = "UI/Dialog/RedeemCodeDialog";

	public static string[] UIPrefabNames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath,
		};

	public static void Show() {
		ModMenu.Ins.Overlay(UIPrefabNames);
	}

	public void OnUISpawned() {
		Input.text = "";
		Tip.text = "";
	}

	public void OnUIDespawn() {
	}
	#endregion

	public InputField Input;
	public Text Tip;

	public void OnBtnOKClick() {
		Tip.text = "";
		if (string.IsNullOrEmpty(Input.text)) {
			Tip.text = (LString.REDEEMCODEDIALOG_ONBTNOKCLICK).ToLocalized();
			return;
		}

		WaittingTip.Show((LString.REDEEMCODEDIALOG_ONBTNOKCLICK_1).ToLocalized());
		GetChangeReward(Input.text, (code) => {
			WaittingTip.Hide();
			switch (code) {
				case -1:
					Tip.text = (LString.REDEEMCODEDIALOG_ONBTNOKCLICK_2).ToLocalized();
					break;
				case 0:
					Tip.text = (LString.REDEEMCODEDIALOG_ONBTNOKCLICK_3).ToLocalized();
					break;
				case 10001:
					Tip.text = (LString.REDEEMCODEDIALOG_ONBTNOKCLICK_4).ToLocalized();
					break;
				default:
					Tip.text = (LString.REDEEMCODEDIALOG_ONBTNOKCLICK_5).ToLocalized();
					break;
			}
		});
	}

	public void OnBtnCloseClick() {
		ModMenu.Ins.Back();
	}

	public static void GetChangeReward(string exchangeCode, Action<int> ondone = null) {

		int code = -1;
		WebManager.Ins.AddItem(new WebItem() {
			A = "UseConvert",
			P = new JArray(new[] { exchangeCode }),
			M = "Convert",
			Callback = (callback) => {
				if (WebManager.Ins.isShowLog) {
					Debug.Log("[UseConvert] " + callback.Success + " - " + callback.CallBackType + " content:" + callback.content);
				}
				if (callback.CallBackType == WebCallBackType.Success) {
					JObject root = JObject.Parse(callback.content);
					code = root["code"].AsInt();
					if (code == 0) {

						JArray data = (JArray)root["result"];
						var list = new List<RewardItemInfo>();

						for (int i = 0; i < data.Count; i++) {
							var rewards = (JArray)data[i];
							int id = int.Parse(rewards[0].ToString());
							int amount = int.Parse(rewards[1].ToString());
							list.Add(new RewardItemInfo(id, amount));
						}
						Client.Item.GetRewards(list, true);
						ModMenu.Ins.Back();
						RewardDialog.Show(list);
					}
					if (ondone != null) {
						ondone(code);
					}
				} else {
					if (ondone != null) {
						ondone(code);
					}
					Debug.LogError("GetServerInfo request fail " + callback.Success + " " + callback.CallBackType + " content:" +
					               callback.content);
				}
			}
		});
	}
}
