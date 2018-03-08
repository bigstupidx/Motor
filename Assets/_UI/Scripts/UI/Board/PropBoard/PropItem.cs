
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick;
using Joystick.Other;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI {
	public class PropItem : EnhancedScrollerCellView, IPointerClickHandler,ISelectCell {

		public Image Icon;
		public Image Highlight;
		public Text Count;
		//public RedPoint RedPoint;

		public PropItemInfo Info;
		public int DataIndex { get; private set; }
		public SelectedDelegate selected;
		public TweenCanvasGroupAlpha tweenalpha;
		public void SetData(int index, PropItemInfo info) {
			tweenalpha.ResetToBeginning();
			tweenalpha.PlayForward();
			if (this.Info != null) {
				this.Info.selectedChanged -= SetSelected;
				this.Info.amountChanged -= SetAmount;
			}
			this.Info = info;
			DataIndex = index;
			Count.text = info.PropInfo.Amount.ToString();
			Icon.sprite = info.PropInfo.Data.Icon.Sprite;
			//RedPoint.SetState(info.PropInfo.RedPointState == RedPointState.ShouldShow);

			this.Info.selectedChanged -= SetSelected;
			this.Info.amountChanged -= SetAmount;
			this.Info.selectedChanged += SetSelected;
			this.Info.amountChanged += SetAmount;

			SetSelected(info.Selected);
		}

		public void OnPointerClick(PointerEventData eventData) {
			if (Info.PropInfo.RedPointState == RedPointState.ShouldShow) {
				Client.Prop.SetRedPointShowed(Info.PropInfo.Data.ID);
				//RedPoint.SetState(false);
			}
			SetSelected(true);
			if (selected != null) {
				selected(this);
			}
		}

		public void SetSelected(bool isSelected) {
			Selected = isSelected;
			this.Highlight.gameObject.SetActive(isSelected);
		}

		public void SetAmount(int amount) {
			//			Count.text = amount.ToString();
		}

		public bool Selected { get; set; }
	}


}
