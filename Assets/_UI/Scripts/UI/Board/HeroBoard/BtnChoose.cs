using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI
{
	public class BtnChoose : MonoBehaviour,IPointerClickHandler
	{
		public Image BG;
		public Text Lable;
		public Button Btn;

		public void SetEnable(bool enable)
		{
			Btn.enabled = enable;
			if (enable)
			{
				BG.sprite = UIDataDef.Sprite_Button_Green;
				Lable.color = UIDataDef.Green;
				this.Lable.text = (LString.BIKEBOARD_SETBIKEINFO_1).ToLocalized();
			} else
			{
				BG.sprite = UIDataDef.Sprite_Button_Grey;
				Lable.color = Color.gray;
				this.Lable.text = (LString.BIKEBOARD_SETBIKEINFO).ToLocalized();

			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (HeroBoard.Ins != null && HeroBoard.Ins.gameObject.activeSelf)
			{
				HeroBoard.Ins.OnBtnChooseClick();
			} else if(GarageBoard.Ins != null &&GarageBoard.Ins.gameObject.activeSelf)
			{
				GarageBoard.Ins.OnBtnChooseClick();
			}
		}
	}


}
