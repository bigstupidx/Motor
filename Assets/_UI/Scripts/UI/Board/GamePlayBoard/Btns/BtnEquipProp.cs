using System;
using UnityEngine;
using Game;
using GameClient;
using GameUI;
using UnityEngine.UI;

public class BtnEquipProp : MonoBehaviour {
	public Image CDMask;
	public Image Icon;
	public Text Count;
	public GameObject Add;
	public bool isCD;
	private float cdTime;
	private float timer;

	void OnEnable() {
		if (Client.User.UserInfo.EquipedPropList.Count > 0) {
			Add.SetActive(false);
			Icon.gameObject.SetActive(true);
			var prop = Client.Prop.GetPropInfo(Client.User.UserInfo.EquipedPropList[0]);
			Icon.sprite = prop.Data.Icon.Sprite;
			Count.text = prop.Amount.ToString();
			cdTime = prop.PropData.CDTime;

			if (!GamePlayBoard.Inited) {
				timer = cdTime;
				isCD = false;
			}
		} else {
			Icon.gameObject.SetActive(false);
			Add.SetActive(true);
			Count.text = "0";
		}

	}

	public void SetAmount(int i) {
		Count.text = i.ToString();
	}

	public void OnClick() {
		if (Time.timeScale == 0) {
			return;
		}
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming) {
			return;
		}

		if (Client.User.UserInfo.EquipedPropList.Count == 0) {
			GameModeBase.Ins.Pause();
			PropBoard.Show();
			return;
		}
		if (!isCD) {
			var prop = Client.Prop.GetPropInfo(Client.User.UserInfo.EquipedPropList[0]);
			if (prop.Amount > 0) {
				if (BikeManager.Ins.CurrentBike.bikeProp.Use(prop.PropData.PropType)) {
					prop.ChangeAmount(-1);
					SetAmount(prop.Amount);
					timer = cdTime;
					isCD = true;
				}
			} else {
				Client.Spree.OnArriveShowPoint(ShowPoint.GameBuyProp, (state) => {
					if (state == SpreeShowState.BuyRMB) {
						var item = Client.Prop.GetPropInfo(Client.User.UserInfo.EquipedPropList[0]);
						SetAmount(item.Amount);
					} else {
						if (Client.Prop.GamingBuy) {
							GameModeBase.Ins.Pause();
							SupplyDialog.Show(prop.Data.ID);
						} else {
							CommonTip.Show("道具不足");
						}
					}

				});
			}
		}
	}

	void Update() {
		if (isCD) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				isCD = false;
				timer = 0;
			}

			CDMask.fillAmount = timer / cdTime;
		} else {
			if (Math.Abs(CDMask.fillAmount) > 1e-6) {
				CDMask.fillAmount = 0;
			}
		}
	}
}
