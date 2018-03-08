using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameUI {
	public class CommonDialog : MonoBehaviour {


		public const string UIPrefabPath = "UI/Dialog/CommonDialog";

		public static string[] UINames =
		{
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath
		};

		public static void Show(string Title, string Content, string ConfirmText, UnityAction ConfirmAction) {
			Show(Title,Content,ConfirmText,null,ConfirmAction,null);
		}

		public static void Show(string Title, string Content, string ConfirmText, string CancelText, UnityAction ConfirmAction, UnityAction CancelAction) {
			CommonDialog thiz = ModMenu.Ins.Overlay(UINames)[1].Instance.GetComponent<CommonDialog>();
			thiz.Init(Title, Content, ConfirmText, CancelText, ConfirmAction, CancelAction);
		}

		public Text Title;
		public Text Content;
		public Text Confirm;
		public Text Cancel;

		public Button BtnConfirm;
		public Button BtnCancel;

		public void Init(string Title, string Content, string ConfirmText, string CancelText, UnityAction ConfirmAction, UnityAction CancelAction) {
			this.Title.text = Title;
			this.Content.text = Content;
			this.Cancel.text = CancelText;
			this.Confirm.text = ConfirmText;
			this.Cancel.text = CancelText;
			if (string.IsNullOrEmpty(ConfirmText)) {//没有确认按钮
				this.BtnCancel.transform.SetLocalPositionX(0);
				this.BtnConfirm.gameObject.SetActive(false);
			} else {
				this.BtnConfirm.gameObject.SetActive(true);
				this.BtnCancel.transform.SetLocalPositionX(-270);
			}

			if (string.IsNullOrEmpty(CancelText)) {//没有取消按钮
				this.BtnConfirm.transform.SetLocalPositionX(0);
				this.BtnCancel.gameObject.SetActive(false);
			} else {
				this.BtnCancel.gameObject.SetActive(true);
				this.BtnConfirm.transform.SetLocalPositionX(270);
			}
			this.BtnConfirm.onClick.RemoveAllListeners();
			this.BtnCancel.onClick.RemoveAllListeners();
			this.BtnConfirm.onClick.AddListener(() => {
				ModMenu.Ins.Back();
				if (ConfirmAction != null) {
					ConfirmAction.Invoke();
				}
			});
			this.BtnCancel.onClick.AddListener(() => {
				ModMenu.Ins.Back();
				if (CancelAction != null) {
					CancelAction.Invoke();
				}
			});

		}

	}
}