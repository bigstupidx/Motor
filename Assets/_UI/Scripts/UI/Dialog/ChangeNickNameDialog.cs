using System;
using GameClient;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class ChangeNickNameDialog : UIStackBehaviour {
		public const string UIPrefabPath = "UI/Dialog/ChangeNickNameDialog/ChangeNickNameDialog";

		public static void Show() {
			string[] UIPrefabNames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath,
				UICommonItem.TOP_BOARD_BACK
			};
			ModMenu.Ins.Overlay(UIPrefabNames);
		}

		public override void OnUISpawned() {
			Init();
		}

		public InputField NickName;

		void Awake() {
			NickName.characterLimit = Client.Nickname.CharacterLimit();
		}

		public void Init() {
			NickName.text = Client.User.UserInfo.Setting.Nickname;
		}

		public void OnBtnRandomClick() {
			NickName.text = Client.Nickname.Lib.Random;
		}

		public void OnBtnCloseClick() {
			ModMenu.Ins.Back();
		}

		public void OnBtnOKClick() {
			string name = NickName.text;
			CheckNickName(name, b => {
				if (b == ChangeNickBackType.Success) {
					ModMenu.Ins.Back();
				}
			});
		}

		public static void CheckNickName(string name, Action<ChangeNickBackType> onDone) {
			name = name.Trim();
			//昵称检查
			if (string.IsNullOrEmpty(name)) {
				CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK).ToLocalized());
				onDone(ChangeNickBackType.Fail);
				return;
			}

			if (name == Client.User.InitNickName) {
				CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_5).ToLocalized());
				onDone(ChangeNickBackType.AlreadyExist);
				return;
			}

			bool isFilter = false;
			Client.DFA.Filter(name, out isFilter);
			if (isFilter) {
				CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_1).ToLocalized());
				onDone(ChangeNickBackType.Fail);
				return;
			}

			if (Client.User.UserInfo.Setting.UserId != -1) {
				Client.Nickname.ChangeNickName(name, (type) => {
					switch (type) {
						case ChangeNickBackType.Success:
							//						CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_2).ToLocalized());
							break;
						case ChangeNickBackType.AlreadyExist:
							CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_5).ToLocalized());
							break;
						case ChangeNickBackType.Fail:
							CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_3).ToLocalized());
							break;
						case ChangeNickBackType.NetworkFail:
							CommonTip.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_3).ToLocalized());
							break;
					}
					onDone(type);
				});
			} else {
				WaittingTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_4).ToLocalized());
				Client.Nickname.GetPlayerId(name, (type) => {
					WaittingTip.Hide();
					switch (type) {
						case ChangeNickBackType.Success:
							//						CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_2).ToLocalized());
							break;
						case ChangeNickBackType.AlreadyExist:
							CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_5).ToLocalized());
							break;
						case ChangeNickBackType.Fail:
							CommonTip.Show((LString.GAMEUI_CHANGENICKNAMEDIALOG_ONBTNOKCLICK_3).ToLocalized());
							break;
						case ChangeNickBackType.NetworkFail:
							CommonTip.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_3).ToLocalized());
							break;
					}
					onDone(type);
				});
			}
		}

	}


}
