using System;
using Game;
using XPlugin.Data.Json;

namespace GameClient {
	public class SettingInfo {

		/// <summary>
		/// 操控方式变化回调
		/// </summary>
		[JsonIgnore]
		public Action<GameControlMode> OnControlModeChange;

		[JsonIgnore]
		public Action<CameraMode> OnCameraModeChange;

		[JsonIgnore]
		public Action<GameQuality> OnGameQualityChange;

		[JsonIgnore]
		public Action<string> OnNickNameChange;

		[JsonIgnore]
		public Action<string> OnAvatarChange;

		//账号
		private int _userId = -1;
		public int UserId {
			get { return _userId; }
			set {
				_userId = value;
				Client.Setting.SaveData();
			}
		}

		//所属国家或地区
		private RegionEnum _region = RegionEnum.UnKnown;

		public RegionEnum Region {
			get { return _region; }
			set {
				_region = value;
				Client.Setting.SaveData();
			}
		}

		/// <summary>
		/// 昵称
		/// </summary>
		public string Nickname {
			get {
				return this._nickname;
			}

			set {
				if (this._nickname != value) {
					this._nickname = value;
					Client.EventMgr.SendEvent(EventEnum.Player_ChangeNickName, value);
					if (OnNickNameChange != null) {
						OnNickNameChange(this._nickname);
					}
					Client.Setting.SaveData();
				}
			}
		}
		private string _nickname = "";

		/// <summary>
		/// 头像
		/// </summary>
		public string Avatar {
			get {
				return this._avatar;
			}

			set {
				if (this._avatar != value) {
					this._avatar = value;
					Client.EventMgr.SendEvent(EventEnum.Player_ChangeAvatar, value);
					if (OnAvatarChange != null) {
						OnAvatarChange(this._avatar);
					}
					Client.Setting.SaveData();
				}
			}
		}
		private string _avatar = "";

		private bool _isChangeName = false;
		/// <summary>
		/// 是否修改过昵称
		/// </summary>
		public bool IsChangeName {
			get {
				return this._isChangeName;
			}
			set {
				if (!_isChangeName) {
					this._isChangeName = value;
					Client.Setting.SaveData();
				}
			}
		}

		private bool _isFirstEnter = true;
		/// <summary>
		/// 是否首次进入游戏
		/// </summary>
		public bool IsFirstEnter {
			get {
				return this._isFirstEnter;
			}
			set {
				this._isFirstEnter = value;
				Client.Setting.SaveData();
			}
		}

		/// <summary>
		/// 游戏操控方式
		/// </summary>
		public GameControlMode ControlMode {
			get {
				return this._controlMode;
			}

			set {
				if (this._controlMode != value) {
					this._controlMode = value;
					Client.Setting.SaveData();
					if (this.OnControlModeChange != null) {
						this.OnControlModeChange(this._controlMode);
					}
				}

			}
		}
		private GameControlMode _controlMode = GameControlMode.Btn;

		/// <summary>
		/// 游戏画质
		/// </summary>
		public GameQuality GameQuality {
			get {
				return this._gameQuality;
			}

			set {
				if (this._gameQuality != value) {
					this._gameQuality = value;
					Client.Setting.SaveData();
				}
				switch (value) {
					case GameQuality.High:
						DeviceLevel.Score = 100;
						break;
					case GameQuality.Low:
						DeviceLevel.Score = 0;
						break;
				}
				if (this.OnGameQualityChange != null) {
					this.OnGameQualityChange(this._gameQuality);
				}

			}
		}

		private GameQuality _gameQuality = GameQuality.Low;

		/// <summary>
		/// 相机模式
		/// </summary>
		public CameraMode CamaerMode {
			get { return _camaerMode; }
			set {
				if (this._camaerMode != value) {
					this._camaerMode = value;
					Client.Setting.SaveData();
					if (this.OnCameraModeChange != null) {
						this.OnCameraModeChange(this._camaerMode);
					}
				}
			}
		}
		private CameraMode _camaerMode = CameraMode.Drift;


		/// <summary>
		/// </summary>
		public float MusicVolume {
			get {
				return this._musicVolume;
			}

			set {
				this._musicVolume = value;
				Client.Setting.SaveData();
			}
		}

		public bool IsMusicOn {
			get {
				return MusicVolume >= 1.0f;
			}
		}
		private float _musicVolume = 1f;

		/// <summary>
		/// 音效音量
		/// </summary>
		public float SfxVolume {
			get {
				return this._sfxVolume;
			}

			set {
				this._sfxVolume = value;
				Client.Setting.SaveData();

			}
		}
		public bool IsSfxOn {
			get {
				return SfxVolume >= 1.0f;
			}
		}

		private float _sfxVolume = 1f;

		public float getVolumeValue(bool isOn) {
			return isOn ? 1.0f : 0.0f;
		}






	}
}