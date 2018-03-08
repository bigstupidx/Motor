using EnhancedUI.EnhancedScroller;
using UnityEngine;
using XUI;

namespace GameUI {
	public class OnlineLobbyBoard : SingleUIStackBehaviour<OnlineLobbyBoard>, IEnhancedScrollerDelegate {
		public const string PrefabPath = "UI/Dialog/Online/OnlineLobbyBoard";

		public static void Show() {
			string[] PrefabNames ={
				UICommonItem.MENU_BACKGROUND,
				PrefabPath,
			};
			GroupIns= ModMenu.Ins.Cover(PrefabNames);
		}
		public static UIGroup GroupIns { get; private set; }

		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;
		public EnhancedScroller Scroller;

		public GameObject NoRoomTag;

		public float UpdateTime = 1;
		private float _updateTimer = 0;

		public override void OnUILeaveStack() {
			base.OnUILeaveStack();
			GroupIns = null;
		}

		void Start() {
			this.Scroller.Delegate = this;
		}

		void Update() {
			this._updateTimer += Time.unscaledDeltaTime;
			if (this._updateTimer > this.UpdateTime) {
				Lobby.Ins.RoomClient.ToLobbyClient.FetchRooms(0, 100);
				this._updateTimer = 0;
			}
		}

		public void RefreshRoomList() {
			this.NoRoomTag.SetActive(Lobby.Ins.RoomClient.ToLobbyClient.FetchedRooms.Count == 0);
			this.Scroller.ReloadData();
		}


		public int GetNumberOfCells(EnhancedScroller scroller) {
			return Lobby.Ins.RoomClient.ToLobbyClient.FetchedRooms.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return this.CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			LobbyRoomListItem item = scroller.GetCellView(CellViewPrefab) as LobbyRoomListItem;
			item.SetData(dataIndex, Lobby.Ins.RoomClient.ToLobbyClient.FetchedRooms[dataIndex]);
			item.gameObject.name = "room" + dataIndex;
			return item;
		}

		public void OnCreateRoomClick() {
			CreateRoomDialog.Show();
		}

		public void OnInputRoomIdClick() {
			InputRoomIdDialog.Show();
		}

		public void __OnBackClicked() {
			ModMenu.Ins.Back();
			Lobby.Ins.Disconnect();
		}

	}
}