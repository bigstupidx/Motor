using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class WaittingTipWithCancel : MonoBehaviour {

		public const string PREFAB_PATH = "UI/Tip/WaittingTipWithCancel";

		public Text Tip;
		public Button CloseBtn;

		public Action OnCancel;

		public static WaittingTipWithCancel Ins { get; private set; }

		public static void Show(Action onCancel) {
			Show(LString.WaittingTip_Text.ToLocalized(), onCancel);
		}

		public static void Show(string txt, Action onCancel) {
			WaittingTipWithCancel ins = ModTip.Ins.Cover(new string[] { PREFAB_PATH })[0].Instance.GetComponent<WaittingTipWithCancel>();
			ins.Tip.text = txt;
			ins.OnCancel = onCancel;
		}

		public static void Update(string txt) {
			if (Ins != null) {
				Ins.Tip.text = txt;
			}
		}

		public static void Hide() {
			if (Ins != null) {
				ModTip.Ins.Back();
			}
		}

		void OnEnable() {
			Ins = this;
			this.CloseBtn.gameObject.SetActive(true);
			//			this.DelayInvoke(() => {
			//				this.CloseBtn.gameObject.SetActive(true);
			//			}, 2f);
		}

		void OnDisable() {
			Ins = null;
			this.StopAllCoroutines();
		}

		public void __OnCloseClick() {
			Hide();
			if (this.OnCancel != null) {
				this.OnCancel();
			}
		}


	}

}