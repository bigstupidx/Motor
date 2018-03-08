using System;
using XUI;

namespace GameUI {

	public class TopBoardBack : UIStackBehaviour {
		public Action OnClick;

		public override void OnUISpawned() {
			base.OnUISpawned();
			this.OnClick = null;
		}

		public void __OnClicked() {
			if (this.OnClick != null) {
				this.OnClick();
			}
			ModMenu.Ins.Back();
		}
	}
}