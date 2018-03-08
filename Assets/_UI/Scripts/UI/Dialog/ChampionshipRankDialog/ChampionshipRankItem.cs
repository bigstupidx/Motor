
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine.UI;

namespace GameUI
{
	public class ChampionshipRankItem : EnhancedScrollerCellView
	{
		public Image Bg;
		public Image RankImg;
		public Text RankTxt;
		public Image Icon;
		public Text NickName;
		public Text Bike;
		public Text RunTime;
		public Image TimeFrame;
        public int DataIndex { get; private set; }

		public void SetData(int index, RankData info)
		{
            DataIndex = index;

			//if (info.Rank <= 3)
			//{
			//	RankImg.gameObject.SetActive(true);
			//	RankTxt.gameObject.SetActive(false);
			//	RankImg.sprite = UIDataDef.GetRankMedal(info.Rank);
			//}
			//else
			//{
			//	RankImg.gameObject.SetActive(false);
			//	RankTxt.gameObject.SetActive(true);
			//	RankTxt.text = info.Rank.ToString();
			//}
            if(info.Rank == 1)
            {
                RankImg.color = UIDataDef.RankFirst;
            }
            else
            {
                RankImg.color = UIDataDef.RankLast;
            }
			RankTxt.text = info.Rank.ToString ();
			NickName.text = info.NickName;
			Bike.text = info.Bike == null?Client.Bike.Random().Name:info.Bike.Name;
			RunTime.text = CommonUtil.GetFormatTime(info.RunTime);
			Icon.sprite = UIDataDef.GetCountryOrRegionFlag(info.Region);
			SetAsSelf(info.PlayerID == Client.User.UserInfo.Setting.UserId);
		}

		public void SetAsSelf(bool isSelf)
		{
			Bg.color = isSelf
				? UIDataDef.RankBG_Select
				: UIDataDef.RankBG_Normal;

            NickName.color = isSelf
                ? UIDataDef.SelfText
                : UIDataDef.OtherText;
            //NickName.GetComponent<Outline>().effectColor = isSelf ? UIDataDef.Tan : UIDataDef.DarkBlue;
            //Bike.GetComponent<Outline>().effectColor = isSelf ? UIDataDef.Tan : UIDataDef.DarkBlue;
            //TimeFrame.color = isSelf ? UIDataDef.Tan : UIDataDef.DarkBlue;
            //RunTime.GetComponent<Outline>().effectColor = isSelf ? UIDataDef.Tan : UIDataDef.DarkBlue;
        }

	}


}
