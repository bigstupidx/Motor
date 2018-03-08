
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class LevelPage : EnhancedScrollerCellView {
		public RectTransform Panel;
		public LevelItem ItemPrefab;
		[System.NonSerialized]
		public List<LevelItem> Items = new List<LevelItem>();
		public RawImage Bg;
		public ChapterInfo Chapter;
		private List<MatchInfo> matchList;
		public int DataIndex { get; private set; }
		public Text StarState;
		public GameObject Lock;
		public GameObject BtnUnLock;
		public GameObject BtnUnlockBeforeTip;
		public Text UnlockStarCount;
		public Image CurrencyIcon;
		public Text CurrencyAmount;

		private int countBefore;//此页面之前的关卡数量

		public void SetData(int index, ChapterInfo info) {
			Chapter = info;
			matchList = Chapter.GetSortedMatches();
			Bg.texture = Chapter.Data.LevelBg.Texture;
			DataIndex = index;
			int own = Client.Match.GetChapterOwnedStar(Chapter.Data.ID);
			int total = Client.Match.GetChapterTotalStar(Chapter.Data.ID);
			this.StarState.text = own + "/" + total;
			if (index > Client.User.UserInfo.Chapter) {
				if (Client.Match.GetTotalOwnedStar() >= Chapter.Data.UnlockStarCount) {
					Client.User.UserInfo.Chapter = index;
					SetUnlock(Chapter.Data);
				} else {
					SetLock(Chapter.Data);
				}

			} else if (index == Client.User.UserInfo.Chapter) {
				SetCurrent(Chapter.Data);
			} else {
				SetUnlock(Chapter.Data);
			}

			for (int i = 0; i < Items.Count; i++) {

				Items[i].gameObject.SetActive(false);
			}

			var totalCount = Chapter.Data.Matches.Count;
			if (Items.Count < totalCount) {
				int count = totalCount - Items.Count;
				for (int i = 0; i < count; i++) {
					LevelItem item = Instantiate(ItemPrefab);
					item.transform.SetParent(Panel);
					item.gameObject.SetActive(false);
					item.gameObject.name = "levelItem" + i;
					Items.Add(item);
				}
			}

			for (int i = 0; i < totalCount; i++) {
				if (i > 5) {
					return;
				}
				Items[i].gameObject.SetActive(true);
				Items[i].transform.localScale = Vector3.one;
				Items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Chapter.Data.UILine.PointList[i].PosX, Chapter.Data.UILine.PointList[i].PosY);
				Items[i].SetData(i, matchList[i]);
			}
		}

		void OnDisable() {
			Bg.texture = null;
			for (int i = 0; i < Items.Count; i++) {
				Items[i].gameObject.SetActive(false);
			}
		}

		void Update() {
			if (DataIndex > Client.User.UserInfo.Chapter) {
				if (Client.Match.GetTotalOwnedStar() >= Chapter.Data.UnlockStarCount) {
					Client.User.UserInfo.Chapter = DataIndex;
					SetUnlock(Chapter.Data);
				} else {
					SetLock(Chapter.Data);
				}

			} else if (DataIndex == Client.User.UserInfo.Chapter) {
				SetCurrent(Chapter.Data);
			} else {
				SetUnlock(Chapter.Data);
			}
		}

		private void SetCurrent(ChapterData data) {
			this.Lock.SetActive(false);
			this.BtnUnLock.SetActive(false);
			this.BtnUnlockBeforeTip.SetActive(false);
		}

		private void SetLock(ChapterData data) {
			this.Lock.SetActive(true);
			UnlockStarCount.text = data.UnlockStarCount.ToString();
			if (DataIndex > Client.User.UserInfo.Chapter + 1) {//需要先解锁前一关卡
				this.BtnUnLock.SetActive(false);
				this.BtnUnlockBeforeTip.SetActive(true);
			} else {
				this.BtnUnLock.SetActive(true);
				this.BtnUnlockBeforeTip.SetActive(false);
				//设置解锁价格
				this.CurrencyIcon.sprite = data.Currency.Icon.Sprite;
				this.CurrencyAmount.text = data.CurrencyAmount.ToString();
			}
		}

		private void SetUnlock(ChapterData data) {
			this.Lock.SetActive(false);
			this.BtnUnLock.SetActive(false);
			this.BtnUnlockBeforeTip.SetActive(false);
		}

		public void onClick() {
			var data = Chapter.Data;
			if (data.Index > Client.User.UserInfo.Chapter) {
				if (data.Index == Client.User.UserInfo.Chapter + 1) {//可以使用金蛋解锁
					CommonDialog.Show((LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED).ToLocalized(), (LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_1).ToLocalized() + data.CurrencyAmount + "" + data.Currency.Name + (LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED).ToLocalized(), (LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_2).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_3).ToLocalized(), () => {
						if (!Client.Item.GetItem(data.Currency.ID).ChangeAmount(-data.CurrencyAmount)) {
							CommonTip.Show(data.Currency.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
						} else {
							if (data.Currency.Type == ItemType.Diamond) {
								AnalyticsMgrBase.Ins.Purchase(data.Name, 1, data.CurrencyAmount);
							}
							Client.User.UserInfo.Chapter++;
							SetCurrent(data);
							CommonTip.Show((LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_3).ToLocalized());
							//TODO 解锁动画等
						}
					}, null);
				} else {//需要解锁上一章
					CommonTip.Show((LString.GAMEUI_CHAPTERCHOOSEBOARD_CELLVIEWSELECTED_4).ToLocalized());
				}
			}
		}
	}

}

