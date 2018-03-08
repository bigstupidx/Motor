namespace GameUI {
	public class ChampionshipRuleDialog : RuleDialog {

		public const string UIPrefabPath = "UI/Dialog/RuleDialog/ChampionshipRuleDialog";

		public static void Show(string content) {
			ChampionshipRuleDialog ins = ModMenu.Ins.Overlay(new string[] { UIPrefabPath })[0].Instance.GetComponent<ChampionshipRuleDialog>();
			ins.Init(content);
		}

	}
}
