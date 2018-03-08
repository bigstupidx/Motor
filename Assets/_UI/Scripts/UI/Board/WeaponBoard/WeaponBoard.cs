using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine.UI;
using System;

namespace GameUI {
	public class WeaponBoard : MonoBehaviour, IEnhancedScrollerDelegate {

		#region base

		public const string UIPrefabPath = "UI/Board/WeaponBoard/WeaponBoard";
		public static string[] UINames =
		{
				UICommonItem.MENU_BACKGROUND,
				UIPrefabPath,
				UICommonItem.TOP_BOARD
		};

		public static void Show(bool destroyBefore = false) {		
			WeaponBoard ins = ModMenu.Ins.Cover(UINames, "WeaponBoard", destroyBefore)[1].Instance.GetComponent<WeaponBoard>();
			ins.Init();
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "OnWeaponBoardInited");
		}
		
		#endregion

		private List<WeaponItemInfo> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		public WeaponItemInfo CurrentWeapon;
		public Text Name;
		public Text Description;
		public Text SelectCount;
		public Image SelectedWeaponImg;
        //public Image CurrentEquipProp;
        //public Text CurrentEquipPropCount;
        public Text BuyCost;//售价
		public Image CostIcon;//货币图标
		public GameObject DefaulWeapon;
		public GameObject BtnEquip;
		public Text BtnEquipTxt;
        public Image BtnBg;
		public Text weaponDesc;
		public GameObject BtnBuy;
		private bool isEquip;
		private bool canSelect = true;
		private int currentSelectIndex;

		public void Init() {
			this.Scroller.Delegate = this;
			Reload();
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].WeaponInfo.Data.ID == Client.User.UserInfo.ChoosedWeaponID)
				{//初始选择当前装备的武器
					this.CurrentWeapon = _data[i];
					this.currentSelectIndex = i;
					_data[i].Selected = true;
				}
				else
				{
					_data[i].Selected = false;
				}
			}
			this.CurrentWeapon.Selected = true;
            //SetEquipInfo();
            SetDetailInfo();
			SetBtnEquipTxt();
		}

		public void SetDetailInfo() {
			this.Name.text = this.CurrentWeapon.WeaponInfo.Data.Name;
			this.Description.text = this.CurrentWeapon.WeaponInfo.AbilityDesc;
			this.SelectedWeaponImg.sprite = this.CurrentWeapon.WeaponInfo.Data.Icon.Sprite;
			this.weaponDesc.text = this.CurrentWeapon.WeaponInfo.Data.Description;

			if (!this.CurrentWeapon.WeaponInfo.WeaponData.Consum) {//无限武器不能购买
				this.BtnBuy.SetActive(false);
				this.DefaulWeapon .SetActive(true);
				this.SelectCount.text = (LString.GAMEUI_WEAPONBOARD_SETDETAILINFO).ToLocalized();


			} else {
				this.DefaulWeapon.SetActive(false);
				this.BtnBuy.SetActive(true);
				this.CostIcon.sprite = this.CurrentWeapon.WeaponInfo.WeaponData.Currency.Icon.Sprite;
				this.BuyCost.text = this.CurrentWeapon.WeaponInfo.WeaponData.CurrencyAmount.ToString();
                this.SelectCount.text = (LString.GAMEUI_PROPBOARD_SETDETAILINFO).ToLocalized() + this.CurrentWeapon.WeaponInfo.Amount + (LString.GAMEUI_PROPBOARD_SETDETAILINFO_1).ToLocalized() + (LString.GAMEUI_WEAPONBOARD_SETDETAILINFO_1).ToLocalized();

			}
			JumpTo(currentSelectIndex);
			SetBtnEquipTxt();
		}

		public void JumpTo(int index)
		{
			canSelect = false;
			int i = index;
			float sOffset = 0.5f;
			float cOffset = 0.5f;
			if (currentSelectIndex<2)
			{
				i = 0;
				sOffset = 0;
				cOffset = 0;
			}
			else if (currentSelectIndex > _data.Count - 3)
			{
				i = _data.Count - 1;
				sOffset = 1;
				cOffset = 1;
			}

			Scroller.JumpToDataIndex(i, sOffset, cOffset, true, EnhancedScroller.TweenType.easeInOutSine, 0.2f, () =>
			{
				canSelect = true;
			});
		}

		private void SetBtnEquipTxt() {
			if (this.CurrentWeapon.WeaponInfo.Data.ID == Client.User.UserInfo.ChoosedWeaponID) {
				this.BtnEquipTxt.text = (LString.GAMEUI_PROPBOARD_SETBTNEQUIPTXT).ToLocalized();
                BtnBg.SetGreyMaterail(true);
				this.isEquip = true;
			} else {
				this.BtnEquipTxt.text = (LString.GAMEUI_PROPBOARD_SETBTNEQUIPTXT_1).ToLocalized();
                BtnBg.SetGreyMaterail(false);
                this.isEquip = false;
			}
		}

        /*public void SetEquipInfo()
        {
			CurrentEquipPropCount.text = _data[currentSelectIndex].WeaponInfo.Amount.ToString();
			CurrentEquipPropCount.transform.parent.gameObject.SetActive (!this.CurrentWeapon.WeaponInfo.WeaponData.Consum);
            CurrentEquipProp.sprite = _data[currentSelectIndex].WeaponInfo.Data.Icon.Sprite;
            CurrentEquipProp.gameObject.SetActive(true);
            SetBtnEquipTxt();
        }*/


        public void OnBtnEquipClick() {
			if (this.isEquip) {
				if (Client.User.UserInfo.ChoosedWeaponID == DataDef.DefalutWeapon) {
					CommonTip.Show((LString.GAMEUI_WEAPONBOARD_ONBTNEQUIPCLICK).ToLocalized());
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				} else {

					Client.User.UserInfo.ChoosedWeaponID = DataDef.DefalutWeapon;//武器卸载后使用初始武器
                    //SetEquipInfo();
                }
			} else {
				if (this.CurrentWeapon.WeaponInfo.WeaponData.Consum && this.CurrentWeapon.WeaponInfo.Amount <= 0) {
					CommonTip.Show((LString.GAMEUI_WEAPONBOARD_ONBTNEQUIPCLICK_1).ToLocalized());
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				} else {
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Equip);
					Client.User.UserInfo.ChoosedWeaponID = this.CurrentWeapon.WeaponInfo.Data.ID;
                   // SetEquipInfo();
                }
			}
            SetBtnEquipTxt();
        }

		public void OnBtnBuyClick() {
			if (this.CurrentWeapon != null) {
				if (Client.Weapon.BuyWeapon(this.CurrentWeapon.WeaponInfo))
				{
                    SfxManager.Ins.PlayOneShot(SfxType.SFX_Buy);
                    SelectCount.text = (LString.GAMEUI_PROPBOARD_SETDETAILINFO).ToLocalized() + CurrentWeapon.WeaponInfo.Amount + (LString.GAMEUI_PROPBOARD_SETDETAILINFO_1).ToLocalized() + (LString.GAMEUI_WEAPONBOARD_SETDETAILINFO_1).ToLocalized();
                    if (this.CurrentWeapon.WeaponInfo.RedPointState == RedPointState.ShouldShow)
					{
						Client.Weapon.SetRedPointShowed(this.CurrentWeapon.WeaponInfo.Data.ID);
					}
				} else {
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
					CommonTip.Show(this.CurrentWeapon.WeaponInfo.WeaponData.Currency.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
					Client.IAP.ShowShopBoardForSupply(this.CurrentWeapon.WeaponInfo.WeaponData.Currency.ID);
				}

			}
		}

        public void OnBtnNextClick()
        {
            if (this.currentSelectIndex < _data.Count-1)
            {
                this.currentSelectIndex += 1;
            }
            for (var i = 0; i < this._data.Count; i++)
            {
                this._data[i].Selected = false;
            }
            this.CurrentWeapon = this._data[currentSelectIndex];
            this.CurrentWeapon.Selected = true;
            JumpTo(currentSelectIndex);
            SetDetailInfo();
            SetBtnEquipTxt();
        }

        public void OnBtnPreClick()
        {
            if (this.currentSelectIndex > 0)
            {
                this.currentSelectIndex -= 1;
            }
            for (var i = 0; i < this._data.Count; i++)
            {
                this._data[i].Selected = false;
            }
            this.CurrentWeapon = this._data[currentSelectIndex];
            this.CurrentWeapon.Selected = true;
            JumpTo(currentSelectIndex);
            SetDetailInfo();
            SetBtnEquipTxt();
        }


        #region Scroll
        private void Reload() {
			this._data = new List<WeaponItemInfo>();

			foreach (var weapon in Client.Weapon.GetSortedInfo()) {
				this._data.Add(new WeaponItemInfo(weapon));
			}
			this.Scroller.ReloadData();
		}

		private void CellViewSelected(EnhancedScrollerCellView cellView) {
			if (!canSelect)
			{
				return;
			}
			var selectedDataIndex = (cellView as WeaponItem).DataIndex;

			for (var i = 0; i < this._data.Count; i++) {
				this._data[i].Selected = (selectedDataIndex == i);			
			}
			this.CurrentWeapon = this._data[cellView.dataIndex];
			this.currentSelectIndex = selectedDataIndex;
			SetDetailInfo();
		}

		public int GetNumberOfCells(EnhancedScroller scroller) {
			return this._data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return this.CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			WeaponItem cellView = scroller.GetCellView(this.CellViewPrefab) as WeaponItem;
			cellView.selected = CellViewSelected;
			cellView.gameObject.name = "weaponItem" + dataIndex;
			cellView.SetData(dataIndex, this._data[dataIndex]);

			return cellView;
		}
		#endregion
	}

}

