using System;
using System.Collections;
using Game;
using GameUI;
using Joystick;
using Orbbec;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameClient {
	/// <summary>
	/// 游戏模式
	/// </summary>
	public enum GameMode {
		/// <summary>
		/// 无
		/// </summary>
		None = 0,
		/// <summary>
		/// 竞速赛
		/// </summary>
		Racing = 1,
		/// <summary>
		/// 淘汰赛
		/// </summary>
		Elimination = 2,
		/// <summary>
		/// 计时赛
		/// </summary>
		Timing = 3,
		/// <summary>
		/// 竞速道具赛
		/// </summary>
		RacingProp = 4,
		/// <summary>
		/// 淘汰道具赛
		/// </summary>
		EliminationProp = 5,
	}

	/// <summary>
	/// 游戏操控模式
	/// </summary>
	public enum GameControlMode {
		/// <summary>
		/// 按键操控
		/// </summary>
		Btn = 0,

		/// <summary>
		/// 滑动
		/// </summary>
		//		Swipe,

		/// <summary>
		/// 重力感应+滑动
		/// </summary>
		GravitySwipe = 1,

		/// <summary>
		/// 遥控
		/// </summary>
		RemoteControl = 2,

		/// <summary>
		/// 体感
		/// </summary>
		Somatosensory = 3
	}

	public enum GameQuality {
		High,
		Low
	}

	public class ModGame : ClientModule {
		public GameMode CurrentGameMode = GameClient.GameMode.None;

		/// <summary>
		/// 当前比赛信息
		/// </summary>
		public MatchInfo MatchInfo;

		public bool IsGaming {
			get { return RaceManager.Ins != null; }
		}

		/// <summary>
		/// 开始游戏
		/// </summary>
		public void StartGame(MatchInfo info, PlayerInfo player = null) {
			// 设置比赛信息
			MatchInfo = info;
			StartCoroutine(LoadSceneCoroutine(MatchInfo.Data.Scene.SceneName,
				() => {
					if (player == null) {
						Client.User.ResetChoosedPlayerInfo();
						player = Client.User.ChoosedPlayerInfo;
						//消耗武器
						player.ChoosedWeapon.ChangeAmount(-1);
					}
					var gameMode = GameModeBase.InitGameModeIns(MatchInfo.Data.GameMode);
					gameMode.PrepareGame(MatchInfo, player);
				}));
		}

		IEnumerator LoadSceneCoroutine(string sceneName, Action onFinish) {
			// 显示Load UI
			LoadingBoard.Show((LString.GAMECLIENT_MODGAME).ToLocalized(), 0.1f, true);
			var _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
			yield return null;
				LoadingBoard.Ins.SetLoadingBarValue(0.2f);
			yield return null;
			while (!_asyncOperation.isDone) {
				yield return _asyncOperation;
				LoadingBoard.Ins.SetLoadingBarValue(_asyncOperation.progress+0.2f);
			}
			LoadingBoard.Ins.SetLoadingBarValue(1);
			onFinish();
		}

		/// <summary>
		/// 游戏结束（For UI）
		/// </summary>
		public void FinishGame() {
			var delayTime = 2f;
			if (GameModeBase.Ins.Match.MatchMode == MatchMode.Guide) {
				delayTime = 0;
			}
			StartCoroutine(DelayFinish(delayTime));
			Client.User.ResetChoosedPlayerInfo();
		}

		IEnumerator DelayFinish(float delayTime) {
			float musicVolume = Client.User.UserInfo.Setting.MusicVolume;
			for (int i = 0; i < delayTime / 0.1f; i++) {
				yield return new WaitForSecondsRealtime(0.1f);
				musicVolume = Mathf.MoveTowards(musicVolume, 0f, 0.05f);
				SfxManager.Ins.SetMusicVolume(musicVolume);
			}
			// 结束音效
			SfxManager.Ins.PlayOneShot(GameModeBase.Ins.GetWinOrFail() ? SfxType.SFX_Win : SfxType.SFX_Lose);
			BikeManager.Ins.CurrentBike.bikeSound.EngineAudio.Stop();//结束后玩家车辆引擎声关掉，太吵了
			BikeManager.Ins.CurrentBike.bikeSound.DriftAudio.Volume = 0f;
			BikeManager.Ins.CurrentBike.bikeSound.BoostStartAudio.Volume = 0f;
			BikeManager.Ins.CurrentBike.bikeSound.BoostStopAudio.Volume = 0f;
			if (MatchInfo.MatchMode == MatchMode.Online || this.MatchInfo.MatchMode == MatchMode.OnlineRandom) {
				GameOverBoard.Show();
			} else {
				//				if (this.CurrentGameMode == GameMode.Elimination || this.CurrentGameMode == GameMode.EliminationProp) {
				//				}
				GameOverBoard.Show();
				//				FinishMovieBoard.Show();
			}
		}

		public static GameControlMode GetControlMode() {
			if (FocusManager.TVMode) {
				if (BikeOrbecController.HasOrbbecDevice()) {
					return (GameControlMode.Somatosensory);
				} else {
					return (GameControlMode.RemoteControl);
				}
			} else {
				return (Client.User.UserInfo.Setting.ControlMode);
			}
		}

	}
}

