using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {

	public class ChampionshipItem : EnhancedScrollerCellView {
		public Button Btn;
		public Text Name;
		public Text staminaCount;
		public RawImage Img;
		private GameMode mode;

		public Text RemianTime;

		public int DataIndex { get; private set; }
		private ChampionshipInfo championshipInfo;
		private bool isFinish = false;
		public TweenCanvasGroupAlpha tweenalpha;

		void Awake() {
			Btn.onClick.AddListener(OnClick);
		}

		void Update() {
			if (!isFinish) {
				RefreshTime();
			}
		}

		public void SetData(int index, ChampionshipInfo info) {
			tweenalpha.ResetToBeginning();
			tweenalpha.PlayForward();
			this.championshipInfo = info;
			Name.text = info.Data.Name;
			//Img.texture = info.ChampionshipData.Icon.Texture ?? UIDataDef.ChampionshipDefaultImg;
			Img.texture = UIDataDef.ChampionshipDefaultImg;
			staminaCount.text = this.championshipInfo.Data.NeedStamina.ToString();
			RefreshTime();
		}

		private void RefreshTime() {
			isFinish = championshipInfo.RemainTime <= 0;
			RemianTime.text = isFinish ? (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_REFRESHTIME).ToLocalized() : CommonUtil.GetFormatTimeDHMS(championshipInfo.RemainTime);
		}

		public void OnClick() {
			ChampionshipDetailBoard.Show(championshipInfo);
		}
	}


}
