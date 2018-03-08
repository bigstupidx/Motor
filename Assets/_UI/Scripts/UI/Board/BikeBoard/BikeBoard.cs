using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using GameUI;
using UnityEngine.UI;

public class BikeBoard : Singleton<BikeBoard>, IEnhancedScrollerDelegate {
	#region base

	public const string UIPrefabPath = "UI/Board/BikeBoard/BikeBoard";

	public static string[] UIPrefabNames =
	{
				UIPrefabPath,
				UICommonItem.TOP_BOARD
	};

	private static bool _CurrentShow;

	public static void Show(int bikeId) {
		if (_CurrentShow) {
			BikeBoard.Ins.Init(BikeBoard.Ins.CurrentShowBike.Info.Data.ID);
		} else {
			BikeBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "BikeBoard")[0].Instance.GetComponent<BikeBoard>();
			ins.Init(bikeId);
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "OnBikeBoardInited");
		}
	}

	void OnUISpawned() {
		_CurrentShow = true;
		foreach (var t in this.Ts) {
			t.ResetToBeginning();
			t.PlayForward();
		}
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

	public BikeUpgradeItem[] BikeUpgradeItems;

	public Text Name;
	public Image Grade;
	public Text BuyCost;
	public Image BuyCostIcon;
	public GameObject BuyBtn;
	public CommonBtn BtnChoose;
	public Text AbilityText;
	public Image AbilityBg;
	public GameObject Max;

	public GameObject BtnSpree;

	public SmallList<BikeListItemData> _data;
	public EnhancedScroller Scroller;
	public EnhancedScrollerCellView CellViewPrefab;
	public float CellWidth;
	public float ScrollTime = 0.2f;

	public BikeListItemData CurrentShowBike;
	private int CurrentSelectDataIndex = -1;


	private bool canSelect = true;

	public void Init(int bikeId) {
		Client.Bike.SetRedPointShowed(bikeId);
		this.Scroller.Delegate = this;
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
		BtnSpree.SetActive(Client.Spree.ShouldShow(data));
	}

	public void ShowBike() {
		CurrentShowBike = _data[CurrentSelectDataIndex];
		UpdateBikeInfo();
		ModelShow.Ins.ShowBike(this.CurrentShowBike.Info.Data.Prefab);
		SetBikeInfo();

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

		Scroller.JumpToDataIndex(i, sOffset, cOffset, true, EnhancedScroller.TweenType.easeInOutSine, ScrollTime, () => {
			canSelect = true;
		});

	}

	public void SetBikeInfo() {
		this.Grade.sprite = UIDataDef.Get_Bike_Rank_Icon(this.CurrentShowBike.Info.Data.Rank);
		if (this.CurrentShowBike.Info.isUnLock) {
			this.BuyBtn.SetActive(false);
			this.BtnChoose.gameObject.SetActive(true);
			if (this.CurrentShowBike.Info.Data.ID == Client.User.UserInfo.ChoosedBikeID) {
				this.BtnChoose.SetEnable(false, (LString.BIKEBOARD_SETBIKEINFO).ToLocalized());

			} else {
				this.BtnChoose.SetEnable(true, (LString.BIKEBOARD_SETBIKEINFO_1).ToLocalized());
			}
		} else {
			this.BtnChoose.gameObject.SetActive(false);
			this.BuyBtn.SetActive(true);
			this.BuyCost.text = this.CurrentShowBike.Info.Data.CostAmount.ToString();
			this.BuyCostIcon.sprite = this.CurrentShowBike.Info.Data.Cost.Icon.Sprite;
		}
		this.Name.text = this.CurrentShowBike.Info.Data.Name;
		foreach (var i in this.BikeUpgradeItems) {
			i.Refresh();
		}
		UGUIGrey.SetGreyMaterail(AbilityBg, !CurrentShowBike.Info.IsAllUpgradeMax);
		Max.SetActive(CurrentShowBike.Info.IsAllUpgradeMax);
	}

	public void UpdateBikeInfo() {
		this.CurrentShowBike.Info = Client.Bike.GetBikeInfo(this.CurrentShowBike.Info.Data.ID);
		for (int i = 0; i < this._data.Count; i++) {
			this._data[i].Selected = (this.CurrentSelectDataIndex == i);
			if (this._data[i].Info.Data.ID == this.CurrentShowBike.Info.Data.ID) {
				this._data[i].Info = this.CurrentShowBike.Info;
				this._data[i].isUnLock = this.CurrentShowBike.Info.isUnLock;
			}
		}
	}

	public void OnBtnBuyClick() {

		Client.Spree.OnArriveShowPoint(ShowPoint.BuyBike, (state) => {
			if (state == SpreeShowState.BuyRMB) {
				UpdateBikeInfo();
				SetBikeInfo();
			} else if (state == SpreeShowState.RefuseRMB || state == SpreeShowState.NoRMB) {
				Buy();
			}
		});

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
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
			return;
		}
		Client.User.UserInfo.ChoosedBikeID = this.CurrentShowBike.Info.Data.ID;
		this.BtnChoose.SetEnable(false, (LString.BIKEBOARD_SETBIKEINFO).ToLocalized());
	}

	public void OnBtnNextClick() {
		if (!this.canSelect) {
			return;
		}
		if (this.CurrentSelectDataIndex < this._data.Count - 1) {
			this.CurrentSelectDataIndex += 1;
		}
		ShowBike();
	}

	public void OnBtnPreClick() {
		if (!this.canSelect) {
			return;
		}
		if (this.CurrentSelectDataIndex > 0) {
			this.CurrentSelectDataIndex -= 1;
		}
		ShowBike();
	}
	public void Reload() {
		this._data = new SmallList<BikeListItemData>();

		foreach (var bike in Client.Bike.GetSortedDatas()) {
			BikeInfo info = Client.Bike.GetBikeInfo(bike.ID);
			if (info == null) {
				info = new BikeInfo(Client.Bike[bike.ID]);
			}
			this._data.Add(new BikeListItemData(info));
		}
		this.Scroller.ReloadData();
	}

	private void CellViewSelected(EnhancedScrollerCellView cellView) {
		if (!canSelect) {
			return;
		}
		this.CurrentSelectDataIndex = (cellView as BikeListItem).DataIndex;
		ShowBike();
	}

	#region EnhancedScroller Handlers
	public int GetNumberOfCells(EnhancedScroller scroller) {
		return this._data.Count;
	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
		return this.CellWidth;
	}

	public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
		BikeListItem cellView = scroller.GetCellView(this.CellViewPrefab) as BikeListItem;
		cellView.selected = CellViewSelected;
		cellView.gameObject.name = "bikeItem" + dataIndex;
		cellView.SetData(dataIndex, this._data[dataIndex]);
		return cellView;
	}
	#endregion
}
