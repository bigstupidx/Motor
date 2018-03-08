using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick.Other;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI {
	public class HeroListItem : EnhancedScrollerCellView, IPointerClickHandler, ISelectCell {
		public Image BG, BG2;
		public Image NameBG;
		public Image Icon;
		public Text Name;
		public GameObject Lock;
		public RedPoint RedPoint;

		public HeroListItemData Data;

		public int DataIndex { get; private set; }
		public SelectedDelegate selected;
		public TweenCanvasGroupAlpha tweenalpha;
		public void SetData(int index, HeroListItemData data) {
			if (Data != null) {
				Data.selectedChanged -= ChooseToShow;
				Data.LockStateChanged -= ChangeLockState;
			}
			tweenalpha.ResetToBeginning();
			tweenalpha.PlayForward();
			Data = data;
			DataIndex = index;
			//set info
			Icon.sprite = Data.Info.Data.Icon.Sprite;
			//Name.text = Data.Info.Data.Name;
			RedPoint.SetState(data.Info.RedPointState == RedPointState.ShouldShow);

			Data.selectedChanged -= ChooseToShow;
			Data.LockStateChanged -= ChangeLockState;
			Data.selectedChanged += ChooseToShow;
			Data.LockStateChanged += ChangeLockState;

			ChooseToShow(data.Selected);
			data.IsUnlock = data.Info.isUnLock;
			ChangeLockState(data.Info.isUnLock);
		}

		public void ChooseToShow(bool isShow) {
			Selected = isShow;
			if (isShow) {
				BG2.gameObject.SetActive(true);
				this.transform.localScale = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);
			} else {
				BG2.gameObject.SetActive(false);
				this.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
			}
		}

		public void ChangeLockState(bool isUnlock) {
			Lock.SetActive(!isUnlock);
		}

		public void OnPointerClick(PointerEventData eventData) {

			if (Data.Info.RedPointState == RedPointState.ShouldShow) {
				Client.Hero.SetRedPointShowed(Data.Info.Data.ID);
				RedPoint.SetState(false);
			}

			if (selected != null) {
				selected(this);
			}
		}

		public bool Selected { get; set; }
	}

}
