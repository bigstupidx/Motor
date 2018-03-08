using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class UserInfoBoard : SingleUIStackBehaviour<UserInfoBoard> {
		public const string UIPrefabPath = "UI/Board/UserInfoBoard/UserInfoBoard";

		public static void Show() {
			string[] UINames =
			{
				UICommonItem.MENU_BACKGROUND,
				UIPrefabPath,
				UICommonItem.TOP_BOARD
			};
			ModMenu.Ins.Cover(UINames);
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			Client.User.UserInfo.Setting.OnNickNameChange += ChangeNickName;
			Client.User.UserInfo.Setting.OnAvatarChange += ChangeAvatar;
			Init();
		}

		public override void OnUIDespawn() {
			base.OnUIDespawn();
			Client.User.UserInfo.Setting.OnNickNameChange -= ChangeNickName;
			Client.User.UserInfo.Setting.OnAvatarChange -= ChangeAvatar;
			Save();
		}

		public Text NickName;
		public Image avatar;
		public Slider Music;
		public Slider Sfx;
		public Toggle ToggleBtn;
		public Toggle ToggleGravity;
		public Toggle ToggleHighQuality;
		public Toggle ToggleLowQuality;

		public GameObject QualityGroup;

		private void Init() {
			SettingInfo setting = Client.User.UserInfo.Setting;
			NickName.text = setting.Nickname;
			avatar.sprite = Client.User.ChoosedHeroInfo.Data.Icon.Sprite;
			this.Music.onValueChanged.RemoveAllListeners();
			this.Music.onValueChanged.AddListener(OnMusicVolumChange);
			this.Music.value = setting.MusicVolume;
			this.Sfx.onValueChanged.RemoveAllListeners();
			this.Sfx.onValueChanged.AddListener(OnSfxVolumeChange);
			this.Sfx.value = setting.SfxVolume;
			this.NickName.text = setting.Nickname;
			this.ToggleBtn.isOn = setting.ControlMode == GameControlMode.Btn;
			this.ToggleGravity.isOn = setting.ControlMode == GameControlMode.GravitySwipe;
			this.ToggleLowQuality.isOn = setting.GameQuality == GameQuality.Low;
			this.ToggleHighQuality.isOn = setting.GameQuality == GameQuality.High;

			this.QualityGroup.SetActive(!Client.Config.DefaultLowQuality);
		}

		public void ChangeNickName(string name) {
			NickName.text = name;
		}

		public void ChangeAvatar(string index) {
			avatar.sprite = UIDataDef.Get_Player_Avatar(index);
		}

		public void OnBtnChangeNickNameClick() {
			ChangeNickNameDialog.Show();
		}

		public void OnBtnChangeAvatarClick() {
			//CommonDialog.Show ("提示", "功能尚未开放！", "确定", null, null, null);
			//ModMenu.Ins.ShowChangeAvatarDialog ();
		}


		public void OnMusicVolumChange(float value) {
			SfxManager.Ins.SetMusicVolume(value);
		}

		public void OnSfxVolumeChange(float value) {
		}

		public void OnToggleHighQuality(bool toggle) {
			if (toggle) {
				Client.User.UserInfo.Setting.GameQuality = GameQuality.High;
			}
		}

		public void OnToggleLowQuality(bool toggle) {
			if (toggle) {
				Client.User.UserInfo.Setting.GameQuality = GameQuality.Low;
			}
		}

		private void Save() {
			SettingInfo setting = Client.User.UserInfo.Setting;
			setting.MusicVolume = this.Music.value;
			setting.SfxVolume = this.Sfx.value;
			if (this.ToggleBtn.isOn) {
				setting.ControlMode = GameControlMode.Btn;
			} else if (this.ToggleGravity.isOn) {
				setting.ControlMode = GameControlMode.GravitySwipe;
			}

			if (this.ToggleLowQuality.isOn) {
				setting.GameQuality = GameQuality.Low;
			} else if (this.ToggleHighQuality.isOn) {
				setting.GameQuality = GameQuality.High;
			}
			if (Game.BikeManager.Ins == null) {//如果处于游戏中说明当前在暂停界面，不改变音效音量，等到恢复游戏时会自动处理音量
				SfxManager.Ins.SetSfxVolume(this.Sfx.value);
			}
		}



		//Test
		private float timer = 0;
		void Update() {
			if (Input.touchCount >= 3) {
				ModMenu.Ins.Back();
				HideSettingBoard.Show();
				return;
			}
			if (Input.GetKey(KeyCode.UpArrow)) {
				this.timer += Time.deltaTime;
				if (this.timer > 1) {
					ModMenu.Ins.Back();
					HideSettingBoard.Show();
				}
			} else {
				this.timer = 0;
			}
		}
	}
}
