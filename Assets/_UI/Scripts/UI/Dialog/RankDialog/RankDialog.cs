using System.Collections.Generic;
using Game;
using UnityEngine;
using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class RankDialog : UIStackBehaviour {
		#region base
		public const string UIPrefabPath = "UI/Dialog/RankDialog/RankDialog";

		public static void Show() {
			string[] UINames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath,
				UICommonItem.TOP_BOARD_BACK
			};
			RankDialog ins = ModMenu.Ins.Cover(UINames, "RankDialog")[1].Instance.GetComponent<RankDialog>();
			ins.Init();
		}

		public override void OnUIDespawn() {
			base.OnUIDespawn();
			ClearItems();
		}
		#endregion

		public ScrollRect Scroll;
		public RankListItem RankListItemPrefab;
		public RectTransform GridRectTransform;
		public float CellHeight;

		private List<RankListItem> RankList;

		public void Init() {
			GridRectTransform.sizeDelta = new Vector2(GridRectTransform.rect.width, CellHeight * (Client.Game.MatchInfo.Enemys.Count + 1) + 15 * Client.Game.MatchInfo.Enemys.Count);
			RankList = new List<RankListItem>();
			for (int i = 0; i < RaceManager.Ins.PlayerNum; i++) {
				RankListItem item = Instantiate(RankListItemPrefab);
				if (i < RaceManager.Ins.FinishList.Count) {
					item.SetData(i + 1, RaceManager.Ins.FinishList[i]);
				} else {
					item.SetData(i + 1, RaceManager.Ins.PlayerList[i - RaceManager.Ins.FinishList.Count]);
				}
				RectTransform rt = item.GetComponent<RectTransform>();
				rt.SetParent(GridRectTransform);
				rt.localScale = Vector3.one;
				RankList.Add(item);
			}

			int rank = BikeManager.Ins.CurrentBike.racerInfo.Rank;
			if (rank > 4) {
				if (RaceManager.Ins.PlayerNum > 10 && rank < 9) {
					Scroll.verticalNormalizedPosition = 0.5f;
				} else {
					Scroll.verticalNormalizedPosition = 0;
				}
			} else {
				Scroll.verticalNormalizedPosition = 1;
			}
		}

		void ClearItems() {
			for (int i = 0; i < RankList.Count; i++) {
				Destroy(RankList[i].gameObject);
			}
			RankList.Clear();
		}

		void Update() {
			for (int i = 0; i < RaceManager.Ins.PlayerNum; i++) {
				switch (RaceManager.Ins.RaceMode) {
					case RaceMode.Elimination:
						if (i < RaceManager.Ins.PlayerList.Count) {
							this.RankList[i].SetText(RaceManager.Ins.PlayerList[i], "未淘汰");	
						} else {
							RankList[i].UpdateTime(RaceManager.Ins.FinishList[i - RaceManager.Ins.PlayerList.Count]);
						}
						break;
					default:
						if (i < RaceManager.Ins.FinishList.Count) {
							RankList[i].UpdateTime(RaceManager.Ins.FinishList[i]);
						} else {
							RankList[i].UpdateTime(RaceManager.Ins.PlayerList[i - RaceManager.Ins.FinishList.Count]);
						}
						break;
				}
			}
		}

		public void OnBtnNextClick() {
			GameOverBoard.Show();
		}

	}

}

