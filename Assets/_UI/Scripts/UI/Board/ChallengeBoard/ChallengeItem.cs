using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {

	public class ChallengeItem : EnhancedScrollerCellView {
		public Text RankLimit;
		public Text PlayTimes;
		public RawImage Img;
		public Image[] Rewards;
		public GameObject Mask;

		public int DataIndex { get; private set; }
		private ChallengeInfo Info;
		public TweenCanvasGroupAlpha tweenalpha;

		public void SetData(int index, ChallengeInfo info) {
			tweenalpha.ResetToBeginning();
			tweenalpha.PlayForward();
			this.Info = info;
			RankLimit.text = (BikeRank)Info.Data.LevelBike + (LString.GAMEUI_CHALLENGEITEM_SETDATA).ToLocalized();
			PlayTimes.text = (Info.Data.MatchTimes - info.hasPlay) + "";

			foreach (var item in Info.Data.LevelTasks) {
				int i = 0;
				foreach (var reward in item.RewardList) {
					Rewards[i].sprite = reward.Data.Icon.Sprite;
					i++;
				}
				break;
			}
			Mask.SetActive(!Info.OutOfTimes());
			Img.texture = UIDataDef.GetModeLinTexture(Info.Data.BG);
		}
		public void OnClick() {
			if (!this.Info.OutOfTimes()) {
				CommonTip.Show("参赛次数已用完");
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				return;
			}
			if (Client.User.ChoosedBikeInfo.Data.Rank != (BikeRank)Info.Data.LevelBike) {
				CommonTip.Show((LString.GAMEUI_CHALLENGEITEM_ONCLICK).ToLocalized());
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				return;
			}
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Btn);
			GamePrepareBoard.Show(Info);
		}
	}
}
