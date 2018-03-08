using Joystick.UGUI;

namespace Joystick.Other {
	public class ModTagFocusable : FocusItemPointerClick {

		public ModeTag Tag;
		protected override void Awake() {
			base.Awake();
			if (this.Tag == null) {
				this.Tag = GetComponent<ModeTag>();
			}
		}

		public override void OnFocused(FocusItemBase lastFocus) {
			base.OnFocused(lastFocus);
			this.Tag.SetSelectState(true);
		}

		public override void OnLostFocuse(FocusItemBase newFocus) {
			base.OnLostFocuse(newFocus);
			this.Tag.SetSelectState(false);
		}
	}
}