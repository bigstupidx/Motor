using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using GameClient;
using System.Collections.Generic;

namespace GameUI {
	public class GarageBoard : Singleton<GarageBoard> {

		#region base

		public const string UIPrefabPath = "UI/Board/GarageBoard/GarageBoard";

		public static string[] UIPrefabNames =
		{
				UIPrefabPath,
				UICommonItem.TOP_BOARD
		};

		private static bool _CurrentShow;

		public static void Show(int bikeId) {
			if (_CurrentShow) {
				GarageBoard.Ins.Init(GarageBoard.Ins.CurrentShowBike.Info.Data.ID);
			} else {
				GarageBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "GarageBoard")[0].Instance.GetComponent<GarageBoard>();
				ins.Init(bikeId);
				Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "OnGarageBoardInited");
			}
		}

		void OnUISpawned() {
			_CurrentShow = true;
			foreach (var t in this.Ts) {
				t.ResetToBeginning();
				t.PlayForward();
			}
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.BikeBoard);
			if (this.CurrentShowBike != null) {
				ModelShow.Ins.ShowBike(this.CurrentShowBike.Info.Data.Prefab);
			}
		}

		void OnUIDeOverlay() {
			Refresh();
		}

		void OnUIDeCover() {
			Refresh();
		}

		void OnUIDespawn() {
			_CurrentShow = false;
		}

		#endregion

		public UITweener[] Ts;

		public Text Name;
		public Image Grade;
		public Text powerScore;
		public Text BuyCost;
		public Image BuyCostIcon;
		public GameObject BuyBtn;
		public GameObject BtnUpdate;
		//public CommonBtn btnSelect;
		public GameObject btnSelect;
		public Text SelectText;

		public List<BikeListItemData> _data;
		public List<BikeAttributeItem> attItems = new List<BikeAttributeItem>();

		public BikeListItemData CurrentShowBike;
		private int CurrentSelectDataIndex = -1;
		public MotorSelectedIconManager selectIconManager;


		private bool canSelect = true;

		public void Init(int bikeId) {
			Client.Bike.SetRedPointShowed(bikeId);
			//this.Scroller.Delegate = this;
			Reload();
			this.canSelect = true;

			for (int i = 0; i < this._data.Count; i++) {
				if (this._data[i].Info.Data.ID == bikeId) {
					this.CurrentSelectDataIndex = i;
					this._data[i].Selected = true;
					this.CurrentShowBike = _data[i];
				} else {
					this._data[i].Selected = false;
				}

			}
			selectIconManager.InitSelectInfo(_data.Count, CurrentSelectDataIndex, _data);
			ShowBike();
			UpdateBtnSpree();
		}



		public void Refresh() {
			Reload();
			UpdateBikeInfo();
			UpdateBtnSpree();
		}

		private void UpdateBtnSpree() {
			var data = Client.Spree.GetSpreeDataByShowType(ShowType.BikeBoard);
		}

		public void ShowBike() {
			CurrentShowBike = _data[CurrentSelectDataIndex];
			UpdateBikeInfo();
			ModelShow.Ins.ShowBike(this.CurrentShowBike.Info.Data.Prefab);
			SetBikeInfo();

			JumpTo(CurrentSelectDataIndex);
		}

		public void ShowBike(int index) {
			CurrentSelectDataIndex = index;
			CurrentShowBike = _data[CurrentSelectDataIndex];
			UpdateBikeInfo();
			SetBikeInfo();
			selectIconManager.Refresh(index);
			JumpTo(CurrentSelectDataIndex);
		}

		public void JumpTo(int index) {
			canSelect = false;
			int i = index;
			float sOffset = 0.5f;
			float cOffset = 0.5f;
			if (CurrentSelectDataIndex < 2) {
				i = 0;
				sOffset = 0;
				cOffset = 0;
			} else if (CurrentSelectDataIndex > _data.Count - 3) {
				i = _data.Count - 1;
				sOffset = 1;
				cOffset = 1;
			}
		}

		public void SetBikeInfo() {
			this.Grade.sprite = UIDataDef.Get_Bike_Rank_Icon(this.CurrentShowBike.Info.Data.Rank);
			if (this.CurrentShowBike.Info.isUnLock) {
				this.BuyBtn.SetActive(false);
				this.btnSelect.gameObject.SetActive(true);
				this.BtnUpdate.gameObject.SetActive(true);
				if (this.CurrentShowBike.Info.Data.ID == Client.User.UserInfo.ChoosedBikeID) {
					this.SelectText.text = (LString.GAMEUI_GARAGEBOARD_SHOWBIKE).ToLocalized();

				} else {
					this.SelectText.text = (LString.GAMEUI_GARAGEBOARD_SHOWBIKE_1).ToLocalized();
				}
			} else {
				this.BtnUpdate.gameObject.SetActive(false);
				this.btnSelect.gameObject.SetActive(false);
				this.BuyBtn.SetActive(true);
				this.BuyCost.text = this.CurrentShowBike.Info.Data.CostAmount.ToString();
				this.BuyCostIcon.sprite = this.CurrentShowBike.Info.Data.Cost.Icon.Sprite;
			}
			this.Name.text = this.CurrentShowBike.Info.Data.Name;
		}

		public void UpdateBikeInfo() {
			this.CurrentShowBike.Info = Client.Bike.GetBikeInfo(this.CurrentShowBike.Info.Data.ID);
			powerScore.text = Client.Bike.GetBikePowerSocre(this.CurrentShowBike.Info.Data.ID).ToString();
			for (int i = 0; i < this._data.Count; i++) {
				this._data[i].Selected = (this.CurrentSelectDataIndex == i);
				if (this._data[i].Info.Data.ID == this.CurrentShowBike.Info.Data.ID) {
					this._data[i].Info = this.CurrentShowBike.Info;
					this._data[i].isUnLock = this.CurrentShowBike.Info.isUnLock;
				}
			}
			for (int i = 0; i < attItems.Count; ++i) {
				attItems[i].Refresh(this.CurrentShowBike.Info);
			}
		}

		public void OnBtnBuyClick() {

			Buy();
			//Client.Spree.OnArriveShowPoint (ShowPoint.BuyBike, (state) => {
			//	if (state == SpreeShowState.BuyRMB) {
			//		UpdateBikeInfo ();
			//		SetBikeInfo ();
			//	} else if (state == SpreeShowState.RefuseRMB || state == SpreeShowState.NoRMB) {
			//		Buy ();
			//	}
			//});

		}

		private void Buy() {
			if (Client.Bike.BuyBike(this.CurrentShowBike.Info.Data.ID)) {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Buy);
				CommonTip.Show((LString.BIKEBOARD_BUY).ToLocalized());
				UpdateBikeInfo();
				SetBikeInfo();
				Client.Bike.SetRedPointShowed(this.CurrentShowBike.Info.Data.ID);
			} else {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				CommonTip.Show(this.CurrentShowBike.Info.Data.Cost.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
				Client.Spree.OnArriveShowPoint(ShowPoint.BuyBikeFail, (state) => {
					if (state == SpreeShowState.NoRMB) {
						Client.IAP.ShowShopBoardForSupply(this.CurrentShowBike.Info.Data.Cost.ID);
					}
				});
			}
		}

		public void OnBtnChooseClick() {
			if (!this.CurrentShowBike.Info.isUnLock) {
				return;
			}
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Equip);
			Client.User.UserInfo.ChoosedBikeID = this.CurrentShowBike.Info.Data.ID;
			this.SelectText.text = (LString.GAMEUI_GARAGEBOARD_SHOWBIKE).ToLocalized();
		}

		public void OnBtnNextClick() {
			//if (!this.canSelect) {
			//	return;
			//}
			ModelShow.Ins.ShowNext(CurrentSelectDataIndex);
		}

		public void OnBtnPreClick() {
			//if (!this.canSelect) {
			//	return;
			//}
			ModelShow.Ins.ShowPre(CurrentSelectDataIndex);
		}

		public void OnBtnUpdateClick() {
			MotorUpdateBoard.Show(CurrentShowBike.Info.Data.ID);
		}

		public void Reload() {
			this._data = new List<BikeListItemData>();

			foreach (var bike in Client.Bike.GetSortedDatas()) {
				BikeInfo info = Client.Bike.GetBikeInfo(bike.ID);
				if (info == null) {
					info = new BikeInfo(Client.Bike[bike.ID]);
				}
				this._data.Add(new BikeListItemData(info));
			}
		}
	}
}
