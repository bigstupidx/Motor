using EnhancedUI.EnhancedScroller;

namespace Joystick.UGUI {
	public class FocusEnhancedCell : FocusItemPointerClick {
		public EnhancedScrollerCellView cellView;
		public EnhancedScroller Scroller;

		public bool ClickOnFocus = false;

		protected override void Awake() {
			base.Awake();
			if (cellView == null) {
				cellView = GetComponent<EnhancedScrollerCellView>();
			}
		}

		private void JumpTo(int index) {
			int i = index;
			float sOffset = 0.5f;
			float cOffset = 0.5f;
			int max = Scroller.Delegate.GetNumberOfCells(Scroller);
			sOffset = (float)i / (max - 1);
			cOffset = (float)i / (max - 1);
			Scroller.JumpToDataIndex(i, sOffset, cOffset, true, EnhancedScroller.TweenType.linear, 0.1f);
		}

		public override void OnFocused(FocusItemBase lastFocus) {
			base.OnFocused(lastFocus);
			if (Scroller == null) {
				Scroller = GetComponentInParent<EnhancedScroller>();
			}
			JumpTo(cellView.dataIndex);

			if (this.ClickOnFocus) {
				OnConfirmDown();
			}
		}
	}
}