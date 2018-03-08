using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class HeroBoard : SingleUIStackBehaviour<HeroBoard>, IEnhancedScrollerDelegate {

		#region base

		public const string UIPrefabPath = "UI/Board/HeroBoard/HeroBoard";

		private static bool _CurrentShow;

		public static void Show(int heroId) {
			if (_CurrentShow) {
				HeroBoard.Ins.Init(HeroBoard.Ins.CurrentShowHero.Info.Data.ID);
			} else {
				string[] UIPrefabNames ={
					UIPrefabPath,
					UICommonItem.TOP_BOARD
				};
				HeroBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "HeroBoard")[0].Instance.GetComponent<HeroBoard>();
				ins.Init(heroId);
				Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "OnHeroBoardInited");
			}
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			_CurrentShow = true;
			foreach (var t in this.Ts) {
				t.ResetToBeginning();
				t.PlayForward();
			}
			ModelShow.Ins.HideBike();
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.HeroBoard);
			if (this.CurrentShowHero != null) {
				ModelShow.Ins.ShowHero(this.CurrentShowHero.Info.Data.Prefab);
			}
		}

		public override void OnUIDespawn() {
			base.OnUIDespawn();
			_CurrentShow = false;
			ModelShow.Ins.HideHero();
		}

		public override void OnUIDeCover() {
			Refresh();
		}

		public override void OnUIDeOverlay() {
			Refresh();
		}


		#endregion

		public UITweener[] Ts;

		public Text Title;
		public Text LvLabel;
		public HeroUpgradeItem[] UpgradeItems;
		public Text AbilityDesc;
		public Text BuyCost;
		public Image BuyCostIcon;
		public Text UpgradeCost;
		public Image UpgradeCostIcon;
		public Button Buy;
		public GameObject BtnChoose;
		public Text choooseText;
		public Image btnBG;
		public Button BtnUpgrade;
		public GameObject BtnLvMax;
		public Text heroDesc;

		public GameObject BtnSpree;

		public SmallList<HeroListItemData> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;
		public float ScrollTime;

		public HeroListItemData CurrentShowHero;
		private int CurrentSelectDataIndex;

		private Coroutine HeroChangeShow;
		private bool canSelect = true;

		private DriverShow _driverShow;

		public void Init(int heroId) {
			Client.Hero.SetRedPointShowed(heroId);
			this.Scroller.Delegate = this;
			Reload();
			this.canSelect = true;
			for (int i = 0; i < this._data.Count; i++) {
				if (this._data[i].Info.Data.ID == heroId) {
					this.CurrentSelectDataIndex = i;
					this._data[i].Selected = true;
					CurrentShowHero = _data[i];
				} else {
					this._data[i].Selected = false;
				}
			}
			ShowHero();
			UpdateBtnSpree();
		}

		public void Refresh() {
			Reload();
			UpdateHeroInfo();
			UpdateBtnSpree();
		}

		private void UpdateBtnSpree() {
			var data = Client.Spree.GetSpreeDataByShowType(ShowType.HeroBoard);
			//BtnSpree.SetActive(Client.Spree.ShouldShow(data));
		}

		public void SetHeroInfo() {
			var heroInfo = this.CurrentShowHero.Info;

			if (this.CurrentShowHero.Info.isUnLock) {//已解锁
				this.Title.text = this.CurrentShowHero.Info.Data.Name;
				this.LvLabel.text = "Lv." + (this.CurrentShowHero.Info.Level + 1);
				this.LvLabel.gameObject.SetActive(true);
				this.Buy.gameObject.SetActive(false);
				this.BtnUpgrade.gameObject.SetActive(true);
				this.UpgradeCost.text = heroInfo.Data.LvUpCost.GetValue(heroInfo.Level).ToString();
				this.UpgradeCostIcon.sprite = heroInfo.Data.heroItem.Icon.Sprite;
				this.BtnChoose.SetActive(true);
				if (this.CurrentShowHero.Info.Data.ID == Client.User.UserInfo.ChoosedHeroID) {
					this.choooseText.text = (LString.GAMEUI_GARAGEBOARD_SHOWBIKE).ToLocalized();
					btnBG.SetGreyMaterail(true);
				} else {
					this.choooseText.text = (LString.GAMEUI_GARAGEBOARD_SHOWBIKE_1).ToLocalized();
					btnBG.SetGreyMaterail(false);
				}
				if (heroInfo.Level >= Client.Hero.MaxLevel - 1) { //满级
					this.BtnLvMax.SetActive(true);
					this.BtnUpgrade.GetComponent<UpdateMax>().SetData(true);
				} else {
					this.BtnLvMax.SetActive(false);
					this.BtnUpgrade.GetComponent<UpdateMax>().SetData(false);
				}
			} else {//未解锁
				this.Title.text = this.CurrentShowHero.Info.Data.Name;
				this.LvLabel.gameObject.SetActive(false);
				//this.BtnChange.GetComponent<RectTransform>().anchoredPosition = BtnChangePosCenter;
				this.BtnUpgrade.gameObject.SetActive(false);
				this.BtnLvMax.SetActive(false);
				this.BtnChoose.SetActive(false);
				this.Buy.gameObject.SetActive(true);
				this.BuyCost.text = this.CurrentShowHero.Info.Data.CostAmount.ToString();
				this.BuyCostIcon.sprite = heroInfo.Data.CostItem.Icon.Sprite;
			}
			//特殊能力描述
			this.AbilityDesc.text = heroInfo.AbilityDesc;
			this.heroDesc.text = heroInfo.Data.Description;
			foreach (var item in this.UpgradeItems) {
				item.Refresh();
			}
		}

		public void ShowHero() {
			CurrentShowHero = _data[CurrentSelectDataIndex];
			for (int i = 0; i < this._data.Count; i++) {
				this._data[i].Selected = (this.CurrentSelectDataIndex == i);
			}
			UpdateHeroInfo();
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

			Scroller.JumpToDataIndex(i, sOffset, cOffset, true, EnhancedScroller.TweenType.easeInOutSine, 0.2f, () => {
				canSelect = true;
			});
		}

		public void UpdateHeroInfo() {
			this.CurrentShowHero.Info = Client.Hero.GetHeroInfo(this.CurrentShowHero.Info.Data.ID);
			for (int i = 0; i < this._data.Count; i++) {
				if (this._data[i].Info.Data.ID == this.CurrentShowHero.Info.Data.ID) {
					this._data[i].Info = this.CurrentShowHero.Info;
					this._data[i].IsUnlock = CurrentShowHero.Info.isUnLock;
					break;
				}
			}
			ModelShow.Ins.ShowHero(this.CurrentShowHero.Info.Data.Prefab);
			SetHeroInfo();
		}

		public void BtnNextHero() {
			if (!this.canSelect) {
				return;
			}
			if (this.CurrentSelectDataIndex < this._data.Count - 1) {
				this.CurrentSelectDataIndex += 1;
			}
			ShowHero();
		}

		public void BtnPreHero() {
			if (!this.canSelect) {
				return;
			}
			if (this.CurrentSelectDataIndex > 0) {
				this.CurrentSelectDataIndex -= 1;
			}
			ShowHero();
		}

		public void OnBtnChooseClick() {
			if (!this.CurrentShowHero.Info.isUnLock) {
				return;
			}
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Equip);
			Client.User.UserInfo.ChoosedHeroID = this.CurrentShowHero.Info.Data.ID;
			this.choooseText.text = (LString.GAMEUI_GARAGEBOARD_SHOWBIKE).ToLocalized();
			btnBG.SetGreyMaterail(true);
			//_driverShow.PlayChooseAudio(this.CurrentShowHero.Info.Data.ID);
		}

		public void OnBtnBuyClick() {
			Client.Spree.OnArriveShowPoint(ShowPoint.BuyHero, (state) => {
				if (state == SpreeShowState.BuyRMB) {
					UpdateHeroInfo();
				} else if (state == SpreeShowState.RefuseRMB || state == SpreeShowState.NoRMB) {
					BuyHero();
				}
			});
		}

		private void BuyHero() {
			if (Client.Hero.BuyHero(this.CurrentShowHero.Info.Data.ID)) {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Buy);
				CommonTip.Show((LString.BIKEBOARD_BUY).ToLocalized());
				UpdateHeroInfo();
				Client.Hero.SetRedPointShowed(this.CurrentShowHero.Info.Data.ID);
			} else {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				CommonTip.Show(this.CurrentShowHero.Info.Data.CostItem.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
				Client.Spree.OnArriveShowPoint(ShowPoint.BuyHeroFail, (state) => {
					if (state == SpreeShowState.NoRMB) {
						Client.IAP.ShowShopBoardForSupply(this.CurrentShowHero.Info.Data.CostItem.ID);
					}
				});
			}
		}

		public void OnBtnUpgradeClick() {
			if (this.CurrentShowHero.Info.Level < Client.Hero.MaxLevel - 1) {
				if (Client.Hero.UpgradeHero(this.CurrentShowHero.Info.Data.ID)) {
					ModelShow.Ins.ShowUpgradeEffect(true);
					SfxManager.Ins.PlayOneShot(SfxType.sfx_exp_up);
					UpdateHeroInfo();
					//_driverShow.PlayUpgradeAudio(this.CurrentShowHero.Info.Data.ID);
				} else {
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
					CommonTip.Show(this.CurrentShowHero.Info.Data.heroItem.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
					Client.IAP.ShowShopBoardForSupply(this.CurrentShowHero.Info.Data.heroItem.ID);
				}
			}
		}

		public void Reload() {
			this._data = new SmallList<HeroListItemData>();
			foreach (var hero in Client.Hero.GetSortedDatas()) {
				HeroInfo info = Client.Hero.GetHeroInfo(hero.ID);
				if (info == null) {
					info = new HeroInfo(Client.Hero[hero.ID], 0);
				}
				this._data.Add(new HeroListItemData(info));
			}
			this.Scroller.ReloadData();
		}

		private void CellViewSelected(EnhancedScrollerCellView cellView) {
			if (!canSelect) {
				return;
			}
			this.CurrentSelectDataIndex = (cellView as HeroListItem).DataIndex;
			ShowHero();
		}

		#region EnhancedScroller Handlers
		public int GetNumberOfCells(EnhancedScroller scroller) {
			return this._data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return this.CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			HeroListItem cellView = scroller.GetCellView(this.CellViewPrefab) as HeroListItem;
			cellView.selected = CellViewSelected;
			cellView.gameObject.name = "heroItem" + dataIndex;
			cellView.SetData(dataIndex, this._data[dataIndex]);
			return cellView;
		}
		#endregion

	}


}
