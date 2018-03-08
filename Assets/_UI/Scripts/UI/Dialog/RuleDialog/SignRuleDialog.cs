namespace GameUI {
	public class SignRuleDialog : RuleDialog {

		public const string UIPrefabPath = "UI/Dialog/RuleDialog/SignRuleDialog";

		public static void Show(string content) {
			SignRuleDialog ins = ModMenu.Ins.Overlay(new string[] { UIPrefabPath })[0].Instance.GetComponent<SignRuleDialog>();
			ins.Init(content);
		}
	}

}

