using GameClient;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI {
	public enum RewardCaseState {
		None = -1,
		NotReach = 0,
		CanGet = 1,
		Rewarded = 2
	}

	public class RewardCase : MonoBehaviour, IPointerClickHandler {

		public UIEffect_LoadOnStart Effect;
		public Image Case;

		private RewardCaseState state = RewardCaseState.None;

		public void Init(ChapterInfo info) {
			if (!info.IsRewarded) {
				if (info.IsGetAllStars()) {
					state = RewardCaseState.CanGet;
					Case.sprite = UIDataDef.Sprite_Icon_Case_Close;
					Effect.gameObject.SetActive(true);

				} else {
					state = RewardCaseState.NotReach;
					Case.sprite = UIDataDef.Sprite_Icon_Case_Close;
					Effect.gameObject.SetActive(false);
				}
			} else {
				state = RewardCaseState.Rewarded;
				Case.sprite = UIDataDef.Sprite_Icon_Case_Open;
				Effect.gameObject.SetActive(false);
			}
		}

		public void OnPointerClick(PointerEventData eventData) {
			switch (state) {
				case RewardCaseState.NotReach:
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Btn);
					CommonTip.Show((LString.GAMEUI_REWARDCASE_ONPOINTERCLICK).ToLocalized());
					break;
				case RewardCaseState.CanGet:
					Client.Match.GetPerfectReward(LevelChooseBoard.Ins.Chapter.Data.ID, (b) => {
						if (b) {
							SfxManager.Ins.PlayOneShot(SfxType.SFX_Blink);
							RewardDialog.Show(LevelChooseBoard.Ins.Chapter.Data.RewardList);
						}
					});
					break;
				case RewardCaseState.Rewarded:
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Btn);
					CommonTip.Show((LString.GAMEUI_REWARDCASE_ONPOINTERCLICK_1).ToLocalized());
					break;
			}

			Init(Client.User.UserInfo.ChapterInfoList[LevelChooseBoard.Ins.Chapter.Data.ID]);
		}
	}

}

