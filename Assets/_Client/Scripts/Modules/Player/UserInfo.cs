
using System.Collections.Generic;
using UnityEngine;
using XPlugin.Data.Json;

namespace GameClient {

	public class UserInfo {
		/// <summary>
		/// 设置
		/// </summary>
		[JsonIgnore]
		public SettingInfo Setting;

		/// <summary>
		/// 签到信息
		/// </summary>
		[JsonIgnore]
		public SignInfo Sign;

		/// <summary>
		/// 体感礼包信息
		/// </summary>
		[JsonIgnore]
		public SensorSpreeInfo SensorSpreeInfo;

		/// <summary>
		/// 礼包
		/// </summary>
		[JsonIgnore]
		public SpreeInfo Spree;

		/// <summary>
		/// 上场人物ID
		/// </summary>
		public int ChoosedHeroID {
			get {
				return choosedHeroID;
			}
			set {
				if (this.choosedHeroID != value) {
					this.choosedHeroID = value;
					Client.User.SaveData();
				}
			}
		}
		private int choosedHeroID;
		/// <summary>
		/// 上场车辆ID
		/// </summary>
		public int ChoosedBikeID {
			get {
				return choosedBikeID;
			}
			set {
				if (this.choosedBikeID != value) {
					this.choosedBikeID = value;
					Client.User.SaveData();
				}
			}
		}
		private int choosedBikeID;

		/// <summary>
		/// 装备的道具ID
		/// </summary>
		public List<int> EquipedPropList = new List<int>();

		/// <summary>
		/// 选择的武器
		/// </summary>
		public int ChoosedWeaponID {
			get {
				return choosedWeaponID;
			}
			set {
				if (this.choosedWeaponID != value) {
					this.choosedWeaponID = value;
					Client.User.SaveData();
				}
			}
		}
		private int choosedWeaponID;


		private int _chapter = 0;
		/// <summary>
		/// 章节进度
		/// </summary>
		public int Chapter {
			get {
				return _chapter;
			}

			set {
				if (this._chapter != value) {
					this._chapter = value;
					Client.User.SaveData();
				}
			}
		}

		/// <summary>
		/// 章节信息（内有关卡信息）
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, ChapterInfo> ChapterInfoList = new Dictionary<int, ChapterInfo>();

		/// <summary>
		/// 赛事信息
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, ChampionshipInfo> ChampionshipInfoList = new Dictionary<int, ChampionshipInfo>();

		/// <summary>
		/// 挑战赛事信息
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, ChallengeInfo> ChallengeInfoList = new Dictionary<int, ChallengeInfo>();

		/// <summary>
		/// 体力
		/// </summary>
		[JsonIgnore]
		public StaminaInfo Stamina;

		/// <summary>
		/// 拥有的人物
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, HeroInfo> HeroList = new Dictionary<int, HeroInfo>();

		/// <summary>
		/// 拥有的车辆
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, BikeInfo> BikeList = new Dictionary<int, BikeInfo>();

		/// <summary>
		/// 拥有的道具
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, PropInfo> PropList = new Dictionary<int, PropInfo>();

		/// <summary>
		/// 拥有的武器
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, WeaponInfo> WeaponList = new Dictionary<int, WeaponInfo>();

		/// <summary>
		/// 已经拥有的物品(除道具,武器，车辆，人物)
		/// </summary>
		[JsonIgnore]
		public Dictionary<int, ItemInfo> OwnedItems = new Dictionary<int, ItemInfo>();

		/// <summary>
		/// 统计数值
		/// </summary>
		[JsonIgnore]
		public JObject DailyStatValue {
			set {
				_dailyStatValue = value;
			}
			get {
				return Client.Game.IsGaming ? Client.User.ChoosedPlayerInfo.DailyStatValue : _dailyStatValue;
			}
		}
		[JsonIgnore]
		private JObject _dailyStatValue = new JObject();

		[JsonIgnore]
		public JObject AchieveStatValue {
			set {
				_achieveStatValue = value;
			}
			get {
				return Client.Game.IsGaming ? Client.User.ChoosedPlayerInfo.AchieveStatValue : _achieveStatValue;
			}
		}
		[JsonIgnore]
		private JObject _achieveStatValue = new JObject();

		[JsonIgnore]
		public List<int> DailyRewardedList = new List<int>();
		[JsonIgnore]
		public List<int> AchieveRewardedList = new List<int>();
		/// <summary>
		/// 进行中的每日任务ID
		/// </summary>
		[JsonIgnore]
		public List<int> DailyTasks = new List<int>();

		[JsonIgnore]
		public long LastResetTaskTime;
		[JsonIgnore]
		public long LastResetChallengeTime;

		#region Function
		/// <summary>
		/// 设置游戏内用的临时数据
		/// </summary>
		public void SetGameData() {
			// 将任务数据拷贝到游戏内临时数据
			Client.User.ChoosedPlayerInfo.DailyStatValue = JObject.Parse(_dailyStatValue.ToString());
			Client.User.ChoosedPlayerInfo.AchieveStatValue = JObject.Parse(_achieveStatValue.ToString());
		}

		/// <summary>
		/// 保存游戏内临时存储的任务进度数据
		/// </summary>
		public void SaveGameData() {
			// 存金币
			var count = Client.User.ChoosedPlayerInfo.GetStat<int>("PropType" + (int)PropType.Coin);
			Client.Item.Coin.ChangeAmount(count);
			// 游戏中的临时任务数据，写回到存储用任务进度数据
			_dailyStatValue = JObject.Parse(Client.User.ChoosedPlayerInfo.DailyStatValue.ToString());
			_achieveStatValue = JObject.Parse(Client.User.ChoosedPlayerInfo.AchieveStatValue.ToString());
			Client.Task.SaveWhenGameFinish();
		}

		/// <summary>
		/// 清除每日数据
		/// </summary>
		public void ClearDaily() {
			DailyStatValue = new JObject();
			DailyRewardedList = new List<int>();
			DailyTasks = new List<int>();
		}
		#endregion
	}

}

