using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using Game;
using GameClient;
using UnityEngine.UI;

namespace GameUI {
	public class PropBoard : MonoBehaviour, IEnhancedScrollerDelegate {

		#region base

		public const string UIPrefabPath = "UI/Board/PropBoard/PropBoard";
		public static string[] UINames =
		{
			UICommonItem.MENU_BACKGROUND,
			UIPrefabPath,
			UICommonItem.TOP_BOARD
		};

		public static void Show(bool destroyBefore = false) {
			ModMenu.Ins.Cover(UINames, "PropBoard", destroyBefore);
		}

		void OnUISpawned() {
			Init();
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "OnPropBoardInited");
		}

		void OnUIDespawn() {
			//ItemList.Clear();
		}

		void OnUILeaveStack() {
			if (GameModeBase.Ins != null)
			{
				GameModeBase.Ins.Resume();
			}
		}

		void OnUIDeOverlay() {
			Refresh();
		}

		void OnUIDeCover() {
			Refresh();
		}

		#endregion

		private List<PropItemInfo> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		public PropItemInfo CurrentSelectedProp;
		public Text Name;
		public Text Description;
		public Image SelectedProp;
		public Text SelectCount;

		public Image CostIcon;//货币图标
		public Text BuyCost;//售价

		public GameObject BtnEquip;
		public Text BtnEquipTxt;
        public Image BtnBg;
        private bool isEquip;
		private bool canSelect = true;
		private int currentSelectIndex;

		public void Init()
		{
			canSelect = true;
			Scroller.Delegate = this;
			Reload();
			if(Client.User.UserInfo.EquipedPropList.Count > 0)
			{
				for (int i = 0; i < _data.Count; i++)
				{
					if (_data[i].PropInfo.Data.ID == Client.User.UserInfo.EquipedPropList[0])
					{//初始选择当前装备的道具
						this.CurrentSelectedProp = _data[i];
						this.currentSelectIndex = i;
						_data[i].Selected = true;
					} else
					{
						_data[i].Selected = false;
					}
				}
			}
			else
			{
				CurrentSelectedProp = _data[0];
				currentSelectIndex = 0;
			}
			CurrentSelectedProp.Selected = true;

            //SetEquipInfo();
            SetBtnEquipTxt();
            SetDetailInfo();
		}

		public void Refresh()
		{
			Reload();
			UpdatePropInfo();
		}

		public void UpdatePropInfo()
		{
			CurrentSelectedProp.PropInfo = Client.Prop.GetPropInfo(CurrentSelectedProp.PropInfo.Data.ID);
			UpdateInfo();
		}

		public void SetDetailInfo() {
			if (CurrentSelectedProp != null) {
				Name.text = CurrentSelectedProp.PropInfo.Data.Name;
				Description.text = CurrentSelectedProp.PropInfo.Data.Description;
				SelectedProp.sprite = CurrentSelectedProp.PropInfo.Data.Icon.Sprite;
                SelectCount.text = (LString.GAMEUI_PROPBOARD_SETDETAILINFO).ToLocalized() + CurrentSelectedProp.PropInfo.Amount.ToString() + (LString.GAMEUI_PROPBOARD_SETDETAILINFO_1).ToLocalized();
				CostIcon.sprite = CurrentSelectedProp.PropInfo.PropData.Currency.Icon.Sprite;
				BuyCost.text = CurrentSelectedProp.PropInfo.PropData.CurrencyAmount.ToString();
				JumpTo(currentSelectIndex);
			}

		}

		public void JumpTo(int index) {
			canSelect = false;
			int i = index;
			float sOffset = 0.5f;
			float cOffset = 0.5f;
			if (currentSelectIndex < 2)
			{
				i = 0;
				sOffset = 0;
				cOffset = 0;
			} else if (currentSelectIndex > _data.Count - 3)
			{
				i = _data.Count - 1;
				sOffset = 1;
				cOffset = 1;
			}

			Scroller.JumpToDataIndex(i, sOffset, cOffset, true, EnhancedScroller.TweenType.easeInOutSine, 0.2f, () => {
				canSelect = true;
			});

		}

		/*public void SetEquipInfo() {
			if (Client.User.UserInfo.EquipedPropList.Count > 0) {
				CurrentEquipPropCount.text = Client.Prop.GetPropInfo(Client.User.UserInfo.EquipedPropList[0]).Amount.ToString();
				CurrentEquipProp.sprite = Client.Prop[Client.User.UserInfo.EquipedPropList[0]].Icon.Sprite;
				CurrentEquipProp.gameObject.SetActive(true);
			} else {
				CurrentEquipPropCount.text = "0";
				CurrentEquipProp.gameObject.SetActive(false);
			}
			SetBtnEquipTxt();
			
		}*/

		public void SetBtnEquipTxt() {
			if (Client.User.UserInfo.EquipedPropList.Count > 0 && CurrentSelectedProp.PropInfo.Data.ID == Client.User.UserInfo.EquipedPropList[0]) {
				BtnEquipTxt.text = (LString.GAMEUI_PROPBOARD_SETBTNEQUIPTXT).ToLocalized();
                BtnBg.SetGreyMaterail(true);
                isEquip = true;
			} else {
				BtnEquipTxt.text = (LString.GAMEUI_PROPBOARD_SETBTNEQUIPTXT_1).ToLocalized();
                BtnBg.SetGreyMaterail(false);
                isEquip = false;
			}
		}

		public void OnBtnEquipClick() {
			if (isEquip) {
				Client.Prop.RemoveProp();
			} else {
				if (CurrentSelectedProp != null) {
					Client.Prop.EquipProp(CurrentSelectedProp.PropInfo.Data.ID);
				}
			}
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Equip);
            SetBtnEquipTxt();
        }

		public void OnBtnBuyClick() {
            Buy();
            /*if (CurrentSelectedProp != null) {
				Client.Spree.OnArriveShowPoint(ShowPoint.BuyProp, (s) =>
				{
					if (s == SpreeShowState.BuyRMB)
					{
						UpdateInfo();
					}else if (s == SpreeShowState.NoRMB || s == SpreeShowState.RefuseRMB||s==SpreeShowState.NoReady)
					{
						
					}
				});
			}*/
		}

		private void Buy()
		{
			if (Client.Prop.BuyProp(this.CurrentSelectedProp.PropInfo))
			{
                SfxManager.Ins.PlayOneShot(SfxType.SFX_Buy);
                CommonTip.Show((LString.BIKEBOARD_BUY).ToLocalized());
				UpdateInfo();
				if (this.CurrentSelectedProp.PropInfo.RedPointState == RedPointState.ShouldShow)
				{
					Client.Prop.SetRedPointShowed(this.CurrentSelectedProp.PropInfo.Data.ID);
				}
			} else
			{
                SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
                CommonTip.Show(this.CurrentSelectedProp.PropInfo.PropData.Currency.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
				Client.Spree.OnArriveShowPoint(ShowPoint.BuyPropFail, (state) =>
				{
					if (state == SpreeShowState.NoRMB)
					{
						Client.IAP.ShowShopBoardForSupply(this.CurrentSelectedProp.PropInfo.PropData.Currency.ID);
					}
				});
			}
		}

		private void UpdateInfo()
		{
		    SelectCount.text = (LString.GAMEUI_PROPBOARD_SETDETAILINFO).ToLocalized() + this.CurrentSelectedProp.PropInfo.Amount.ToString() + (LString.GAMEUI_PROPBOARD_SETDETAILINFO_1).ToLocalized();
			if (Client.User.UserInfo.EquipedPropList.Count > 0)
			{
				if (Client.User.UserInfo.EquipedPropList[0] == CurrentSelectedProp.PropInfo.PropData.ID)
				{
					//CurrentEquipPropCount.text = this.CurrentSelectedProp.PropInfo.Amount.ToString();
				}
			}
		}
        public void OnBtnNextClick()
        {
            if (this.currentSelectIndex < _data.Count - 1)
            {
                this.currentSelectIndex += 1;
            }
            for (var i = 0; i < this._data.Count; i++)
            {
                this._data[i].Selected = false;
            }
            this.CurrentSelectedProp = this._data[currentSelectIndex];
            this.CurrentSelectedProp.Selected = true;
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
            this.CurrentSelectedProp = this._data[currentSelectIndex];
            this.CurrentSelectedProp.Selected = true;
            JumpTo(currentSelectIndex);
            SetDetailInfo();
            SetBtnEquipTxt();
        }
        #region Scroll
        private void Reload() {
			_data = new List<PropItemInfo>();

			foreach (var prop in Client.Prop.GetSortedInfos()) {
				_data.Add(new PropItemInfo(prop));
			}
			Scroller.ReloadData();
		}

		private void CellViewSelected(EnhancedScrollerCellView cellView) {
			if (!canSelect)
			{
				return;
			}
			var selectedDataIndex = (cellView as PropItem).DataIndex;

			for (var i = 0; i < _data.Count; i++) {
				_data[i].Selected = (selectedDataIndex == i);
			}
			CurrentSelectedProp = _data[cellView.dataIndex];
			currentSelectIndex = selectedDataIndex;
			SetDetailInfo();
			SetBtnEquipTxt();
		}

		public int GetNumberOfCells(EnhancedScroller scroller) {
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			PropItem cellView = scroller.GetCellView(CellViewPrefab) as PropItem;
			cellView.selected = CellViewSelected;
			cellView.gameObject.name = "propItem" + dataIndex;
			cellView.SetData(dataIndex, _data[dataIndex]);
			return cellView;
		}
		#endregion
	}

}

