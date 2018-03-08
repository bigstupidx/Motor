using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using XPlugin.Data.Json;

namespace GameClient {
	public class ModOnline : ClientModule {

		public int OnlinePlayerNum { get; private set; }

		public Dictionary<GameMode, List<OnlineReward>> rewardList = new Dictionary<GameMode, List<OnlineReward>>() {
			{GameMode.Racing, new List<OnlineReward>(){new OnlineReward() { ID = 1001,Amount = 100} } },
			{GameMode.RacingProp, new List<OnlineReward>(){new OnlineReward() { ID = 1001, Amount = 100} } },
			{GameMode.Elimination, new List<OnlineReward>(){new OnlineReward() { ID = 1001, Amount = 100} } },
			{GameMode.EliminationProp, new List<OnlineReward>(){new OnlineReward() { ID = 1001, Amount = 100} } },
		};//TODO 数据库读取

		private List<GameMode> NetMatchModes = new List<GameMode>(){
			GameMode.Racing,GameMode.RacingProp,GameMode.Elimination,GameMode.EliminationProp
		};

		public Action OnNetworkDisable;

		/// <summary>
		/// 更新在线人数
		/// </summary>
		private void UpdateOnlineInfo() {
			var delta = Random.Range(-5, 5);
			this.OnlinePlayerNum += delta;
			this.OnlinePlayerNum = Mathf.Clamp(this.OnlinePlayerNum, 0, 99999);
		}

		/// <summary>
		/// 更新网络状态
		/// </summary>
		public bool CheckNetwork() {
			bool ret = Application.internetReachability != NetworkReachability.NotReachable;
			if (!ret) {
				if (this.OnNetworkDisable != null) {
					this.OnNetworkDisable();
				}
			}
			return ret;
		}

		/// <summary>
		/// 开始在线模式
		/// </summary>
		public void StartOnline() {
			if (this.OnlinePlayerNum < 1) {
				this.OnlinePlayerNum = Random.Range(100, 300);
			}
			this._inOnlineMode = true;
		}

		private bool _inOnlineMode = false;
		private float _timer;
		private const float RefreshTime = 5;

		protected override void Update() {
			base.Update();
			if (this._inOnlineMode) {
				this._timer += Time.unscaledDeltaTime;
				if (this._timer >= RefreshTime) {
					CheckNetwork();
					UpdateOnlineInfo();
					this._timer = 0;
				}
			}
		}

		/// <summary>
		/// 结束在线模式
		/// </summary>
		public void StopOnline() {
			this._inOnlineMode = false;
		}

		public int GetAlreadyInPlayerCount() {
			return Random.Range(0, this.PlayerNum - 1);
		}

		public List<GameMode> GetMatchMode() {
			return NetMatchModes;
		}

		/// <summary>
		/// 获取比赛信息
		/// </summary>
		/// <param name="mode"></param>
		/// <returns></returns>
		public MatchInfo GetMatchInfo(GameMode mode, MatchMode matchMode) {
			var sceneData = Client.Scene.SceneDatas.Random();
			string objLine = null;
			switch (mode) {
				case GameMode.Racing:
				case GameMode.Elimination:
					objLine = sceneData.RandomRaceObjLine;
					break;
				case GameMode.EliminationProp:
				case GameMode.RacingProp:
					objLine = sceneData.RandomPropObjLine;
					break;
				case GameMode.Timing:
					objLine = sceneData.RandomTimingObjLine;
					break;
			}
			var data = new MatchData {
				ID = -1,
				GameMode = mode,
				Scene = sceneData,
				RaceLine = sceneData.RandomRaceLine,
				Turn = Random.Range(1, 3),
				ObjLine = objLine
			};
			List<PlayerInfo> list = new List<PlayerInfo>();
			if (mode != GameMode.Timing) {
				var num = this.PlayerNum;
				for (var i = 0; i < num - 1; i++) {
					list.Add(GetRandomPlayerInfo(i));
				}
			}
			var info = new MatchInfo(data, list, matchMode);
			return info;
		}

		/// <summary>
		/// 获取随机玩家信息
		/// </summary>
		/// <returns></returns>
		public PlayerInfo GetRandomPlayerInfo(int spawnPos) {
			string nickName = Client.Nickname.Lib.Random;
			//string avatar = Random.value < 0.2 ? Client.User.InitAvatar : Client.User.RandomAvatar;
			var player = new PlayerInfo {
				NickName = nickName,
				//Avatar = avatar,
				ChoosedHero = GetRandomHeroInfo(),
				ChoosedBike = GetRandomBikeInfo(),
				AI = PlayerInfo.RandomAILevel(),
				ChoosedWeapon = GetRandomWeaponInfo(),
				EquipedProps = new List<PropInfo>(){
					new PropInfo(Client.Prop.Random())
				},
				SpawnPos = spawnPos

			};
			player.EquipedProps[0].ChangeAmountWithoutSave(Random.Range(0, 5));
			return player;
		}

		/// <summary>
		/// 获取随机英雄信息
		/// </summary>
		/// <returns></returns>
		public HeroInfo GetRandomHeroInfo() {
			return new HeroInfo(Client.Hero.Random(), Random.Range(1, Client.Hero.MaxLevel));
		}

		/// <summary>
		/// 获取随机车辆信息
		/// </summary>
		/// <returns></returns>
		private BikeInfo GetRandomBikeInfo() {
			var bikeData = Client.Bike.Random();
			var levelMax = Client.Bike.LevelMax;
			return new BikeInfo(
				bikeData,
				Random.Range(1, levelMax),
				Random.Range(1, levelMax),
				Random.Range(1, levelMax),
				Random.Range(1, levelMax)
				);
		}

		private WeaponInfo GetRandomWeaponInfo() {
			var data = Client.Weapon.Random();
			return new WeaponInfo(data, 1);
		}


		public int PlayerNum {
			//			get { return Client.System.GetMiscValue<int>("Online.PlayerNum"); }
			get { return 6; }
		}

		public List<OnlineReward> GetReward(GameMode mode) {
			if (rewardList != null && rewardList.ContainsKey(mode)) {
				return rewardList[mode];
			}
			return null;
		}

	}
}