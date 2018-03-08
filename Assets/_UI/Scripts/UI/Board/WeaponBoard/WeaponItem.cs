
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick.Other;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI {
	public class WeaponItem : EnhancedScrollerCellView, IPointerClickHandler ,ISelectCell{

		public Image Icon;
        public Image BG, BG2;
        //public RedPoint RedPoint;
        public Text Num;
        public Image Redpoint;
		public WeaponItemInfo Info;
		public int DataIndex { get; private set; }
		public SelectedDelegate selected;
        public TweenCanvasGroupAlpha tweenalpha;
        private int number;
		public void SetData(int index, WeaponItemInfo info) {
            tweenalpha.ResetToBeginning();
            tweenalpha.PlayForward();
            if (this.Info != null) {
				this.Info.selectedChanged -= SetSelected;
				this.Info.amountChanged -= SetAmount;
			}
			this.Info = info;
			DataIndex = index;
            number = info.WeaponInfo.Amount;
            Icon.sprite = info.WeaponInfo.Data.Icon.Sprite;
            Num.text = info.WeaponInfo.Amount.ToString();
            Redpoint.gameObject.SetActive(false);
            if(info.WeaponInfo.Data.ID == Client.User.UserInfo.ChoosedWeaponID)
            {
                //Redpoint.gameObject.SetActive(true);
            }
            else
            {
                //Redpoint.gameObject.SetActive(false);
            }
			//RedPoint.SetState(info.WeaponInfo.RedPointState == RedPointState.ShouldShow);
			this.Info.selectedChanged -= SetSelected;
			this.Info.amountChanged -= SetAmount;
			this.Info.selectedChanged += SetSelected;
			this.Info.amountChanged += SetAmount;

			SetSelected(info.Selected);
        }

		public void OnPointerClick(PointerEventData eventData) {
			if (Info.WeaponInfo.RedPointState == RedPointState.ShouldShow)
			{
				Client.Weapon.SetRedPointShowed(Info.WeaponInfo.Data.ID);
				//RedPoint.SetState(false);
			}

			SetSelected(true);
			if (selected != null) selected(this);
		}

		public void SetSelected(bool isSelected) {
			Selected = isSelected;
			if (isSelected) {
                BG2.gameObject.SetActive(true);
                this.transform.localScale = new UnityEngine.Vector3(1.0f, 1.0f, 1.0f);
            } else {
                BG2.gameObject.SetActive(false);
                this.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            }
		}

		public void SetAmount(int amount) {
//			Count.text = amount.ToString();
		}

		public bool Selected { get; set; }
	}


}
