using GameClient;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Localization;

namespace GameUI {
	public class MainBoardTopBtnList : MonoBehaviour {

		public RectTransform RectTrans;

		public GameObject MoreGame;
		public GameObject About;
		public GameObject Service;
		public GameObject Exchange;
		public GameObject Rank;
		public GameObject Sign;
		public GameObject FirstSpree;
		public GameObject SensorSpree;
		public GameObject InvitePhone;

		[Header("小红点")]
		public GameObject SignHit;
		public GameObject FirstHit;
		public GameObject SensorSpreeHit;

		public void Resize() {
			int activeChildCount = 0;
			foreach (Transform child in transform) {
				if (child.gameObject.activeSelf) {
					activeChildCount++;
				}
			}

            if(Localization.Language == LanguageEnum.en_US)
            {
                RectTrans.sizeDelta = new Vector2(activeChildCount * 170 + (activeChildCount + 1) * 4, RectTrans.sizeDelta.y);
            }else
            {
                RectTrans.sizeDelta = new Vector2(activeChildCount * 110 + (activeChildCount + 1) * 4, RectTrans.sizeDelta.y);
            }
		}

		public void SetTopBtns() {
			MoreGame.SetActive(SDKManager.Instance.IsSupport("moreGame"));
			Exchange.SetActive(Client.Spree.ShowRedeemCode);
			About.SetActive(Client.Config.OpenAbout);
			Service.SetActive(Client.Config.ShowService);
			this.InvitePhone.SetActive(Client.Config.ShowInvite);

			SensorSpree.SetActive(Client.Config.SensorSpree);
			if (Client.Config.SensorSpree) {
				this.SensorSpreeHit.SetActive(Client.SensorSpree.CanReceive() && BikeOrbecController.HasOrbbecDevice());
			}

			//签到按钮
			this.Sign.SetActive(Client.Sign.IsSignOpen);
			if (Client.Sign.IsSignOpen) {
				this.SignHit.SetActive(Client.Sign.CanSign());//小红点
			}

			//首充礼包按钮
			if (Client.Spree.ShowFirstSpree && !Client.User.UserInfo.Spree.IsGetFirstSpree) {
				this.FirstSpree.SetActive(true);
				this.FirstHit.SetActive(Client.User.UserInfo.Spree.CanGetFirstSpree);//小红点
			} else {
				this.FirstSpree.SetActive(false);
			}

			this.Resize();
		}

		// 兑换码
		public void OnBtnRedeemCodeClick() {
			RedeemCodeDialog.Show();
		}

		//签到
		public void OnBtnSignClick() {
			SignBoard.Show();
		}

		//客服
		public void OnBtnServiceClick() {
			ServiceDialog.Show(Client.System.ServiceInfo);
		}

		//关于
		public void OnBtnAboutClick() {
			AboutDialog.Show();
		}

		public void OnBtnSensorSpreeClick() {
			SensorSpreeDialog.Show();
		}

		public void OnInvitePhoneClick() {
			if (!Interface.isNetworkConnected()) {
				CommonTip.Show(LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT.ToLocalized());
				return;
			}
			WaittingTip.Show();
			WebManager.Ins.AddItem(new WebItem() {
				M = "Invite",
				A = "GetNewInviteCode",
				P = new JArray(),
				Callback = item => {
					WaittingTip.Hide();
					if (item.Success && item.CallBackType == WebCallBackType.Success) {
						JObject root = JObject.Parse(item.content);
						if (root["code"].AsInt() == 0) {
							JArray result = root["result"].AsArray();
							string code = result[0].AsString();
							string url = Client.Config.WebHost + "/index.php/Admin/Invite/Index";
							Texture2D qrTexture = QRGenerator.EncodeString(string.Format(url, code));
							InvitePhoneDialog.Show(code, qrTexture);
						} else {
							CommonTip.Show(LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT.ToLocalized());
						}
					} else {
						CommonTip.Show(LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT.ToLocalized());
					}
				}
			});

		}

		public void OnBtnMoreGameClick() {
			CommonDialog.Show((LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK).ToLocalized(), (LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK_1).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, null, null);
			//SDKManager.MoreGame ();
		}

		public void OnBtnRankClick() {
			if (Client.Online.CheckNetwork()) {
				WaittingTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK).ToLocalized());
				Client.Rank.GetRank((b) => {
					WaittingTip.Hide();
					if (b) {
						if (Client.Rank.RankList.Count == 0) {
							CommonTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK_1).ToLocalized());
							return;
						}
						RankBoard.Show(Client.Rank.GetRankInfo());
					} else {
						CommonTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK_2).ToLocalized());
					}
				});
			}
		}

	}
}