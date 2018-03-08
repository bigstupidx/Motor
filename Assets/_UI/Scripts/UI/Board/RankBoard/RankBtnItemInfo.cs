using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EnhancedUI.EnhancedScroller;
using GameClient;

namespace GameUI {

	public class RankBtnItemInfo : EnhancedScrollerCellView, IPointerClickHandler {
		public Image BG;
		public Text Name;
		public int ID;

		public int DataIndex { get; private set; }

		public override void RefreshCellView() {
			base.RefreshCellView();
			BG.sprite = UIDataDef.Get_RankBG_Icon(ID == Client.Rank.CurrentRankID);
			Name.color = ID == Client.Rank.CurrentRankID ? UIDataDef.RankTypeSelected : UIDataDef.RankTypeUnselected;
		}

		public void SetData(int dataIndex, RankItemInfo info) {
			DataIndex = dataIndex;
			Name.text = info.Name;
			ID = info.ID;
			BG.sprite = UIDataDef.Get_RankBG_Icon(ID == Client.Rank.CurrentRankID);
			Name.color = ID == Client.Rank.CurrentRankID ? UIDataDef.RankTypeSelected : UIDataDef.RankTypeUnselected;

		}

		public void OnPointerClick(PointerEventData eventData) {
			Client.Rank.CurrentRankID = ID;
			Client.Rank.IsLocal = false;
			RankBoard.Ins.ChangeRank();
		}
	}
}