using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick;
using XUI;

namespace GameUI {
	public class ChampionshipBoard : SingleUIStackBehaviour<ChampionshipBoard>, IEnhancedScrollerDelegate {

		#region base

		public const string UIPrefabPath = "UI/Board/ChampionshipBoard/ChampionshipBoard";

		public static void Show() {
			string[] UIPrefabNames ={
				UIPrefabPath,
				UICommonItem.TOP_BOARD
			};
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.OnlinePrepareBoard);
			ChampionshipBoard ins = ModMenu.Ins.Cover(UIPrefabNames, "ChampionshipBoard")[0].Instance.GetComponent<ChampionshipBoard>();
			ins.Init();
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ShowBike(bikePrefab);
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.OnlinePrepareBoard);
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ShowBike(bikePrefab);
		}


		#endregion

		private SmallList<ChampionshipInfo> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;

		public void Init() {
			Scroller.Delegate = this;
			Reload();

			if (FocusManager.TVMode) {
				this.DelayInvoke(() => {
					FocusManager.Ins.Focus(transform.GetComponentInChildren<FocusItemBase>());
				}, 0.05f);
			}
		}

		public void ShowRuleDialog(bool show) {
			if (show) {
				ChampionshipRuleDialog.Show(Client.Championship.Rule);
			} else {
				ModMenu.Ins.Back();
			}
		}

		private void Reload() {
			_data = new SmallList<ChampionshipInfo>();
			var list = Client.Championship.GetChampionshipList();
			for (int i = 0; i < list.Count; i++) {
				_data.Add(list[i]);
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
			ChampionshipItem item = scroller.GetCellView(CellViewPrefab) as ChampionshipItem;
			item.SetData(dataIndex, _data[dataIndex]);
			return item;
		}
		#endregion
	}


}
