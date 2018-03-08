
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using XUI;

namespace GameUI {
	public class ShopBoard : SingleUIStackBehaviour<ShopBoard>, IEnhancedScrollerDelegate {

		public const string UIPrefabPath = "UI/Board/ShopBoard/ShopBoard";

		private static bool _CurrentShow;

		public static void Show(IAPType type, bool destroyBefore = false) {
			if (!Client.Config.OpenStore) {
				CommonDialog.Show("", LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK_1.ToLocalized(), LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), null);
				return;
			}

			string[] UINames ={
				UICommonItem.MENU_BACKGROUND,
				UIPrefabPath,
				UICommonItem.TOP_BOARD

			};


			CurrentShowType = type;
			//如果当前处在shop界面则，不叠加界面
			if (_CurrentShow) {
				ShopBoard.Ins.Init();
			} else {
				ModMenu.Ins.Cover(UINames, "", destroyBefore);
			}
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			_CurrentShow = true;
			Init();
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "ShopBoard");
		}

		void OnUIDespawn() {
			base.OnUIDespawn();
			_CurrentShow = false;
		}

		void OnUIDeOverlay() {
			base.OnUIDeOverlay();
			Init();
		}

		public ShopTag[] BtnTags;

		private SmallList<IAPData> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		private SmallList<SpreeData> _spreeData;
		public EnhancedScroller SpreeScroller;
		public EnhancedScrollerCellView SpreeCellViewPrefab;
		public float SpreeCellSize;

		public static IAPType CurrentShowType;


		public void Init() {
			Scroller.Delegate = this;
			SpreeScroller.Delegate = this;
			//SpreeScroller.ScrollbarVisibility = sc
			OnBtnTagClick(CurrentShowType);
		}

		public void OnBtnTagClick(IAPType type) {
			CurrentShowType = type;
			for (int i = 0; i < BtnTags.Length; i++) {
				if (BtnTags[i].Type == type) {
					BtnTags[i].SetSelectState(true);

				} else {
					BtnTags[i].SetSelectState(false);
				}
			}

			if (type == IAPType.Spree) {
				Scroller.gameObject.SetActive(false);
				if (!SpreeScroller.gameObject.activeSelf) {
					SpreeScroller.gameObject.SetActive(true);
				}

				SpreeReload();
			} else {
				SpreeScroller.gameObject.SetActive(false);
				if (!Scroller.gameObject.activeSelf) {
					Scroller.gameObject.SetActive(true);
				}

				Reload(type);
			}
		}

		public void Reload(IAPType type) {
			_data = new SmallList<IAPData>();
			switch (type) {
				case IAPType.Coin:
					for (int i = 0; i < Client.IAP.CoinList.Count; i++) {
						_data.Add(Client.IAP.CoinList[i]);
					}
					break;
				case IAPType.Stamina:
					for (int i = 0; i < Client.IAP.StaminaList.Count; i++) {
						_data.Add(Client.IAP.StaminaList[i]);
					}
					break;
				case IAPType.Diamond:
					for (int i = 0; i < Client.IAP.DiamondList.Count; i++) {
						_data.Add(Client.IAP.DiamondList[i]);
					}
					break;
			}
			Scroller.ReloadData(Scroller.ScrollRect.horizontalNormalizedPosition);
		}

		public void SpreeReload() {
			_spreeData = new SmallList<SpreeData>();
			var list = Client.Spree.GetSpreeDatasForShop();
			for (int i = 0; i < list.Count; i++) {
				_spreeData.Add(list[i]);
			}
			SpreeScroller.ReloadData(SpreeScroller.ScrollRect.horizontalNormalizedPosition);
		}

		#region EnhancedScroller Handlers
		public int GetNumberOfCells(EnhancedScroller scroller) {
			if (CurrentShowType == IAPType.Spree) {
				return _spreeData.Count;
			}
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			if (CurrentShowType == IAPType.Spree) {
				return SpreeCellSize;
			}
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			if (CurrentShowType == IAPType.Spree) {
				SpreeListItem item = scroller.GetCellView(SpreeCellViewPrefab) as SpreeListItem;
				item.SetData(dataIndex, _spreeData[dataIndex]);
				return item;
			}

			ShopListItem cellView = scroller.GetCellView(CellViewPrefab) as ShopListItem;
			cellView.SetData(dataIndex, _data[dataIndex]);
			return cellView;
		}
		#endregion
	}

}
