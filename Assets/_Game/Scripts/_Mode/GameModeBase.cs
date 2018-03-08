using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using GameClient;
using GameUI;
using Joystick;
using Orbbec;
using RoomBasedClient;
using XPlugin.Data.Json;

namespace Game {
	public enum GameState {
		None = -1,
		Prepare = 0,
		Gaming = 1,
		Pause = 2,
		Finish = 3
	}

	public class GameModeBase<T> : GameModeBase where T : GameModeBase<T> {
		protected static T _instance;

		public new static T Ins {
			get { return _instance; }
		}

		protected override void Awake() {
			base.Awake();
			_instance = (T)this;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			_instance = null;
		}
	}

	public class GameModeBase : MonoBehaviour {
		public static GameModeBase Ins { get; private set; }

		public GameState State = GameState.None;
		public MatchInfo Match { get; private set; }
		public List<LevelTaskInfo> Tasks = new List<LevelTaskInfo>();

		public Action<PlayerInfo> OnGetReward = delegate { };

		public bool IsAllowPause = true;

		void OnApplicationPause(bool pauseStatus) {
			if (!IsAllowPause) return;
			if (Client.Game.MatchInfo.MatchMode == MatchMode.Guide || State != GameState.Gaming) {
				return;
			}

			if (pauseStatus) {//离开游戏到桌面时
				Pause();
				PauseDialog.Show();
			}
		}

		protected virtual void Awake() {
			Ins = this;
		}
		protected virtual void OnDestroy() {
			Ins = null;
		}

		public virtual void GetGameStarTaskChecker() {
		}

		public static RaceMode GetRaceMode(GameMode mode) {
			switch (mode) {
				case GameMode.None:
					return RaceMode.None;
				case GameMode.Racing:
					return RaceMode.Racing;
				case GameMode.Elimination:
					return RaceMode.Elimination;
				case GameMode.Timing:
					return RaceMode.Timing;
				case GameMode.RacingProp:
					return RaceMode.Racing;
				case GameMode.EliminationProp:
					return RaceMode.Elimination;
				default:
					throw new Exception("Game mode " + mode + " not exist");
			}
		}

		/// <summary>;
		/// 获取胜负结果
		/// </summary>
		/// <returns></returns>
		public bool GetWinOrFail() {
			// 按完成第一任务情况区分胜负
			switch (Match.MatchMode) {
				case MatchMode.Online:
					return BikeManager.Ins.CurrentBike.racerInfo.Rank <= 3;
				case MatchMode.Normal:
					return Tasks[0].State == TaskState.Completed;
				case MatchMode.Guide:
				case MatchMode.Championship:
					return true;
				default:
					return true;
			}
		}

		protected void PreparePlayerBike(PlayerInfo player) {
			// 创建玩家
			var playerBike = BikeManager.Ins.CurrentBike = BikeManager.Ins.CreateBike(player, true);
			BikeManager.Ins.SetBikeAsPlayer(playerBike);
			BikeManager.Ins.CurrentBike.racerInfo.OnReset += racer => {
				// 重置时立即复位相机
				BikeCamera.Ins.SetTarget(playerBike.bikeControl);
			};
		}

		protected void PrepareEnemy(MatchInfo match) {
			// 创建当前关卡的敌人
			var enemys = match.Enemys;
			if (match.MatchMode == MatchMode.Online) {
				var room = Lobby.Ins.RoomClient.ToRoomClient.Room;
				if (room.MinePlayer.IsMaster) {//由主机生成机器人
					if (room.OtherCustomProperties.ContainsKey(Lobby.RobotsKey)) {
						enemys = (List<PlayerInfo>)Lobby.Ins.RoomClient.ToRoomClient.Room.OtherCustomProperties[Lobby.RobotsKey];
					} else {
						enemys = new List<PlayerInfo>();
					}
				}
			}
			for (var i = 0; i < enemys.Count; i++) {
				var info = enemys[i];
				if (string.IsNullOrEmpty(info.NickName)) {
					info.NickName = Client.Nickname.Lib.Random;
				}
				var enemy = BikeManager.Ins.CreateBike(info, false);
			}
			if (match.MatchMode == MatchMode.Guide) {
				for (var i = 0; i < BikeManager.Ins.Bikes.Count; i++) {
					var racerInfo = BikeManager.Ins.Bikes[i];//TODO 应该把引导的东西放到引导里去
					if (racerInfo.CompareTag(Tags.Ins.Enemy)) {
						Transform targetPos = GameObject.Find("ai_" + i).transform; // TODO no Object.Find
						racerInfo.transform.position = targetPos.position;
						racerInfo.transform.rotation = targetPos.rotation;
						racerInfo.bikeControl.ActiveControl = false;
					}
				}
			}
		}

		public virtual void PrepareBike(MatchInfo match, PlayerInfo player) {
			PreparePlayerBike(player);
			PrepareEnemy(match);
			BikeManager.Ins.SetAllBikeActive(false);
			RaceManager.Ins.OnMatchStart += () => {
				BikeManager.Ins.SetAllBikeActive(true);
			};
		}


		private bool IsPauseWhenLostTrack = false;
		public virtual void PrepareGame(MatchInfo matchInfo, PlayerInfo player) {
			AnalyticsMgrBase.Ins.GameStart(matchInfo.GetMathSymbol());

			State = GameState.Prepare;
			Match = matchInfo;

			RoomClient.OfflineMode = matchInfo.MatchMode != MatchMode.Online;
			int playerCount = Match.Enemys.Count + 1;
			if (!RoomClient.OfflineMode) {
				playerCount = Lobby.Ins.Robots.Count + Lobby.Ins.RoomClient.ToRoomClient.Room.PlayerCount;
			}
			//准备赛道
			RaceManager.Ins.PrepareRace(turn: Match.Data.Turn,
				playerNum: playerCount,
				raceLine: Match.Data.RaceLine,
				ObjLine: Match.Data.ObjLine,
				mode: GetRaceMode(matchInfo.Data.GameMode));
			InitLevelTask();
			CountDownCamera.Ins.Init();
			//进入等待和场景展示
			RaceManager.Ins.StartWaiting();

			if (!RoomClient.OfflineMode) {//通知加载完成
				Lobby.Ins.SetState(Lobby.PlayerState.Loaded);
			}

			if (FocusManager.TVMode && BikeOrbecController.HasOrbbecDevice()) {//如果有体感设备
				this.DelayInvoke(() => {
					BikeOrbecController.Ins.Init();
					OrbbecSensingManager.instance.trackedAction = () => {
						HelpOnClick.PreserveHelpClick = false;//恢复教程
						if (RaceManager.Ins.GamePhase == GamePhase.Waiting) {
							if (!RoomClient.OfflineMode) {
								Debug.Log("Set Im Ready".Colored(LogColors.blue));
								Lobby.Ins.SetState(Lobby.PlayerState.ReadyGame);
							} else {
								this.firstTracked = true;
							}
						} else {//在游戏中标定完成后从暂停恢复
							if (!this.IsPauseWhenLostTrack) {
								Resume();
							}
						}
					};
					OrbbecSensingManager.instance.unTrackedAction = () => {
						if (RoomClient.OfflineMode) {//单机模式丢失标定，暂停游戏
							HelpOnClick.PreserveHelpClick = true;//暂停教程
							this.IsPauseWhenLostTrack = Time.timeScale == 0;
							if (RaceManager.Ins.GamePhase != GamePhase.Waiting && RaceManager.Ins.GamePhase != GamePhase.Over) {
								Pause();
							}
						}
					};
					RaceManager.Ins.OnGameFinish += () => {//游戏结束后不再显示标定界面
						if (OrbbecSensingManager.instance != null) {
							OrbbecSensingManager.instance.showTrackingUI = false;
						}
					};
				}, 0.75f);
			}


			Action OnLoaded = () => {
				PrepareBike(matchInfo, player);
				if (RoomClient.OfflineMode) {
					//如果有体感设备,等待标定完成
					if (FocusManager.TVMode && BikeOrbecController.HasOrbbecDevice()) {
						float cd = matchInfo.MatchMode == MatchMode.OnlineRandom ? 25 : -1;
						StartCoroutine(WaitForTracked(cd));
					} else {//展示一次后直接开始
						CountDownCamera.Ins.OnShowMovieFinish += () => {
							RaceManager.Ins.StartCountDown();
						};
					}
				} else {
					if (!FocusManager.TVMode || !BikeOrbecController.HasOrbbecDevice()) {
						Lobby.Ins.SetState(Lobby.PlayerState.ReadyGame);
					}
					waitFormOnlineModeStartCoroutine = StartCoroutine(WaitForOnlineModeStart());
				}

				//倒计时到GO的时候比赛正式开始
				CountDownCamera.Ins.OnCountDownGo += () => {
					BikeCamera.Ins.SetTarget(BikeManager.Ins.CurrentBike.bikeControl); //移交相机控制
					RaceManager.Ins.StartMatch();
					GamePlayBoard.Show();
					Client.EventMgr.SendEvent(EventEnum.Game_CountDownFinish);
				};
			};

			if (matchInfo.MatchMode == MatchMode.Online) {
				StartCoroutine(WaitForOnlineModeAllLoaded(OnLoaded));
			} else {
				OnLoaded();
			}
		}

		IEnumerator WaitForOnlineModeAllLoaded(Action onDone) {
			float waitingTime = 20f;
			WaittingTip.Show("等待玩家加载... " + waitingTime);

			while (waitingTime > 0) {
				yield return null;
				waitingTime -= Time.deltaTime;
				WaittingTip.Update("等待玩家加载... " + (int)waitingTime);
				bool allLoaded = true;
				foreach (var player in Lobby.Ins.RoomClient.ToRoomClient.Room.Players.Values) {
					if (player.CustomProperties[Lobby.PlayerStateKey] == null || (Lobby.PlayerState)player.CustomProperties[Lobby.PlayerStateKey] < Lobby.PlayerState.Loaded) {
						allLoaded = false;
						break;
					}
				}
				if (allLoaded) {
					Debug.Log("All Loaded ".Colored(LogColors.aqua));
					waitingTime = 0;
				}
			}
			WaittingTip.Hide();
			onDone();
		}

		private bool waitForMasterCallStart = false;
		private Coroutine waitFormOnlineModeStartCoroutine;
		IEnumerator WaitForOnlineModeStart() {
			this.waitForMasterCallStart = true;
			float waitingTime = 25f;
			WaittingTip.Show("等待玩家准备... " + waitingTime);
			while (waitingTime > 0) {
				yield return null;
				waitingTime -= Time.deltaTime;
				WaittingTip.Update("等待玩家准备... " + (int)waitingTime);
				bool allReady = true;
				foreach (var player in Lobby.Ins.RoomClient.ToRoomClient.Room.Players.Values) {
					if (player.CustomProperties[Lobby.PlayerStateKey] == null || (Lobby.PlayerState)player.CustomProperties[Lobby.PlayerStateKey] < Lobby.PlayerState.ReadyGame) {
						allReady = false;
						break;
					}
				}
				if (allReady) {
					Debug.Log("All Ready ".Colored(LogColors.aqua));
					waitingTime = 0;
				}
			}
			WaittingTip.Update("即将开始... ");
			while (this.waitForMasterCallStart) {
				if (Lobby.Ins.RoomClient.ToRoomClient.Room.MinePlayer.IsMaster) {//主机通知所有玩家开始
					Lobby.Ins.StartOnlineModeCountdown();
					break;
				}
				yield return null;
			}
		}

		public void StartOnlineMode() {
			if (waitFormOnlineModeStartCoroutine != null) {
				StopCoroutine(this.waitFormOnlineModeStartCoroutine);
			}
			WaittingTip.Hide();
			RaceManager.Ins.StartCountDown();
			Lobby.Ins.SetState(Lobby.PlayerState.Gaming);
		}

		private bool firstTracked = false;
		IEnumerator WaitForTracked(float cd) {
			if (cd <= 0) {
				while (true) {
					if (this.firstTracked) {
						break;
					}
					yield return null;
				}
			} else {
				WaittingTip.Show("等待玩家准备... " + cd);
				while (cd > 0) {
					yield return null;
					cd -= Time.deltaTime;
					WaittingTip.Update("等待玩家准备... " + (int)cd);
					if (this.firstTracked) {
						cd = 0;
					}
				}
				WaittingTip.Hide();
			}
			RaceManager.Ins.StartCountDown();
		}
		public virtual void Pause() {
			if (Match.MatchMode != MatchMode.Online && Match.MatchMode != MatchMode.OnlineRandom) {
				Time.timeScale = 0;
			}
			SfxManager.Ins.SetPause(true);
			State = GameState.Pause;
		}

		public virtual void Resume() {
			Time.timeScale = 1;
			SfxManager.Ins.SetPause(false);
			State = GameState.Gaming;
		}

		public virtual void InitLevelTask() {
			// 创建任务
			Client.Task.LevelStatValue = new JObject();
			Tasks = new List<LevelTaskInfo>();
			if (Match.Data.LevelTasks != null) {//联网模式下没有任务信息
				foreach (var data in Match.Data.LevelTasks) {
					var task = new LevelTaskInfo(data);
					task.Enable();
					Tasks.Add(task);
				}
			}
		}

		public int GetTaskReward(int index) {
			int amount = 0;
			for (int i = 0; i < Tasks.Count; i++) {
				if (Tasks[i].State == TaskState.Completed) {
					amount += Tasks[i].Data.RewardList[index].Amount;
				}
			}
			return amount;
		}

		public virtual void FinishGame() {
			State = GameState.Finish;
			if (RaceManager.Ins.GamePhase == GamePhase.Over) {
				return;
			}

			RaceManager.Ins.FinishGame();

			// 结算数据处理
			var info = BikeManager.Ins.CurrentBike.info;
			OnGetReward(info);

			// 计时赛统计剩余时间，其他模式为完成时间
			var runTime = RaceManager.Ins.RaceMode == RaceMode.Timing
				? (int)GameModeTiming.Ins.TimeLeft
				: (int)BikeManager.Ins.CurrentBike.racerInfo.RunTime;

			// 发送结束事件
			if (Match.MatchMode != MatchMode.Guide) {
				Client.EventMgr.SendEvent(
					EventEnum.Game_End,
					Match.Data.GameMode,
					Match.Data.ID,
					Match.MatchMode == MatchMode.Online || Match.MatchMode == MatchMode.OnlineRandom,
					Match.MatchMode == MatchMode.Championship,
					Client.User.UserInfo.ChoosedHeroID,
					Client.User.UserInfo.ChoosedBikeID,
					BikeManager.Ins.CurrentBike.racerInfo.Rank,
					runTime,
					BikeManager.Ins.CurrentBike.bikeHealth.CrashCount,
					BikeManager.Ins.CurrentBike.racerInfo.IsCompleteGame,
					Match.MatchMode
					);
			}

			// 记录比赛结果
			switch (Match.MatchMode) {
				case MatchMode.Championship: //锦标赛
					var racerInfo = BikeManager.Ins.CurrentBike.racerInfo;
					ChampionshipInfo championshipInfoinfo = Client.Championship.GetCurrentChampionshipInfo();
					if (racerInfo.RunTime < championshipInfoinfo.Data.TimeLimit) {
						championshipInfoinfo.TaskResults[2] = true;
					}
					if (racerInfo.Rank == 1) {
						championshipInfoinfo.TaskResults[1] = true;
					}
					if (racerInfo.IsCompleteGame) {
						championshipInfoinfo.TaskResults[0] = true;
					}
					championshipInfoinfo.BestTime = racerInfo.RunTime;
					Client.Championship.SaveData();
					Client.Championship.UploadResult(championshipInfoinfo.BestTime); //上传赛事成绩
					break;
				case MatchMode.Normal: //闯关模式
					var taskStar = 0;
					var taskResults = Client.Match.GetMatchInfo(Match.Data.ID).TaskResults;
					for (var i = 0; i < Tasks.Count; i++) {
						this.Tasks[i].CheckState();
						var result = this.Tasks[i].State == TaskState.Completed;
						//发放奖励
						if (result) {
							Client.Item.GainItem(Client.Item.CoinData, Tasks[i].Data.RewardList[0].Amount, false);
						}
						// 记录新完成的任务
						if (!taskResults[i]) {
							taskResults[i] = result;
							if (result) taskStar++;
						}
					}
					if (taskStar > 0) {
						Client.EventMgr.SendEvent(EventEnum.Game_Star, taskStar);
					}
					// 保存
					Client.Match.SaveData();
					break;
				case MatchMode.Challenge:
					//发放奖励
					for (int i = 0; i < Tasks.Count; i++) {
						this.Tasks[i].CheckState();
						if (this.Tasks[i].State == TaskState.Completed) {
							foreach (var reward in Tasks[i].Data.RewardList) {
								Client.Item.GainItem(reward.Data, reward.Amount, false);
							}
						}
					}
					break;
			}

			if (Ins.GetWinOrFail()) {
				AnalyticsMgrBase.Ins.GameWin(Match.GetMathSymbol());
			} else {
				AnalyticsMgrBase.Ins.GameFail(Match.GetMathSymbol());
			}
			// 保存
			Client.User.UserInfo.SaveGameData();
			Client.Game.FinishGame();
		}

		/// <summary>
		/// 关闭关卡任务，必须在任务结算完成后调用一次
		/// </summary>
		public void CloseLevelTasks() {
			// 关闭任务
			Client.Task.ResetLevelTask();
			foreach (var task in Tasks) {
				task.Disable();
			}
			Tasks.Clear();
		}

		private void OnDisable() {
			CloseLevelTasks();
		}

		public static GameModeBase InitGameModeIns(GameMode mode) {
			var raceMode = GetRaceMode(mode);
			switch (raceMode) {
				case RaceMode.Racing:
					return new GameObject("__GameMode").AddComponent<GameModeRacing>();
				case RaceMode.Elimination:
					return new GameObject("__GameMode").AddComponent<GameModeElimination>();
				case RaceMode.Timing:
					return new GameObject("__GameMode").AddComponent<GameModeTiming>();
				default:
					throw new Exception("Game mode " + mode + "  not exist.");
			}
		}
	}
}
