using UnityEngine;
using System.Collections;
using GameClient;
using GameUI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameUI {
	public class MotorUpdateBoard : Singleton<MotorUpdateBoard> {

		#region base

		public const string UIPrefabPath = "UI/Board/MotorUpdateBoard/MotorUpdateBoard";

		public static string[] UIPrefabNames =
		{
				UIPrefabPath,
				UICommonItem.TOP_BOARD
	};


		public static void Show(int bikeId) {

			MotorUpdateBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "MotorUpdateBoard")[0].Instance.GetComponent<MotorUpdateBoard>();
			ins.Init(bikeId);
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "OnMotorUpdateBoardInited");
		}

		void OnUISpawned() {
			foreach (var t in this.Ts) {
				t.ResetToBeginning();
				t.PlayForward();
			}
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.MotorUpdateBoard);
			if (this.bikeInfo != null) {
				ModelShow.Ins.ShowBike(this.bikeInfo.Data.Prefab);
			}
		}

		void OnUIDeOverlay() {
			Refresh();
		}

		void OnUIDeCover() {
			Refresh();
		}

		void OnUIDespawn() {
		}

		#endregion
		public UITweener[] Ts;
		public BikeUpgradeItem[] BikeUpgradeItems;

		public Text Name;
		public Text powScore;
		public Image Grade;
		public GameObject Max;
		public GameObject BtnSpree;

		public BikeInfo bikeInfo;
		public GameObject MaxButton;
		public Image Cost;
		public Text CostNum;
		public List<BikeAttributeItem> attItems = new List<BikeAttributeItem>();
		public void Init(int bikeId) {
			bikeInfo = Client.Bike.GetBikeInfo(bikeId);
			ShowBike();
			UpdateBtnSpree();
		}

		public void Refresh() {
			UpdateBtnSpree();
		}

		private void UpdateBtnSpree() {
			//var data = Client.Spree.GetSpreeDataByShowType (ShowType.BikeBoard);
			//BtnSpree.SetActive (Client.Spree.ShouldShow (data));
		}

		public void ShowBike() {
			ModelShow.Ins.ShowBike(this.bikeInfo.Data.Prefab);
			SetBikeInfo();
		}

		public void SetBikeInfo() {
			this.Grade.sprite = UIDataDef.Get_Bike_Rank_Icon(this.bikeInfo.Data.Rank);
			this.Name.text = this.bikeInfo.Data.Name;
			this.powScore.text = Client.Bike.GetBikePowerSocre(bikeInfo.Data.ID).ToString();
			foreach (var i in this.BikeUpgradeItems) {
				i.Refresh();
			}
			if (Client.Bike.IsMaxBike(bikeInfo.Data.ID)) {
				MaxButton.SetActive(false);
			} else {
				MaxButton.SetActive(true);
			}
			Cost.sprite = bikeInfo.Data.MaxCost.Icon.Sprite;
			CostNum.text = bikeInfo.Data.MaxCostNum.ToString();
			for (int i = 0; i < attItems.Count; ++i) {
				attItems[i].Refresh(bikeInfo);
			}
		}

		public void UpdatePowScore() {
			this.powScore.text = Client.Bike.GetBikePowerSocre(bikeInfo.Data.ID).ToString();
		}

		public void UpdataMax() {
			var bikeInfo = this.bikeInfo;
			if (Client.Bike.UpgradeMaxBike(bikeInfo.Data.ID)) {
				ModelShow.Ins.ShowFastUpgradeEffect();
				SfxManager.Ins.PlayOneShot(SfxType.sfx_menu_upgrade);
				SetBikeInfo();
				UpdatePowScore();
			} else {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				CommonTip.Show(bikeInfo.Data.MaxCost.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
				Client.IAP.ShowShopBoardForSupply(bikeInfo.Data.MaxCost.ID);
			}
		}

		public void OnBtnChangeClick() {
			CommonDialog.Show((LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK).ToLocalized(), (LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK_1).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, null, null);
		}
	}
}
