using UnityEngine;
using XPlugin.Data.Json;

namespace GameClient {

	public class ModUser : ClientModule {
		/// <summary>
		/// 用户数据
		/// </summary>
		public UserInfo UserInfo = new UserInfo();

		public HeroInfo ChoosedHeroInfo {
			get { return this.UserInfo.HeroList[this.UserInfo.ChoosedHeroID]; }
		}

		public BikeInfo ChoosedBikeInfo {
			get { return this.UserInfo.BikeList[this.UserInfo.ChoosedBikeID]; }
		}

		public WeaponInfo ChoosedWeaponInfo {
			get {
				return Client.Weapon.GetWeaponInfo(this.UserInfo.ChoosedWeaponID);
			}
		}
		/// <summary>
		/// 游戏上场信息（人物、车辆、道具）
		/// </summary>
		public PlayerInfo ChoosedPlayerInfo {
			get {
				if (_choosedPlayerInfo == null) {
					_choosedPlayerInfo = new PlayerInfo() {
						NickName = Client.User.UserInfo.Setting.Nickname,
						Avatar = Client.User.UserInfo.Setting.Avatar,
						ChoosedHero = Client.Hero.GetHeroInfo(Client.User.UserInfo.ChoosedHeroID),
						ChoosedBike = Client.Bike.GetBikeInfo(Client.User.UserInfo.ChoosedBikeID),
						ChoosedWeapon = Client.Weapon.GetWeaponInfo(Client.User.UserInfo.ChoosedWeaponID),
						//TODO 道具 ai 
					};
				}
				//某些情况下昵称没有更新。。这里强制更新下
				this._choosedPlayerInfo.NickName = Client.User.UserInfo.Setting.Nickname;
				this._choosedPlayerInfo.Avatar = Client.User.UserInfo.Setting.Avatar;
				return _choosedPlayerInfo;
			}
		}
		private PlayerInfo _choosedPlayerInfo;

		public void ResetChoosedPlayerInfo() {
			_choosedPlayerInfo = null;
			if (Client.Weapon.GetWeaponInfo(Client.User.UserInfo.ChoosedWeaponID).Amount <= 0) {
				Client.User.UserInfo.ChoosedWeaponID = DataDef.DefalutWeapon;
			}
		}

		public override void InitInfo(string s) {
			if (Client.User.UserInfo.Setting.IsFirstEnter) {
				Client.User.UserInfo.Setting.IsFirstEnter = false;
				Client.User.UserInfo.Setting.Region = SDKManager.Instance.GetCountryOrRegionId();
				Client.User.UserInfo.Setting.Nickname = this.InitNickNameInSdk;
				Client.User.UserInfo.Setting.Avatar = this.InitAvatar;
				Client.Hero.GainHero(DataDef.DefaultHero);
				Client.Bike.GainBike(DataDef.DefaultBike);
				this.UserInfo.ChoosedWeaponID = DataDef.DefalutWeapon;
				this.UserInfo.ChoosedBikeID = DataDef.DefaultBike;
				this.UserInfo.ChoosedHeroID = DataDef.DefaultHero;
				Client.Item.Coin.ChangeAmount(Client.System.GetMiscValue<int>("User.InitCoin"));
				Client.Item.Diamond.ChangeAmount(Client.System.GetMiscValue<int>("User.InitDiamond"));
#if UNITY_EDITOR
				Client.Item.Diamond.ChangeAmount(2000000);
#endif
			}

			if (!string.IsNullOrEmpty(s)) {
				var user = JsonMapper.ToObject<UserInfo>(s);
				if (user != null) {
					this.UserInfo.Chapter = user.Chapter;
					this.UserInfo.ChoosedHeroID = user.ChoosedHeroID;
					this.UserInfo.ChoosedBikeID = user.ChoosedBikeID;
					this.UserInfo.ChoosedWeaponID = user.ChoosedWeaponID;
					this.UserInfo.EquipedPropList = user.EquipedPropList;
				}
			}
		}

		public override void ResetInfo() {
			this.UserInfo = new UserInfo();
		}

		public override string ToJson(UserInfo user) {
			var ret = JsonMapper.ToJson(user);
			return ret;
		}

		public string InitNickName {
			get { return Client.System.GetMiscValue<string>("User.InitNickname"); }
		}

		public string InitNickNameInSdk {
			get {
				if (SDKManager.Instance.IsSupport("sdkNickname")) {
					var sdkNickname = SDKManager.Instance.GetSdkNickname();
					if (!string.IsNullOrEmpty(sdkNickname)) {
						return sdkNickname;
					}
				}
				return InitNickName;
			}
		}

		public string InitAvatar {
			get { return Client.System.GetMiscValue<string>("User.InitAvatar"); }
		}
		/// <summary>
		/// Gets the avatar count.
		/// </summary>
		/// <value>The avatar count.</value>
		public int AvatarCount {
			get { return Client.System.GetMiscValue<int>("User.AvatarCount"); }
		}
		/// <summary>
		/// Gets the random avatar.
		/// </summary>
		/// <value>The random avatar.</value>
		public string RandomAvatar {
			get {
				return (Random.Range(0, AvatarCount) + 1).ToString();
			}
		}
	}
}