using System.Collections.Generic;
using EnhancedUI;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class RankBoard : SingleUIStackBehaviour<RankBoard>, IEnhancedScrollerDelegate {
		#region base
		public const string UIPrefabPath = "UI/Board/RankBoard/RankBoard";

		public static string[] UIPrefabNames =
		{
			UICommonItem.MENU_BACKGROUND,
			UIPrefabPath,
			UICommonItem.TOP_BOARD
		};

		public static void Show(RankInfo rankInfo) {
			RankBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "RankBoard")[1].Instance.GetComponent<RankBoard>();
			ins.SetTag();
			ins.Reload(rankInfo, true);
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			Scroller.Delegate = this;
			BtnScroller.Delegate = this;
		}

		#endregion

		public ChampionshipRankItem SelfRankItem;
		public GameObject SelfNull;

		private SmallList<RankData> _data;
		private SmallList<RankItemInfo> _BtnData;
		public EnhancedScroller Scroller;
		public EnhancedScroller BtnScroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public EnhancedScrollerCellView BtnCellViewPrefab;
		public float CellSize;
		public float BtnCellSize;

		public RankData selfRankInfo;
		public int SelectRankID;

		public Image BtnWorldBG;
		public Image BtnLocalBG;

		public void OnBtnWorldRankClick() {
			Client.Rank.IsLocal = false;
			ChangeRank();
		}

		public void OnBtnLocalRankClick() {
			Client.Rank.IsLocal = true;
			ChangeRank();
		}

		private void SetTag() {
			if (Client.Rank.IsLocal) {
				BtnWorldBG.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Blue;
				BtnLocalBG.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Yellow;
			} else {
				BtnLocalBG.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Blue;
				BtnWorldBG.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Yellow;
			}
		}

		public void Reload(RankInfo rankInfo, bool all) {
			//set self rank
			selfRankInfo = Client.Rank.IsLocal ? rankInfo.LocalSelfData : rankInfo.SelfData;
			if (selfRankInfo != null) {
				SelfNull.SetActive(false);
				SelfRankItem.gameObject.SetActive(true);
				SelfRankItem.SetData(-1, selfRankInfo);
			} else {
				SelfRankItem.gameObject.SetActive(false);
				SelfNull.SetActive(true);
			}

			_data = new SmallList<RankData>();
			var list = Client.Rank.IsLocal ? rankInfo.LocalListItems : rankInfo.ListItems;
			for (int i = 0; i < list.Count; i++) {
				_data.Add(list[i]);
			}
			Scroller.ReloadData();

			if (all) {
				_BtnData = new SmallList<RankItemInfo>();
				List<RankItemInfo> itemInfoList = Client.Rank.GetRankItemList();
				for (int i = 0; i < itemInfoList.Count; i++) {
					_BtnData.Add(itemInfoList[i]);
				}
				BtnScroller.ReloadData();
			} else {
				this.BtnScroller.RefreshActiveCellViews();
			}
		}

		public void ChangeRank() {
			SetTag();
			if (Client.Rank.IsLocal) {
				WaittingTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK).ToLocalized());
				Client.Rank.GetLocalRankInfo(Client.Rank.CurrentRankID, (r) => {
					WaittingTip.Hide();
					this.Reload(r, false);
				});
			}

			this.Reload(Client.Rank.GetRankInfo(Client.Rank.CurrentRankID), false);
		}

		#region scroller
		public int GetNumberOfCells(EnhancedScroller scroller) {
			if (scroller == Scroller) {
				return _data.Count;
			} else {
				return _BtnData.Count;
			}
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			if (scroller == Scroller) {
				return CellSize;
			} else {
				return BtnCellSize;
			}
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			if (scroller == Scroller) {
				ChampionshipRankItem item = scroller.GetCellView(CellViewPrefab) as ChampionshipRankItem;
				item.SetData(dataIndex, _data[dataIndex]);
				return item;
			} else {
				RankBtnItemInfo item = BtnScroller.GetCellView(BtnCellViewPrefab) as RankBtnItemInfo;
				item.SetData(dataIndex, _BtnData[dataIndex]);
				return item;
			}

		}
		#endregion
	}


}
