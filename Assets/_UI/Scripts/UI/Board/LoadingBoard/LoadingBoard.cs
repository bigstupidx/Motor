using UnityEngine;
using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class LoadingBoard : SingleUIStackBehaviour<LoadingBoard> {
		#region base

		public const string UIPrefabName = "UI/Board/Common/LoadingBoard";

		public static void Show(string text, float percent, bool showHint) {
			if (Ins == null) {
				ModMenu.Ins.Cover(new string[] { UIPrefabName });
			}
			Ins.UpdateContent(Ins.Txt.text, percent, showHint);
		}


		public override void OnUISpawned() {
			base.OnUISpawned();
			SetLoadingBarValue(0);
			timer = 0f;
		}

		#endregion

		private string _showText;
		private float _loadingPecent;
		public Text Txt;

		public RectTransform barRect;
		private RectTransform parentRect;

		private float timer;

		public void UpdateContent(string text, float percent, bool showHint) {
			Txt.text = text;
			SetLoadingBarValue(percent);
			if (showHint) {
				Txt.text = Client.Hint.GetHint();
			}
		}

		public void SetLoadingBarValue(float percent) {
			if (parentRect == null) {
				parentRect = (RectTransform)barRect.parent;
			}
			percent = Mathf.Clamp01(percent);
			barRect.sizeDelta = new Vector2((parentRect.sizeDelta.x - 10) * percent, barRect.sizeDelta.y);
		}
	}


}
