using System.Collections.Generic;
using EnhancedUI;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using GameClient;

namespace GameUI
{
	public class ChampionshipRankDialog : Singleton<ChampionshipRankDialog>, IEnhancedScrollerDelegate {

		#region base
		public const string UIPrefabPath = "UI/Dialog/ChampionshipRankDialog/ChampionshipRankDialog";

		public static string[] UIPrefabNames =
		{
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath,
				UICommonItem.TOP_BOARD
		};

		public static void Show(List<ChampionshipRankInfo> list, ChampionshipRankInfo self) {
			ChampionshipRankDialog ins = ModMenu.Ins.Overlay(UIPrefabNames, "ChampionshipRankDialog")[1].Instance.GetComponent<ChampionshipRankDialog>();
			ins.Reload(list, self);
		}

		void OnUISpawned() {
			Scroller.Delegate = this;
		}

		//		void OnUIDespawn() {
		//			
		//		}
		#endregion

		public ChampionshipRankItem SelfRankItem;
		public GameObject SelfNull;

		private SmallList<ChampionshipRankInfo> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		public ChampionshipRankInfo selfRankInfo;

		public void Reload(List<ChampionshipRankInfo> list, ChampionshipRankInfo self)
		{
			//set self rank
			selfRankInfo = self;
			if (self != null)
			{
				SelfNull.SetActive(false);
				SelfRankItem.gameObject.SetActive(true);
				SelfRankItem.SetData(-1, self);
			}
			else
			{
				SelfRankItem.gameObject.SetActive(false);
				SelfNull.SetActive(true);
			}


			_data = new SmallList<ChampionshipRankInfo>();
			for (int i = 0; i < list.Count; i++)
			{
				_data.Add(list[i]);
			}
			Scroller.ReloadData();
		}

		#region scroller
		public int GetNumberOfCells(EnhancedScroller scroller)
		{
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
		{
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			ChampionshipRankItem item = scroller.GetCellView(CellViewPrefab) as ChampionshipRankItem;
			item.SetData(dataIndex, _data[dataIndex]);
			return item;
		}
		#endregion
	}


}
