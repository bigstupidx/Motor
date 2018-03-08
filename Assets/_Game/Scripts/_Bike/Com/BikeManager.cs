//
// BikeManager.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections.Generic;
using GameClient;
using GameUI;
using Joystick;
using LTHUtility;
using RoomBasedClient;
using XPlugin.Update;

namespace Game {
	public class BikeManager : Singleton<BikeManager> {
		[System.NonSerialized]
		public List<RacerInfo> Bikes = new List<RacerInfo>();

		private BikeBase _currentBike;
		public BikeBase CurrentBike {
			get { return this._currentBike; }
			set {
				this._currentBike = value;
			}
		}


//		void OnGUI() {
//			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.dpi / 100f, (float)Screen.dpi / 100f, (float)Screen.dpi / 100f));
//			Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 1);
//
//			foreach (var bike in this.Bikes) {
//				if (GUILayout.Button(bike.name)) {
//					bike.gameObject.SetActive(!bike.gameObject.activeSelf);
//				}
//			}
//
//			foreach (var t in PerformanceTest.Lists) {
//				if (GUILayout.Button(t.gameObject.name)) {
//					t.gameObject.SetActive(!t.gameObject.activeSelf);
//				}
//			}
//		}

		protected override void Awake() {
			base.Awake();
			if (Client.Ins != null) {
				Client.User.UserInfo.Setting.OnControlModeChange += ChangeController;
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if (Client.Ins != null) {
				Client.User.UserInfo.Setting.OnControlModeChange -= ChangeController;
			}
			if (ModHUD.Ins != null) {
				ModHUD.Ins.Clean(true);
			}
		}

		private void ChangeController(GameControlMode mode) {
			ChangeController(CurrentBike, mode);
		}

		/// <summary>
		/// 根据信息创建车辆和英雄
		/// </summary>
		public BikeBase CreateBike(PlayerInfo info, bool isPlayer) {
			info.IsPlayer = isPlayer;
			// 加载车
			var bikeNmae = info.ChoosedBike.Data.Prefab;
			var prefab = UResources.Load<GameObject>(bikeNmae);
			Transform spawnPoint = GetSpawnPoint(info.SpawnPos);
			BikeBase bike;
			if (isPlayer) {
				bike = Lobby.Ins.RoomClient.Instantiate(prefab.name, spawnPoint.position, spawnPoint.rotation, new object[] { isPlayer, info }).GetComponent<BikeBase>();
			} else {
				bike = Lobby.Ins.RoomClient.InstantiateSceneView(prefab.name, spawnPoint.position, spawnPoint.rotation, new object[] { isPlayer, info }).GetComponent<BikeBase>();
			}
			return bike;
		}

		public void ApplyBikeInfo(PlayerInfo info, BikeBase bike) {
			// 初始化车辆
			bike.bikeControl.ClearBikeRigidbody();
			bike.name = Bikes.Count + bike.name;
			// 初始化数据
			bike.InitInfo(info);
			info.Bike = bike;
			info.ResetStatValue();
			// 开启特殊能力
			info.SetAllAbilitiesActive(true);
			// 新手教程 和 计时赛 不显示HUD
			if (GameModeBase.Ins.Match.MatchMode != MatchMode.Guide && RaceManager.Ins.RaceMode != RaceMode.Timing) {
				SetHud(bike, info.IsPlayer);
			}
		}

		/// <summary>
		/// 设置HUD信息
		/// </summary>
		public void SetHud(BikeBase bike, bool isPlayer) {
			// 设置HUD
			var hud = bike.gameObject.AddComponent<DisplayHud>();
			hud.SetBike(bike);
			hud.ShowHud();
			hud.SetIsSelf(isPlayer);
			// 重置时重设
			bike.racerInfo.OnReset += b => {
				hud.SetBike(bike);
			};
		}

		/// <summary>
		/// 激活/关闭所有车辆
		/// </summary>
		public void SetAllBikeActive(bool enable) {
			foreach (RacerInfo b in Bikes) {
				SetBikeActive(b, enable);
			}
		}

		/// <summary>
		/// 激活/关闭车辆
		/// </summary>
		public void SetBikeActive(BikeBase bike, bool enable) {
			var contol = bike.bikeControl;
			if (!enable) {
				contol.Rigidbody.ResetInertiaTensor();
				contol.Boosting = false;
			}
			contol.ActiveControl = enable;
		}

		/// <summary>
		/// 为车分配出生点
		/// </summary>
		private Transform GetSpawnPoint(int spawnPos) {
			var list = RaceManager.Ins.raceLine.SpawnpointManager.SpawnpointList;
			var index = 0;
			if (spawnPos < 0) {//玩家没有给定出生点
				if (!RoomClient.OfflineMode) {//联网模式从属性中取
					var PlayerInfo = (PlayerInfo)Lobby.Ins.RoomClient.ToRoomClient.MimePlayer.CustomProperties["PlayerInfo"];
					index = PlayerInfo.SpawnPos;
				} else {//其他模式出生在最后
					index = list.Count - 1;
				}
			} else {
				index = spawnPos;
			}
			return list[index].transform;
		}

		/// <summary>
		/// 设置为玩家
		/// </summary>
		public void SetBikeAsPlayer(BikeBase bike) {
			if (FocusManager.TVMode) {
				if (BikeOrbecController.HasOrbbecDevice()) {
					ChangeController(bike, GameControlMode.Somatosensory);
				} else {
					ChangeController(bike, GameControlMode.RemoteControl);
				}
			} else {
				ChangeController(bike, Client.User.UserInfo.Setting.ControlMode);
			}
			bike.gameObject.SetLayerRecursion(Layers.Ins.Player);
			bike.gameObject.SetTagRecursion(Tags.Ins.Player);
			ProjectorShadow.ProjectorShadow.Ins.Target = bike.transform;

			// 结束事件
			bike.racerInfo.OnFinish += self => {
				// 游戏结束，玩家改为AI继续跑
				SetBikeAsAi(self);
				self.gameObject.SetLayerRecursion(Layers.Ins.Over);
				GameModeBase.Ins.FinishGame();
				StartLine.Ins.HideAll();
			};
		}

		/// <summary>
		/// 设置为敌人
		/// </summary>
		/// <param name="bike"></param>
		/// <param name="aiLevel"></param>
		public void SetBikeAsEnemy(BikeBase bike, int aiLevel = 5) {
			bike.gameObject.SetLayerRecursion(Layers.Ins.Enemy);
			bike.gameObject.SetTagRecursion(Tags.Ins.Enemy);
			SetBikeAsAi(bike, aiLevel);

			// 卡住自动重置
			bike.racerInfo.OnStuck += self => {
				self.racerInfo.DoReset();
			};
		}

		/// <summary>
		/// 设置为AI自动驾驶
		/// </summary>
		/// <param name="bike"></param>
		/// <param name="aiLevel"></param>
		public void SetBikeAsAi(BikeBase bike, int aiLevel = 5) {
			if (bike == null) return;
			var nowInputs = bike.GetComponents<BikeInputImplBase>();
			foreach (var input in nowInputs) {
				Destroy(input);
			}
			var inputAi = bike.gameObject.AddComponent<BikeInputAi>();
			inputAi.WaypointManager = RaceManager.Ins.raceLine.WaypointManagerAI;
			inputAi.AiLevel = aiLevel;
		}

		private void ChangeController(BikeBase bike, GameControlMode controlMode) {
			bike.gameObject.GetOrAddComponent<BikeInputTouch>().enabled = controlMode == GameControlMode.Btn;
			//			bike.gameObject.GetOrAddComponent<BikeInputSwipe>().enabled = controlMode == GameControlMode.Swipe;
			bike.gameObject.GetOrAddComponent<BikeInputGravitySwipe>().enabled = controlMode == GameControlMode.GravitySwipe;
			bike.gameObject.GetOrAddComponent<BikeInputRemoteContol>().enabled = controlMode == GameControlMode.RemoteControl;
			bike.gameObject.GetOrAddComponent<BikeInputSomatosensory>().enabled = controlMode == GameControlMode.Somatosensory;
		}

		/// <summary>
		/// 查找最近的敌人(圆形区域)
		/// </summary>
		/// <returns></returns>
		public BikeBase FindNearstEneny(BikeBase bike, float attackRange) {
			BikeBase ret = null;
			var minDis = Mathf.Infinity;
			for (int i = 0; i < Ins.Bikes.Count; i++) {
				var enemy = Ins.Bikes[i];
				if (enemy.bikeControl.GetInstanceID() == bike.bikeControl.GetInstanceID() || !enemy.bikeHealth.IsAlive) {
					continue;
				}

				var dis = Vector3.Distance(bike.transform.position, enemy.transform.position);
				if (dis < minDis) {
					minDis = dis;
					ret = enemy;
				}
			}
			if (minDis <= attackRange) {
				return ret;
			}
			return null;
		}

		/// <summary>
		/// 查找最近的敌人(扇形区域)
		/// </summary>
		/// <param name="bike"></param>
		/// <param name="attackRange"></param>
		/// <param name="attackAngle">正前方角度范围</param>
		/// <returns></returns>
		public BikeBase FindNearstEneny(BikeBase bike, float attackRange, float attackAngle) {
			BikeBase ret = null;
			var minDis = Mathf.Infinity;
			for (int i = 0; i < Ins.Bikes.Count; i++) {
				var enemy = Ins.Bikes[i];
				if (enemy.bikeControl.GetInstanceID() == bike.bikeControl.GetInstanceID() || !enemy.bikeHealth.IsAlive) {
					continue;
				}

				var dis = Vector3.Distance(bike.transform.position, enemy.transform.position);
				var angle =
					Mathf.Acos(Vector3.Dot((enemy.transform.position - bike.transform.position).normalized,
						bike.transform.forward)) * Mathf.Rad2Deg;
				if (dis < minDis && angle < attackAngle) {
					minDis = dis;
					ret = enemy;
				}
			}
			if (minDis <= attackRange) {
				return ret;
			}
			return null;
		}

		/// <summary>
		/// 查找最近的敌人(扇形区域)
		/// </summary>
		/// <param name="self"></param>
		/// <param name="user"></param>
		/// <param name="attackRange"></param> 
		/// <param name="attackAngle">正前方角度范围</param>
		/// <returns></returns>
		public BikeBase FindNearstEneny(BikeBase user, Transform self, float attackRange, float attackAngle) {
			BikeBase ret = null;
			var minDis = Mathf.Infinity;
			for (int i = 0; i < Ins.Bikes.Count; i++) {
				var enemy = Ins.Bikes[i];
				if (enemy.bikeControl.GetInstanceID() == user.bikeControl.GetInstanceID() || !enemy.bikeHealth.IsAlive) {
					continue;
				}

				var dis = Vector3.Distance(self.position, enemy.transform.position);
				var angle =
					Mathf.Acos(Vector3.Dot((enemy.transform.position - self.position).normalized, self.transform.forward.normalized)) *
					Mathf.Rad2Deg;
				if (dis < minDis && angle < attackAngle) {
					minDis = dis;
					ret = enemy;
				}
			}
			if (minDis <= attackRange) {
				return ret;
			}
			return null;
		}

	}


}
