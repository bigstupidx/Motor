using UnityEngine;
using XUI;

namespace GameUI {
	public class TopBoard : SingleUIStackBehaviour<TopBoard> {
		public TopBoardItem[] Items;
		public GameObject BtnBack;
		public Vector2 NormalPos;
		public Vector2 PosOffset;

		public override void OnUISpawned() {
			base.OnUISpawned();
			if (MainBoard.Ins != null && MainBoard.Ins.gameObject.activeSelf) {
				BtnBack.SetActive(false);
			} else {
				BtnBack.SetActive(true);
			}
			Refresh();
		}

		public void Refresh() {
			foreach (TopBoardItem t in this.Items) {
				t.Refresh();
			}
		}

	}

}

