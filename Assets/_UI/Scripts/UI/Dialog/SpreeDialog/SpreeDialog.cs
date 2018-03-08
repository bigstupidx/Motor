using System;
using UnityEngine;
using GameClient;
using UnityEngine.UI;

namespace GameUI
{
	public class SpreeDialog : MonoBehaviour {
		[NonSerialized]
		public SpreeData _data;

		public Text Title;
		public Image TitleImg;
		public Text Desc;
		public Image DescImg;
		public Text BtnName;
		public Image BtnClose;

		public SpreeRewardItem[] Items;

		public virtual void Init()
		{
			SetTitle();
			SetDesc();
			SetBtnName();
			SetRewardList();
			SetBtnCloseImg();
		}

		public virtual void SetTitle()
		{
			if (_data.PayCode == -1 || _data.Name.Equals((LString.GAMEUI_SPREEDIALOG_SETTITLE).ToLocalized()))
			{
				SetTxtTitle();
				return;
			}

			Sprite title = UIDataDef.GetSpreeTitle(_data.PayCode);
			if (title == null)
			{
				SetTxtTitle();
			}
			else
			{
				Title.gameObject.SetActive(false);
				TitleImg.gameObject.SetActive(true);
				TitleImg.sprite = title;
			}
		}

		private void SetTxtTitle()
		{
			TitleImg.gameObject.SetActive(false);
			Title.gameObject.SetActive(true);
			Title.text = _data.Name;
		}

		public virtual void SetDesc()
		{
			if (_data.PayCode == -1)
			{
				SetTxtDesc();
			}
			else
			{
				Desc.gameObject.SetActive(false);
				DescImg.gameObject.SetActive(true);
				DescImg.sprite = UIDataDef.GetSpreeDesc(_data.PayCode);
			}
		}

		private void SetTxtDesc()
		{
			string content = _data.Desc.Replace("\\n", "\n");
			content = content.Replace("&lt;", "<");
			content = content.Replace("&gt;", ">");
			Desc.gameObject.SetActive(true);
			Desc.text = content;
			DescImg.gameObject.SetActive(false);
		}

		public virtual void SetRewardList()
		{
			//奖励物品
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i].gameObject.SetActive(false);
			}
			int count = _data.AwardList.Count < 6 ? _data.AwardList.Count:5;
			
			for (int i = 0; i < count; i++)
			{
				Items[i].gameObject.SetActive(true);
				Items[i].SetData(_data.AwardList[i]);
			}
		}

		public virtual void SetBtnName()
		{
			BtnName.text = _data.BtnName;
		}

		public virtual void SetBtnCloseImg()
		{
			switch (_data.BtnCloseType)
			{
					case BtnCloseType.Normal:
					BtnClose.gameObject.SetActive(true);
					BtnClose.sprite = UIDataDef.Sprite_Btn_Close;
					break;
					case BtnCloseType.Blurry:
					BtnClose.gameObject.SetActive(true);
					BtnClose.sprite = UIDataDef.Sprite_Btn_Close_Blurry;
					break;
					case BtnCloseType.Hide:
					BtnClose.gameObject.SetActive(false);
					break;
			}
		}
	}

}

