using XUI;

namespace GameUI {
	public class ModMenu : UIModWithEvent<ModMenu> {

		public override void OnUIGroupCreated(UIGroup uiGroup) {
			base.OnUIGroupCreated(uiGroup);
			if (Joystick.FocusManager.TVMode) {
				uiGroup.gameObject.AddComponent<Joystick.FocusRoot>();
			}
		}
	}
}
