using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class CommonTip : MonoBehaviour, IUIPoolBehaviour {

		public const string PREFAB_PATH = "UI/Tip/CommonTip";

		public TweenCanvasGroupAlpha Ta;

		public Text Content;

		public static void Show(string text) {
			CommonTip tip = ModTip.Ins.Spawn(PREFAB_PATH).GetComponent<CommonTip>();
			tip.Content.text = text;
		}

		public void OnUISpawned() {
			Ta.ResetToBeginning();
			Ta.PlayForward();
		}

		public void OnUIDespawn() {
		}

		public void __AutoHide() {
			ModTip.Ins.Despawn(gameObject, false);
		}

	}

}
