using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using GameUI;
using Joystick;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameClient {
	public class StoryGuide_1_Game : StoryGuideBase {
		public bool GoNext { get; set; }
		public static StoryGuide_1_Game Ins { get; private set; }

		private GameObject Rank;
		private GameObject Map;
		private GameObject Pause;
		private GameObject Bar;


		MaskableGraphic TurnLeft;
		MaskableGraphic TurnRight;
		MaskableGraphic DriftLeft;
		MaskableGraphic DriftRight;
		MaskableGraphic BoostImage;
		MaskableGraphic PropImage;

		public override void Enable() {
			base.Enable();
			Ins = this;
			Client.EventMgr.AddListener(EventEnum.UI_MainMenuBoard_Inited, OnEvent);
		}

		public override void Disable() {
			base.Disable();
			Ins = null;
			Client.EventMgr.RemoveListener(EventEnum.UI_MainMenuBoard_Inited, OnEvent);
		}

		public override void OnEvent(EventEnum eventType, params object[] args) {
			base.OnEvent(eventType, args);
			var id = int.Parse(GetType().Name.Split('_')[1]);
			Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
			Client.Story.StartCoroutine(StartGuide());
		}

		public IEnumerator StartGuide() {
			GameControlMode mode = ModGame.GetControlMode();


			string continueTip = "继续";
			switch (mode) {
				case GameControlMode.Btn:
				case GameControlMode.GravitySwipe:
					continueTip = "点击屏幕继续";
					break;
				case GameControlMode.RemoteControl:
					continueTip = "点击遥控器OK键继续";
					break;
				case GameControlMode.Somatosensory:
					continueTip = "点击遥控器OK键继续";
					break;
			}

			Client.Guide.Log("开始教程关");
			MatchData matchData = new MatchData() {
				ID = -1,
				Name = (LString.GAMECLIENT_MATCHINFO_GETMATCHNAME).ToLocalized(),
				GameMode = GameMode.Racing,
				RaceLine = "guide",
				ObjLine = "guide",
				Turn = 1,
				Scene = Client.Scene[2],
			};
			List<PlayerInfo> enemies = new List<PlayerInfo>();
			BikeInfo bike = new BikeInfo(Client.Bike[4006], 0, 0, 0, 0);
			WeaponInfo weapon = new WeaponInfo(Client.Weapon[5001], 1);
			HeroInfo hero = new HeroInfo(Client.Hero[3001], 10);
			List<PropInfo> prop = new List<PropInfo>() { new PropInfo(Client.Prop[6001], 10) };
			PropInfo missile = Client.Prop.GetPropInfo(6001);
			missile.ChangeAmount(-missile.Amount);//清空导弹数据
			missile.ChangeAmount(5);//初始给5个导弹
			Client.User.UserInfo.EquipedPropList = new List<int>() { 6001 };//初始装备导弹
			PlayerInfo playerInfo = new PlayerInfo() {
				AI = 5,
				ChoosedBike = bike,
				ChoosedHero = hero,
				ChoosedWeapon = weapon,
				EquipedProps = prop
			};

			enemies.Add(new PlayerInfo() {
				AI = 5,
				ChoosedBike = new BikeInfo(Client.Bike[4005], 0, 0, 0, 0),
				ChoosedHero = new HeroInfo(Client.Hero[3002], 10),
				ChoosedWeapon = weapon,
				EquipedProps = new List<PropInfo>(),
			});
			enemies.Add(new PlayerInfo() {
				AI = 5,
				ChoosedBike = new BikeInfo(Client.Bike[4004], 0, 0, 0, 0),
				ChoosedHero = new HeroInfo(Client.Hero[3005], 10),
				ChoosedWeapon = weapon,
				EquipedProps = new List<PropInfo>()
			});

			MatchInfo matchInfo = new MatchInfo(matchData, enemies, MatchMode.Guide);
			Client.EventMgr.AddListener(EventEnum.UI_EnterMenu, OnGamePlayBoardEnter);
			Client.Game.StartGame(matchInfo, playerInfo);
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}
			Debug.Log("车辆准备好，改变AI位置");
			for (var i = 0; i < BikeManager.Ins.Bikes.Count; i++) {
				var racerInfo = BikeManager.Ins.Bikes[i];
				if (racerInfo.CompareTag(Tags.Ins.Enemy)) {
					Transform targetPos = GameObject.Find("ai_" + i).transform;
					racerInfo.transform.position = targetPos.position;
					racerInfo.transform.rotation = targetPos.rotation;
					racerInfo.bikeControl.ActiveControl = false;
				}
			}

			//等待通过GO
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}

			//显示引导欢迎

			HelpOnClick.CanPreseveHelpClick = true;
			Time.timeScale = 0;
			Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE).ToLocalized(), continueTip);
			Guide.ActiveBlackBack(true);
			ClearScreenClick(null, () => {
				this.GoNext = true;
			});
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}

			ClearScreenClick();
			Guide.Close();
			Time.timeScale = 1;

			GameObject target = null;

			//等待到达第一个转弯
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}

			Debug.Log("开始转弯引导");
			Client.EventMgr.SendEvent(EventEnum.Guide_Operation, new object[] { "1", "转弯引导" });
			Time.timeScale = 0;
			//关闭引导上的点击
			Guide.HelpOnClickFinger.GetComponent<MaskableGraphic>().raycastTarget = false;
			Guide.HelpOnClickScreen.GetComponent<MaskableGraphic>().raycastTarget = false;
			//TODO 恶心的swich。考虑多态
			switch (mode) {
				case GameControlMode.Btn:
				case GameControlMode.GravitySwipe:
					Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_1).ToLocalized());
					Guide.ActiveFinger(true);
					Guide.ActiveDarkHole(true);
					Guide.SetFingerPos(TurnLeft.transform);

					TurnLeft.gameObject.SetActive(true);
					TurnRight.gameObject.SetActive(true);
					Component tmpCom = TurnLeft.gameObject.AddComponent<StoryGuide_1_Game_PointerDown>();

					//等待用户操作
					this.GoNext = false;
					while (!this.GoNext) {
						yield return null;
					}
					Object.Destroy(tmpCom);//删除刚添加的组建
					break;
				case GameControlMode.RemoteControl:
					Guide.SetWordPos(null, "前方有小弯道，按住 ← 就可以通过。");
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					yield return new WaitForSecondsRealtime(0.3f);//强制等一会会，避免一闪而过
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext = Input.GetKeyDown(KeyCode.LeftArrow);
						yield return null;
					}
					break;
				case GameControlMode.Somatosensory:
					Guide.SetWordPos(null, "前方有小弯道，向左倾斜身体就可以通过。", null, null);//TODO 添加示意图
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					yield return new WaitForSecondsRealtime(0.6f);//强制等一会会，避免一闪而过
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext =
							Orbbec.OrbbecSensingManager.instance.horizontalValue * BikeInputSomatosensory.TurnMul < -BikeInputSomatosensory.TurnThresholdMin * 2;
						yield return null;
					}
					break;
			}
			Guide.Close();
			Time.timeScale = 1;


			//等待到达漂移地点
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}

			Debug.Log("开始漂移教学");

			Guide.HelpOnClickFinger.GetComponent<MaskableGraphic>().raycastTarget = false;
			Guide.HelpOnClickScreen.GetComponent<MaskableGraphic>().raycastTarget = false;
			DriftLeft.gameObject.SetActive(true);
			DriftRight.gameObject.SetActive(true);
			Time.timeScale = 0;

			switch (mode) {
				case GameControlMode.Btn:
				case GameControlMode.GravitySwipe:
					Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_2).ToLocalized());
					Guide.ActiveFinger(true);
					Guide.ActiveDarkHole(true);
					Guide.SetFingerPos(DriftRight.transform);

					var tmpCom2 = DriftRight.gameObject.AddComponent<StoryGuide_1_Game_PointerDown>();
					this.GoNext = false;
					while (!this.GoNext) {
						yield return null;
					}
					Object.Destroy(tmpCom2);
					break;
				case GameControlMode.RemoteControl:
					Guide.SetWordPos(null, "发现急转弯，双击并按住 → 展现你的漂移技术。");
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					yield return new WaitForSecondsRealtime(0.7f);
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext = Input.GetKeyDown(KeyCode.RightArrow);
						yield return null;
					}
					break;
				case GameControlMode.Somatosensory:
					Guide.SetWordPos(null, "发现急转弯，大幅向右倾斜身体，展现你的漂移技术。", null, null);
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					yield return new WaitForSecondsRealtime(0.7f);
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext =
							Orbbec.OrbbecSensingManager.instance.horizontalValue * BikeInputSomatosensory.TurnMul > BikeInputSomatosensory.TurnThresholdMax;
						yield return null;
					}
					break;
			}


			Guide.Close();
			Time.timeScale = 1;


			//等待到达加速地点
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}
			Debug.Log("开始加速教学");
			Client.EventMgr.SendEvent(EventEnum.Guide_Operation, new object[] { "1", "加速教学" });
			GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/N2oBar").SetActive(true);
			BoostImage.gameObject.SetActive(true);
			Guide.HelpOnClickFinger.GetComponent<MaskableGraphic>().raycastTarget = false;
			Guide.HelpOnClickScreen.GetComponent<MaskableGraphic>().raycastTarget = false;
			Time.timeScale = 0;
			switch (mode) {
				case GameControlMode.Btn:
				case GameControlMode.GravitySwipe:
					Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_3).ToLocalized());
					Guide.ActiveFinger(true);
					Guide.ActiveDarkHole(true);
					Guide.SetFingerPos(BoostImage.transform);
					var tmpCom3 = BoostImage.gameObject.AddComponent<StoryGuide_1_Game_Click>();
					this.GoNext = false;
					while (!this.GoNext) {
						yield return null;
					}
					Object.Destroy(tmpCom3);
					break;
				case GameControlMode.RemoteControl:
					Guide.SetWordPos(null, "按 ↑ 释放氮气能提高车速，感受飞一般的感觉。");
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					yield return new WaitForSecondsRealtime(0.7f);
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext = Input.GetKeyDown(KeyCode.UpArrow);
						yield return null;
					}
					break;
				case GameControlMode.Somatosensory:
					Guide.SetWordPos(null, "向前倾斜身体，释放氮气能提高车速，感受飞一般的感觉。");
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					yield return new WaitForSecondsRealtime(0.7f);
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext =
							Orbbec.OrbbecSensingManager.instance.verticalValue > BikeInputSomatosensory.BoostThreshold;
						yield return null;
					}
					break;
			}

			Guide.Close();
			Time.timeScale = 1;

			//等待到达攻击地点
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}
			Debug.Log("开始攻击教学");
			Client.EventMgr.SendEvent(EventEnum.Guide_Operation, new object[] { "1", "攻击教学" });
			Time.timeScale = 0;

			Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_4).ToLocalized(), continueTip);
			Guide.ActiveBlackBack(true);
			ClearScreenClick(null, () => {
				this.GoNext = true;
				Guide.Close();
			});

			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}

			//变到攻击位置
			target = GameObject.Find("attackPreparePos");
			BikeBase player = BikeManager.Ins.Bikes[0];
			player.transform.position = target.transform.position;
			player.transform.rotation = target.transform.rotation;
			player.bikeControl.Rigidbody.velocity = target.transform.forward * 3;
			player.bikeControl.ActiveControl = false;//临时关闭用户输入
			for (int j = 0; j < 15; j++) {
				BikeCamera.Ins.LateUpdate();
			}
			Time.timeScale = 1;
			yield return new WaitForSecondsRealtime(0.7f);
			player.bikeAttack.Target = BikeManager.Ins.Bikes[1];
			player.bikeInput.OnAttack(null);
			yield return new WaitForSecondsRealtime(0.3f);
			var enemy = BikeManager.Ins.Bikes[1];
			enemy.bikeControl.Rigidbody.velocity = GameObject.Find("ai_1").transform.right * -10f;
			enemy.bikeHealth.Damage(100);
			enemy.bikeHealth.Die();
			player.bikeControl.ActiveControl = true;//恢复用户输入

			//等待到达道具地点
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}
			enemy.gameObject.SetActive(false);
			Debug.Log("开始道具教学");
			Client.EventMgr.SendEvent(EventEnum.Guide_Operation, new object[] { "1", "道具教学" });
			Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_5).ToLocalized(), continueTip);
			Guide.ActiveBlackBack(true);
			Time.timeScale = 0;
			ClearScreenClick(null, () => {
				this.GoNext = true;
			});

			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}

			switch (mode) {
				case GameControlMode.Btn:
				case GameControlMode.GravitySwipe:
					try {
						Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_6).ToLocalized());
						Guide.ActiveBlackBack(false);
						Guide.ActiveFinger(true);
						Guide.ActiveDarkHole(true);
						Guide.SetFingerPos(PropImage.transform);
						Guide.HelpOnClickFinger.GetComponent<MaskableGraphic>().raycastTarget = false;
						Guide.HelpOnClickScreen.GetComponent<MaskableGraphic>().raycastTarget = false;

						PropImage.gameObject.SetActive(true);
					} catch (Exception e) {
						Debug.LogException(e);
					}
					var tmpCom4 = PropImage.gameObject.AddComponent<StoryGuide_1_Game_Click>();
					ClearFingerClick(null, () => {
						this.GoNext = true;
						Guide.Close();
					});
					this.GoNext = false;
					while (!this.GoNext) {
						yield return null;
					}
					Time.timeScale = 1;//道具只能在非暂停下释放
					this.PropImage.GetComponent<BtnEquipProp>().OnClick();
					Object.Destroy(tmpCom4);
					break;
				case GameControlMode.RemoteControl:
					Guide.SetWordPos(null, "点击 OK键 使用道具！");
					Guide.ActiveBlackBack(false);
					Guide.ActiveFinger(false);
					Guide.ActiveDarkHole(false);
					Guide.SetFingerPos(PropImage.transform);
					PropImage.gameObject.SetActive(true);
					yield return new WaitForSecondsRealtime(0.7f);
					this.GoNext = false;
					while (!this.GoNext) {
						this.GoNext = JoystickInput.Ins.IsConfirmDown;
						yield return null;
					}
					Time.timeScale = 1;
					this.PropImage.GetComponent<BtnEquipProp>().OnClick();
					break;
				case GameControlMode.Somatosensory:
					try {
						Guide.SetWordPos(null, "抬起左手使用装备的道具");
						Guide.ActiveBlackBack(false);
						Guide.ActiveFinger(false);
						Guide.ActiveDarkHole(false);
						Guide.SetFingerPos(PropImage.transform);
						PropImage.gameObject.SetActive(true);

						Orbbec.OrbbecSensingManager.instance.leftHandRaiseAction += LeftHandRaiseAction;
					} catch (Exception e) {
						Debug.LogException(e);
					}
					this.GoNext = false;
					while (!this.GoNext) {
						yield return null;
					}
					Orbbec.OrbbecSensingManager.instance.leftHandRaiseAction -= LeftHandRaiseAction;
					Time.timeScale = 1;
					this.PropImage.GetComponent<BtnEquipProp>().OnClick();
					break;
			}

			Guide.Close();
			Time.timeScale = 1;
			try {
				var missileIns = Object.FindObjectOfType<Missile>();
				missileIns.Target = BikeManager.Ins.Bikes[2];
			} catch (Exception e) {
				Debug.LogException(e);
			}

			//等待到达结束点
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}
			//教程结束，丢失标定不在中断教程
			HelpOnClick.CanPreseveHelpClick = false;
			Debug.Log("开始结束教学");
			Guide.SetWordPos(null, (LString.GAMECLIENT_STORYGUIDE_1_GAME_STARTGUIDE_7).ToLocalized(), continueTip);
			Guide.ActiveBlackBack(true);
			ClearScreenClick(null, () => {
				this.GoNext = true;
				Guide.Close();
				Disable();
			});
			//等待用户点击
			this.GoNext = false;
			while (!this.GoNext) {
				yield return null;
			}
			//卸下导弹
			Client.User.UserInfo.EquipedPropList.Clear();
			this.Map.SetActive(true);
			this.Pause.SetActive(true);
			this.Rank.SetActive(true);
			this.Bar.SetActive(true);

		}

		private void LeftHandRaiseAction() {
			this.GoNext = true;
		}

		private void OnGamePlayBoardEnter(EventEnum eventEnum, object[] objects) {
			if ((string)objects[0] == "GamePlayBoard") {
				this.GoNext = true;
				Client.EventMgr.RemoveListener(EventEnum.UI_EnterMenu, OnGamePlayBoardEnter);
				this.Map = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/MinMap");
				this.Pause = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/Pause");
				this.Rank = GameObject.Find("UI/Modules/Menu/Group/RaceInfo_Racing(Clone)/Rank");
				this.Bar = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/N2oBar");

				this.Map.SetActive(false);
				this.Pause.SetActive(false);
				this.Rank.SetActive(false);
				this.Bar.SetActive(false);

				GameControlMode controlMode = ModGame.GetControlMode();
				switch (controlMode) {
					case GameControlMode.Btn:
					case GameControlMode.GravitySwipe:
						TurnLeft = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/BtnPlayMode/BtnTrunLeft").GetComponent<MaskableGraphic>();
						TurnRight = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/BtnPlayMode/BtnTrunRight").GetComponent<MaskableGraphic>();
						DriftLeft = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/BtnPlayMode/LeftDrift").GetComponent<MaskableGraphic>();
						DriftRight = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/BtnPlayMode/RightDrift").GetComponent<MaskableGraphic>();
						BoostImage = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/BtnPlayMode/Nitrogen").GetComponent<MaskableGraphic>();
						PropImage = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/BtnPlayMode/BtnEquipProp").GetComponent<MaskableGraphic>();
						break;
					case GameControlMode.RemoteControl:
						DriftLeft = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/RemoteControl/LeftDrift").GetComponent<MaskableGraphic>();
						DriftRight = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/RemoteControl/RightDrift").GetComponent<MaskableGraphic>();
						BoostImage = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/RemoteControl/Nitrogen").GetComponent<MaskableGraphic>();
						PropImage = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/RemoteControl/BtnEquipProp").GetComponent<MaskableGraphic>();
						break;
					case GameControlMode.Somatosensory:
						DriftLeft = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/SomatosensoryControl/LeftDrift").GetComponent<MaskableGraphic>();
						DriftRight = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/SomatosensoryControl/RightDrift").GetComponent<MaskableGraphic>();
						BoostImage = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/SomatosensoryControl/Nitrogen").GetComponent<MaskableGraphic>();
						PropImage = GameObject.Find("UI/Modules/Menu/Group/GamePlayBoard(Clone)/SomatosensoryControl/BtnEquipProp").GetComponent<MaskableGraphic>();
						break;
				}


				if (this.TurnLeft) TurnLeft.gameObject.SetActive(false);
				if (this.TurnRight) TurnRight.gameObject.SetActive(false);
				if (this.DriftLeft) DriftLeft.gameObject.SetActive(false);
				if (this.DriftRight) DriftRight.gameObject.SetActive(false);
				if (this.BoostImage) BoostImage.gameObject.SetActive(false);
				if (this.PropImage) PropImage.gameObject.SetActive(false);
			}
		}

	}
}
