
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class ChapterItem : EnhancedScrollerCellView {

		public Text Name, Name2;
		public Text Desc;
		public RawImage Icon;
		public Text StarCount;
		public Image Bg;
		public Image BgTop;
		public Text UnlockStarCount;

		public GameObject Lock;
		public GameObject StarCountTip;

		public Button Btn;


		public int DataIndex { get; private set; }
		public SelectedDelegate selected;

		public Transform Renderer;
		private EnhancedScroller scroller;

		private ChapterChooseBoard _chapterChooseBoard;
		public TweenCanvasGroupAlpha tweenalpha;

		public void SetData(ChapterChooseBoard chapterChooseBoard, int index, ChapterData data) {
			tweenalpha.ResetToBeginning();
			tweenalpha.PlayForward();
			this._chapterChooseBoard = chapterChooseBoard;
			this.DataIndex = index;
			this.Name.text = data.Name;
			this.Name2.text = data.Name;
			this.Desc.text = data.Description;
			this.Icon.texture = UIDataDef.GetModeLinTexture(data.Icon);
			if (index > Client.User.UserInfo.Chapter) {
				if (Client.Match.GetTotalOwnedStar() >= data.UnlockStarCount) {
					Client.User.UserInfo.Chapter = index;
					SetUnlock(data);
				} else {
					SetLock(data);
				}
			} else if (index == Client.User.UserInfo.Chapter) {
				SetCurrent(data);
			} else {
				SetUnlock(data);
			}
		}

		private void SetCurrent(ChapterData data) {
			Bg.gameObject.SetActive(true);
			BgTop.gameObject.SetActive(true);
			this.Lock.SetActive(false);
			Name2.gameObject.SetActive(false);
			StarCountTip.SetActive(true);
			int totalStar = Client.Match.GetChapterTotalStar(data.ID);
			int ownedStar = Client.Match.GetChapterOwnedStar(data.ID);
			this.StarCount.text = ownedStar + "/" + totalStar;

		}

		private void SetLock(ChapterData data) {
			Bg.gameObject.SetActive(false);
			BgTop.gameObject.SetActive(false);
			this.Lock.SetActive(true);
			Name2.gameObject.SetActive(true);
			UnlockStarCount.text = data.UnlockStarCount.ToString();
			StarCountTip.SetActive(false);
		}

		private void SetUnlock(ChapterData data) {
			Bg.gameObject.SetActive(true);
			BgTop.gameObject.SetActive(true);
			this.Lock.SetActive(false);
			Name2.gameObject.SetActive(false);
			StarCountTip.SetActive(true);
			int totalStar = Client.Match.GetChapterTotalStar(data.ID);
			int ownedStar = Client.Match.GetChapterOwnedStar(data.ID);
			this.StarCount.text = ownedStar + "/" + totalStar;
		}

		public void OnClick() {
			if (this.selected != null) {
				this.selected(this);
			}
		}
	}

}

