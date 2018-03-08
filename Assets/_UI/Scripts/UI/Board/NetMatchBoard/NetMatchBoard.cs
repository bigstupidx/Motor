using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using Joystick;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {

	public class NetMatchBoard : UIStackBehaviour, IEnhancedScrollerDelegate {
		#region base

		public const string PrefabPath = "UI/Board/NetMatchBoard/NetMatchBoard";

		public static void Show(bool destroyBefore = false) {
			string[] PrefabNames ={
				PrefabPath,
				UICommonItem.TOP_BOARD
			};

			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.OnlinePrepareBoard);
			GroupIns = ModMenu.Ins.Cover(PrefabNames, "NetMatchBoard");
			NetMatchBoard ins = (GroupIns[0]).Instance.GetComponent<NetMatchBoard>();
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ShowBike(bikePrefab);
		}

		public override void OnUISpawned() {
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.OnlinePrepareBoard);
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ShowBike(bikePrefab);
			Client.Online.StartOnline();
		}

		public override void OnUIDespawn() {
			Client.Online.StopOnline();
		}

		public override void OnUILeaveStack() {
			GroupIns = null;
			Client.Online.OnNetworkDisable -= OnNetworkDisable;
		}

		public override void OnUIEnterStack() {
			Client.Online.OnNetworkDisable += OnNetworkDisable;
		}

		#endregion
		public static UIGroup GroupIns { get; private set; }

		private SmallList<GameMode> _data;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;
		public int DataIndex;

		void Start() {
			GetComponent<GraphicRaycaster>().ignoreReversedGraphics = false;
			Scroller.Delegate = this;
			Reload();
			DataIndex = 0;
		}

		private void OnNetworkDisable() {
			CommonDialog.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_3).ToLocalized(), (LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_4).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, () => {
				ModMenu.Ins.BackTo(GroupIns, false);
			}, null);
		}

		private void Reload() {
			_data = new SmallList<GameMode>();
			List<GameMode> list = Client.Online.GetMatchMode();
			for (int i = 0; i < list.Count; i++) {
				_data.Add(list[i]);
			}
			Scroller.ReloadData();

			if (FocusManager.TVMode) {
				this.DelayInvoke(() => {
					FocusManager.Ins.Focus(this.transform.GetComponentInChildren<FocusItemBase>());
				}, 0.05f);
			}
		}


		public int GetNumberOfCells(EnhancedScroller scroller) {
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			NetMatchItem item = scroller.GetCellView(CellViewPrefab) as NetMatchItem;
			item.SetData(this, dataIndex, _data[dataIndex]);
			return item;
		}

	}
}
