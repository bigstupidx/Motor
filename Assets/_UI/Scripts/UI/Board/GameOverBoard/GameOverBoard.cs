using UnityEngine;
using Game;
using GameClient;
using UnityEngine.UI;
using System.Collections;

namespace GameUI {
	public class GameOverBoard : Singleton<GameOverBoard> {

		#region base
		public const string UIPrefabPath = "UI/Board/GameOverBoard/GameOverBoard";
		public static void Show() {
			string[] UIPrefabNames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath,
			};
			GameOverBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "GameOverBoard")[1].Instance.GetComponent<GameOverBoard>();
			ins.Init();

			if (Client.Game.MatchInfo.MatchMode == MatchMode.Guide) {
				Client.Spree.OnArriveShowPoint(ShowPoint.GuideOver);
				return;
			}
			if (GameModeBase.Ins.GetWinOrFail()) {
				Client.Spree.OnArriveShowPoint(ShowPoint.GameWin);
			} else {
				Client.Spree.OnArriveShowPoint(ShowPoint.GameFail);
			}
		}

		#endregion

		public RectTransform TitleRt;
		public Text TitleTxt;
		public UIEffect_LoadOnStart[] LightEffect;
		public MaskableGraphic[] NeedGrey;
		public GameObject LossBtns;
		public GameObject BtnUphero, BtnUpbike;
		public Vector2 NormalTitlePosition;
		public Vector2 OnlineTitlePosition;
		public Vector2 ChampionshipTitlePosition;
		public TweenPosition ImageTweener;
		public TweenScale ImageScaleTweener;

		public UIGameOverInfoBase UIGameOverInfo;

		public void Init() {
			if (GameModeBase.Ins.GetWinOrFail()) {
				foreach (var maskableGraphic in NeedGrey) {
					maskableGraphic.SetGreyMaterail(false);
				}
			} else {
				foreach (var maskableGraphic in NeedGrey) {
					maskableGraphic.SetGreyMaterail(true);
				}
			}
			TitleTxt.gameObject.SetActive(false);
			LossBtns.SetActive(false);
			ImageTweener.ResetToBeginning();
			ImageScaleTweener.ResetToBeginning();
			ShowTweener(ImageScaleTweener, 0f);
			ShowTweener(ImageTweener, 0.5f);
			StartCoroutine(DelayCountDownBoard());
			BtnUphero.SetActive(true);
			BtnUpbike.SetActive(true);
			if (GameModeBase.Ins.GetWinOrFail()) {
				this.DelayInvoke(Win, 1f);
			} else this.DelayInvoke(Loss, 1f);
		}
		private void ShowTweener(UITweener tweener, float delay) {
			this.DelayInvoke(() => {
				tweener.ResetToBeginning();
				tweener.PlayForward();
			}, delay);
		}

		IEnumerator DelayCountDownBoard() {
			yield return new WaitForSeconds(1f);
			switch (Client.Game.MatchInfo.MatchMode) {

				case MatchMode.Championship:
					UIGameOverInfoChampion.Show(); break;
				case MatchMode.Guide:
					break;
				case MatchMode.Normal:
				case MatchMode.Challenge:
					UIGameOverInfoNormal.Show(); break;
				default:
					UIGameOverInfoOnline.Show(); break;
			}
		}

		public void Win() {
			TitleTxt.gameObject.SetActive(true);
			foreach (var effect in LightEffect) {
				effect.gameObject.SetActive(true);
			}
			foreach (var maskableGraphic in NeedGrey) {
				maskableGraphic.SetGreyMaterail(false);
			}
			if (Client.Game.MatchInfo.MatchMode == MatchMode.Challenge) {
				TitleTxt.text = (LString.GAMEUI_GAMEOVERBOARD_WIN).ToLocalized();
			} else {
				TitleTxt.text = (LString.GAMEUI_GAMEOVERBOARD_WIN_1).ToLocalized();
			}
			TitleTxt.color = UIDataDef.RankFirst;
			if (Client.Game.MatchInfo.MatchMode != MatchMode.Guide) {
				LossBtns.SetActive(false);
			} else {
				LossBtns.SetActive(true);
				BtnUpbike.SetActive(false);
				BtnUphero.SetActive(false);
			}
		}

		public void Loss() {
			TitleTxt.gameObject.SetActive(true);
			foreach (var effect in LightEffect) {
				effect.gameObject.SetActive(false);
			}
			foreach (var maskableGraphic in NeedGrey) {
				maskableGraphic.SetGreyMaterail(true);
			}
			TitleTxt.text = (LString.GAMEUI_GAMEOVERBOARD_LOSS).ToLocalized();
			TitleTxt.color = UIDataDef.RankLast;
			LossBtns.SetActive(true);
		}
	}
}


