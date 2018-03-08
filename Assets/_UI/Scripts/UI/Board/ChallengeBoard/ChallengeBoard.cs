using System.Runtime.InteropServices;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {

	public class ChallengeBoard : SingleUIStackBehaviour<ChallengeBoard>, IEnhancedScrollerDelegate {
		public const string UIPrefabPath = "UI/Board/ChallengeBoard/ChallengeBoard";

		public static void Show() {
			string[] UIPrefabNames ={
				UICommonItem.MENU_BACKGROUND,
				UIPrefabPath,
				UICommonItem.TOP_BOARD
			};

			GroupIns = ModMenu.Ins.Cover(UIPrefabNames, "ChallengeBoard");
			ChallengeBoard ins = (GroupIns[1]).Instance.GetComponent<ChallengeBoard>();
			ins.Init();
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			Reload();
		}

		public void OnUILeaveStack() {
			GroupIns = null;
		}

		public static UIGroup GroupIns { get; private set; }

		private SmallList<ChallengeInfo> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		public void Init() {
			Scroller.Delegate = this;
			Reload();
		}

		public void ShowRuleDialog(bool show) {
			CommonDialog.Show(LString.GAMECLIENT_COMMON_GUIZE_TEXT.ToLocalized(), Client.Challenge.Rule,LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(),null);
//			if (show) {
//				ChampionshipRuleDialog.Show(Client.Challenge.Rule);
//			} else {
//				ModMenu.Ins.Back();
//			}
		}

		public void Reload() {
			_data = new SmallList<ChallengeInfo>();
			var list = Client.User.UserInfo.ChallengeInfoList;
			foreach (var info in list.Values) {
				_data.Add(info);
			}
			Scroller.ReloadData();
		}

		#region scroller
		public int GetNumberOfCells(EnhancedScroller scroller) {
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			ChallengeItem item = scroller.GetCellView(CellViewPrefab) as ChallengeItem;
			item.SetData(dataIndex, _data[dataIndex]);
			return item;
		}
		#endregion

	}
}
