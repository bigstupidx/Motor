using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick;
using XUI;

namespace GameUI {
	public class ChapterChooseBoard : SingleUIStackBehaviour<ChapterChooseBoard>, IEnhancedScrollerDelegate {
		public const string UIPrefabPath = "UI/Board/ChapterChooseBoard/ChapterChooseBoard";

		public static void Show() {
			string[] UIPrefabNames ={
				UICommonItem.MENU_BACKGROUND,
				UIPrefabPath,
				UICommonItem.TOP_BOARD
			};
			var ins = ModMenu.Ins.Cover(UIPrefabNames, "ChapterChooseBoard")[1].Instance.GetComponent<ChapterChooseBoard>();
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			Scroller.Delegate = this;
			Reload();
		}

		public override void OnUILeaveStack() {
			base.OnUILeaveStack();
			this.focus = null;
		}

		private SmallList<ChapterData> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		private float currentScrollerPosition = 0f;


		public FocusItemBase focus;

		private void Reload() {
			_data = new SmallList<ChapterData>();
			foreach (var chapter in Client.Match.GetSortedChapterDatas()) {
				_data.Add(chapter);
			}
			Scroller.ReloadData(currentScrollerPosition);

			if (FocusManager.TVMode) {
				if (this.focus == null) {
					var chapters = transform.GetComponentsInChildren<FocusItemBase>();
					focus = chapters[Client.User.UserInfo.Chapter];
				}
				this.DelayInvoke(() => {
					FocusManager.Ins.Focus(this.focus);
				}, 0.05f);
			}
		}

		private void JumpTo(int index) {
			Scroller.JumpToDataIndex(index, 0.5f, 0.5f, true, EnhancedScroller.TweenType.easeInOutSine, 0.3f, null);
		}

		private void CellViewSelected(EnhancedScrollerCellView cellView) {
			var data = this._data[cellView.dataIndex];
			if (data.Index > Client.User.UserInfo.Chapter) {
				if (data.Index == Client.User.UserInfo.Chapter + 1) {//可以使用金蛋解锁
					CommonDialog.Show((LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED).ToLocalized(), (LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_1).ToLocalized() + data.CurrencyAmount + " " + data.Currency.Name + (LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED).ToLocalized(), (LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_2).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_3).ToLocalized(), () => {
						if (!Client.Item.GetItem(data.Currency.ID).ChangeAmount(-data.CurrencyAmount)) {
							CommonTip.Show(data.Currency.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
						} else {
							if (data.Currency.Type == ItemType.Diamond) {
								AnalyticsMgrBase.Ins.Purchase(data.Name, 1, data.CurrencyAmount);
							}
							Client.User.UserInfo.Chapter++;
							this.focus = cellView.GetComponentInChildren<FocusItemBase>();
							Reload();
							CommonTip.Show((LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_3).ToLocalized());
							//JumpTo(Client.User.UserInfo.Chapter);
							//TODO 解锁动画等
						}
					}, null);
				} else {//需要解锁上一章
					CommonTip.Show((LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_4).ToLocalized());
				}
			} else {
				this.focus = cellView.GetComponentInChildren<FocusItemBase>();
				LevelChooseBoard.Show(cellView.dataIndex);
			}
		}

		public int GetNumberOfCells(EnhancedScroller scroller) {
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			ChapterItem cellView = scroller.GetCellView(CellViewPrefab) as ChapterItem;
			cellView.selected = CellViewSelected;
			cellView.SetData(this, dataIndex, _data[dataIndex]);
			cellView.gameObject.name = "chapterItem" + dataIndex;
			return cellView;
		}
	}

}

