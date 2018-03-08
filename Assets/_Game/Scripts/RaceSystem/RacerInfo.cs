//
// RacerInfo.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 比赛信息
	/// 挂在玩家身上用于获取实时信息
	/// </summary>
	public class RacerInfo : BikeBase {

		/// <summary>
		/// 比赛结果 - 每一圈的信息
		/// </summary>
		public class TurnResult {
			public float RunTime;
			public int Turn;
			public int Rank;
		}


		public WaypointManager WaypointMnager;
		public WaypointNode NowPoint;
		public WaypointNode ActuallyPoint;

		#region 实时数据
		/// <summary>
		/// 每一圈的结果记录
		/// </summary>
		[NonSerialized]
		public List<TurnResult> Result = new List<TurnResult>();
		/// <summary>
		/// 通过点列表(累加记录多圈)
		/// </summary>
		[NonSerialized]
		public List<WaypointNode> PassPoint = new List<WaypointNode>();
		[NonSerialized]
		public List<WaypointNode> ReservePassPoint = new List<WaypointNode>();
		/// <summary>
		/// 名次
		/// </summary>
		public int Rank {
			get {
				return RaceManager.Ins.GetPlayerRank(this);
			}
		}
		/// <summary>
		/// 是否第一
		/// </summary>
		public bool IsFirst {
			get { return Rank == 1; }
		}
		/// <summary>
		/// 是否最后
		/// </summary>
		public bool IsLast {
			get { return Rank == RaceManager.Ins.PlayerNum; }
		}
		/// <summary>
		/// 前一名玩家
		/// </summary>
		public RacerInfo Before {
			get { return IsFirst ? null : RaceManager.Ins.GetPlayerByRank(Rank - 1); }
		}
		/// <summary>
		/// 后一名玩家
		/// </summary>
		public RacerInfo After {
			get { return IsLast ? null : RaceManager.Ins.GetPlayerByRank(Rank + 1); }
		}
		/// <summary>
		/// 总距离
		/// </summary>
		public float Distance { get; private set; }
		/// <summary>
		/// 单圈距离
		/// </summary>
		public float SingleTurnDistance {
			get {
				var dis = Distance;
				while (dis > WayLength) {
					dis -= WayLength;
				}
				return dis;
			}
		}

		/// <summary>
		/// 赛道总长(1圈)
		/// </summary>
		public float WayLength { get { return WaypointMnager.WayLength; } }
		/// <summary>
		/// 赛道总长(N圈)
		/// </summary>
		public float WaysLength { get { return WaypointMnager.WayLength * RaceManager.Ins.Turn; } }
		/// <summary>
		/// 完成进度(0-1)
		/// </summary>
		public float TotalSchedule {
			get {
				var schedule = Distance / WaysLength;
				return Mathf.Clamp(schedule, 0, 1f);
			}
		}
		/// <summary>
		/// 当前单圈完成进度(0-1)
		/// </summary>
		public float SingleTurnSchedule {
			get {
				var schedule = (Distance - WaypointMnager.WayLength * Turn) / WaypointMnager.WayLength;
				return Mathf.Clamp(schedule, 0, 1f);
			}
		}
		/// <summary>
		/// 总时间
		/// </summary>
		public float RunTime { get; set; }
		/// <summary>
		/// 圈数
		/// </summary>
		public int Turn { get; private set; }
		/// <summary>
		/// 是否错误方向
		/// </summary>
		public bool WrongDirection { get; private set; }
		/// <summary>
		/// 是否被卡住(超过一定时间速度小于一定值)
		/// </summary>
		public bool Stuck { get; private set; }
		/// <summary>
		/// 是否在跑
		/// </summary>
		public bool Running { get; set; }
		/// <summary>
		/// 是否通过了终点(只算最后一圈)
		/// </summary>
		public bool IsPassFinishPoint { get; private set; }
		/// <summary>
		/// 是否完成比赛<para/>
		/// 淘汰赛正常结束，计时赛通过所有检查点结束，竞速赛通过终点结束
		/// </summary>
		public bool IsCompleteGame {
			get {
				if (RaceManager.Ins.GamePhase != GamePhase.Over) return false;
				switch (RaceManager.Ins.RaceMode) {
					case RaceMode.Elimination:
						return true;
					case RaceMode.Timing:
						return GameModeTiming.Ins.PassCount == GameModeTiming.Ins.CheckerCount;
					default:
						return IsPassFinishPoint;
				}
			}
		}
		/// <summary>
		/// 是否结束
		/// </summary>
		public bool IsFinish { get; private set; }
		#endregion

		#region 接口
		/// <summary>
		/// 开始
		/// </summary>
		public Action<BikeBase> OnStart = delegate { };
		/// <summary>
		/// 通过路径点
		/// </summary>
		public Action<BikeBase> OnPassPoint = delegate { };
		/// <summary>
		/// 通过终点
		/// </summary>
		public Action<BikeBase> OnPassFinishPoint = delegate { };
		/// <summary>
		/// 方向错误
		/// </summary>
		public Action<BikeBase> OnWrongDirection = delegate { };
		/// <summary>
		/// 保持方向错误达到一定时间
		/// </summary>
		public Action<BikeBase> OnStayWrongDirection = delegate { };
		/// <summary>
		/// 从错误方向回到正确方向
		/// </summary>
		public Action<BikeBase> OnRightDirection = delegate { };
		/// <summary>
		/// 被卡住(低于一定速度达到一定时间)
		/// </summary>
		public Action<BikeBase> OnStuck = delegate { };
		/// <summary>
		/// 从卡住状态恢复
		/// </summary>
		public Action<BikeBase> OnUnStuck = delegate { };
		/// <summary>
		/// 结束
		/// </summary>
		public Action<BikeBase> OnFinish = delegate { };
		/// <summary>
		/// 坠落
		/// </summary>
		public Action<BikeBase> OnDrop = delegate { };
		/// <summary>
		/// 复位
		/// </summary>
		public Action<BikeBase> OnReset = delegate { };
		/// <summary>
		/// 复位
		/// </summary>
		public Action<BikeBase> OnResetHud = delegate { };
		#endregion

		#region 私有
		/// <summary>
		/// 反向通过终点次数
		/// </summary>
		private int _reservePassFinishPoint;
		/// <summary>
		/// 保持错误方向提示时间阈值
		/// </summary>
		private float _stayWrongDirectionTimeValue = 2f;
		/// <summary>
		/// 保持错误方向的时间
		/// </summary>
		private float _stayWrongDirectionTime;
		/// <summary>
		/// 是否保持错误方向
		/// </summary>
		private bool _stayWrongDirection;
		/// <summary>
		/// 卡住速度阈值
		/// </summary>
		private float _stuckSpeedValue = 25f;
		/// <summary>
		/// 卡住经过时间
		/// </summary>
		private float _stuckTime;
		/// <summary>
		/// 卡住时间阈值
		/// </summary>
		private float _stuckTimeValue = 4f;
		#endregion

		public override void Awake() {
			base.Awake();
			if (RaceManager.Ins != null) {
				RaceManager.Ins.PlayerList.Add(this);
			}
			if (BikeManager.Ins != null) {
				BikeManager.Ins.Bikes.Add(this);
			}
		}

		void OnDestroy() {
			if (BikeManager.Ins != null) {
				BikeManager.Ins.Bikes.Remove(this);
			}
		}

		#region 实时检测 & 计算
		void Update() {
			if (RaceManager.Ins == null || RaceManager.Ins.GamePhase == GamePhase.CountDown || RaceManager.Ins.GamePhase == GamePhase.Waiting) {
				return;
			}
			var rightDirection = false;
			if (NowPoint == ActuallyPoint) {
				// 检查通过最远目标点
				if (WaypointMath.CheckPassWaypoint(transform.position, NowPoint, NowPoint.NextPoint)) {
					rightDirection = true;
					DoPassPoint();
				}
			} else {
				// 检查实际通过路径点
				if (WaypointMath.CheckPassWaypoint(transform.position, ActuallyPoint, ActuallyPoint.NextPoint)) {
					ActuallyPoint = ActuallyPoint.NextPoint;
					rightDirection = true;
					// 重新通过终点计数
					if (ActuallyPoint == WaypointMnager.FinishPoint) {
						_reservePassFinishPoint--;
					}
					// DebugLog("Time : " + RunTime + "\t RePass :" + ActuallywPoint.name + " \t" + PassPoint.Count);
				}
			}
			if (rightDirection) {
				if (ReservePassPoint.Count == 0) {
					PassPoint.Add(ActuallyPoint);
				} else {
					ReservePassPoint.RemoveAt(ReservePassPoint.Count - 1);
				}
			}
			// 检查实际位置的目标点和位置点
			if (WaypointMath.CheckPassWaypoint(transform.position, ActuallyPoint.NextPoint, ActuallyPoint)) {
				ActuallyPoint = ActuallyPoint.PrePoint;
				// 向后移动通过了已经通过的点
				ReservePassPoint.Add(ActuallyPoint);
				//if (PassPoint.Count > 0) PassPoint.RemoveAt(PassPoint.Count - 1);

				// 反向通过终点计数
				if (ActuallyPoint.PrePoint == WaypointMnager.StartPoint.PrePoint) {
					_reservePassFinishPoint++;
				}
				// DebugLog("Time : " + RunTime + "\t Back :" + ActuallywPoint.name + " \t" + PassPoint.Count);
			}

			// 未完成时
			if (RaceManager.Ins.Turn > 0 && RaceManager.Ins.Turn > Turn) {
				if (RaceManager.Ins.RaceMode == RaceMode.Timing && RaceManager.Ins.GamePhase == GamePhase.Over) {
					return;
				}
				if (IsFinish) {
					return;
				}
				// 计时
				RunTime += Time.deltaTime;
				//计算距离
				Distance = GetDistance();
			}

			// 无法操控时不检测
			if (!Running || !bikeHealth.IsAlive) return;
			if (!bikeControl.ActiveControl) return;
			// 游戏结束则停止更新数据
			if (RaceManager.Ins.GamePhase != GamePhase.Gaming) return;
			// 检查卡住
			CheckStuck();
			// 检查错误方向
			CheckWrongDirection();
		}

		/// <summary>
		/// 检查卡住
		/// </summary>
		private void CheckStuck() {
			if (bikeControl.Speed < _stuckSpeedValue) {
				_stuckTime += Time.deltaTime;
			}
			// 卡住
			if (_stuckTime >= _stuckTimeValue && !Stuck) {
				DoStuck();
			}
			if (bikeControl.Speed >= _stuckSpeedValue) {
				_stuckTime = 0;
				// 恢复
				if (Stuck) {
					DoUnStuck();
				}
			}
		}

		/// <summary>
		/// 检测错误方向
		/// </summary>
		/// <returns></returns>
		private void CheckWrongDirection() {
			var direction = false;
			var angle = Vector3.Angle(transform.forward, ActuallyPoint.NextPoint.Position - ActuallyPoint.Position);
			if (angle > 100) {
				direction = true;
			}
			// 进入错误或正确方向检测
			if (WrongDirection != direction) {
				WrongDirection = direction;
				if (WrongDirection) {
					DoWrongDirection();
				} else {
					_stayWrongDirection = false;
					_stayWrongDirectionTime = 0;
					DoRightDirection();
				}
			}
			// 持续错误方向检测
			if (WrongDirection) {
				_stayWrongDirectionTime += Time.deltaTime;
				if (_stayWrongDirectionTime >= _stayWrongDirectionTimeValue && !_stayWrongDirection) {
					_stayWrongDirection = true;
					DoStayWrongDirection();
				}
			}
		}

		/// <summary>
		/// 计算距离
		/// </summary>
		/// <returns></returns>
		private float GetDistance() {
			var result = 0f;
			var index = 0;
			// 找到起点，忽略游戏开始时倒开的部分
			for (; index < PassPoint.Count; index++) {
				if (PassPoint[index] == WaypointMnager.StartPoint) {
					break;
				}
			}
			if (index == PassPoint.Count) return result;
			// 已通过点的路径
			for (var i = index; i < PassPoint.Count - 1; i++) {
				result += PassPoint[i].NextDistance;
			}
			// 当前所处路段通过距离
			if (PassPoint.Count > 0) {
				var now = PassPoint[PassPoint.Count - 1];
				var next = now.NextPoint;
				// 两个路径点切线的夹角，过小时当作直线路段处理（防止交点在无限远处）
				var angle = Vector3.Angle(now.Right - now.Left, next.Right - next.Left);
				float dis;
				if (angle < 5) {
					dis = WaypointMath.DisPoint2Line(transform.position, now.Left, now.Right);
				} else {
					// 两路径点切线的交点
					var cross = new Vector2();
					WaypointMath.GetIntersection(now.Left, now.Right, next.Left, next.Right, ref cross);
					// 交点与路径线的交点
					var cx = new Vector3(cross.x, 0, cross.y);
					var position = new Vector2();
					WaypointMath.GetIntersection(now.Position, next.Position, transform.position, cx, ref position);
					var pos = new Vector3(position.x, (now.Position.y + next.Position.y) / 2, position.y);
					dis = Vector3.Distance(pos, now.Position);
				}
				result += dis;
			}
			return result;
		}
		#endregion

		#region 事件
		/// <summary>
		/// 开始
		/// </summary>
		public void DoStart(WaypointManager wm) {
			this.WaypointMnager = wm;
			// 各种初始化
			RunTime = 0;
			Distance = -1;
			Turn = 0;
			_reservePassFinishPoint = 0;
			_stayWrongDirection = false;
			_stayWrongDirectionTime = 0;
			_stuckTime = 0;
			IsFinish = false;
			Result.Clear();
			PassPoint.Clear();
			WrongDirection = false;
			bikeControl.ActiveControl = true;
			Stuck = false;
			Running = true;
			IsPassFinishPoint = false;
			NowPoint = WaypointMnager.StartPoint;
			ActuallyPoint = NowPoint;
			PassPoint.Add(ActuallyPoint);

			OnStart(this);
		}

		/// <summary>
		/// 当通过路径点
		/// </summary>
		private void DoPassPoint() {
			NowPoint = NowPoint.NextPoint;
			ActuallyPoint = NowPoint;
			// DebugLog("Time : " + RunTime + "\t Pass :" + NowPoint.name + " \t" + PassPoint.Count);
			// 通过起点
			if (NowPoint == WaypointMnager.StartPoint) {
				DoPassStartPoint();
			}
			// 通过终点
			if (NowPoint == WaypointMnager.FinishPoint) {
				if (_reservePassFinishPoint <= 0 && RaceManager.Ins.Turn > Turn) {
					DoPassFinishPoint();
				}
			}

			OnPassPoint(this);
		}

		/// <summary>
		/// 通过起点
		/// </summary>
		private void DoPassStartPoint() {
		}

		/// <summary>
		/// 通过终点
		/// </summary>
		private void DoPassFinishPoint() {
			if (PassPoint.Count >= 3 && ReservePassPoint.Count == 0) {
				Turn++;
				// 记录每圈结果
				var time = RunTime;
				foreach (var res in Result) {
					time -= res.RunTime;
				}
				var result = new TurnResult() {
					Rank = Rank,
					RunTime = time,
					Turn = Turn
				};
				Result.Add(result);
				// 是否结束
				if (Turn == RaceManager.Ins.Turn && Turn > 0) {
					if (RaceManager.Ins.RaceMode == RaceMode.Racing) {
						IsPassFinishPoint = true;
						DoFinish();
					}
				}
			}
			// Debug.Log("Time : " + RunTime + "\t Pass Start Point :" + NowPoint.name + "\t Turn : " + Turn);
			OnPassFinishPoint(this);
		}

		/// <summary>
		/// 方向错误
		/// </summary>
		private void DoWrongDirection() {

			OnWrongDirection(this);
		}

		/// <summary>
		/// 持续错误方向一段时间
		/// </summary>
		private void DoStayWrongDirection() {
			OnStayWrongDirection(this);
		}

		/// <summary>
		/// 从错误方向回到正确方向
		/// </summary>
		private void DoRightDirection() {
			OnRightDirection(this);
		}

		/// <summary>
		/// 卡住
		/// </summary>
		private void DoStuck() {
			Stuck = true;
			OnStuck(this);
		}

		/// <summary>
		/// 从卡住状态恢复
		/// </summary>
		private void DoUnStuck() {
			Stuck = false;
			OnUnStuck(this);
		}

		/// <summary>
		/// 结束
		/// </summary>
		public void DoFinish() {
			if (IsFinish) return;
			IsFinish = true;
			// 结束处理
			Distance = Turn * WaypointMnager.WayLength;

			// 数据转移
			RaceManager.Ins.PlayerList.Remove(this);
			switch (RaceManager.Ins.RaceMode) {
				case RaceMode.Racing:
					// 顺序插入
					RaceManager.Ins.FinishList.Add(this);
					break;
				case RaceMode.Elimination:
					// 逆序插入
					RaceManager.Ins.FinishList.Insert(0, this);
					break;
				case RaceMode.Timing:
					RaceManager.Ins.FinishList.Add(this);
					break;
			}

			OnFinish(this);
		}

		/// <summary>
		/// 坠落
		/// </summary>
		public void DoDrop() {
			DoReset();
			OnDrop(this);
		}

		/// <summary>
		/// 复位
		/// </summary>
		/// <param name="resetPos">是否重设位置到上一个路径点</param>
		public void DoReset(bool resetPos = true) {
			bikeBuff.StopAll();
			if (this.ActuallyPoint == null) {
				return;
			}
			// 复位,如果不是起点，则回到上一个经过的点
			Vector3 pos;
			if (resetPos) {
				pos = ActuallyPoint.Position + ActuallyPoint.transform.forward.normalized;
				pos.z += UnityEngine.Random.Range(-5, 5);
				pos.x += UnityEngine.Random.Range(-5, 5);
			} else {
				pos = transform.localPosition;
			}


			// 重设到地面，朝向目标点
			pos = WaypointMath.AttachRoadPoint(pos + Vector3.up * 2) + Vector3.up * 0.1f;
			transform.localPosition = pos;
			transform.localRotation = Quaternion.LookRotation(ActuallyPoint.NextPoint.Position - ActuallyPoint.Position, Vector3.up);

			// 重设驾驶员
			var driver = bikeControl.bikeDriver;
			Destroy(driver.Driver.gameObject);
			driver.CreateDriver(this.info);

			// 重设车辆控制状态
			BikeManager.Ins.SetBikeActive(this, true);
			bikeControl.ClearBikeRigidbody();
			bikeControl.Boosting = false;

			// 重置状态监测数据
			_stayWrongDirection = false;
			_stayWrongDirectionTime = 0;
			_stuckTime = 0;
			OnResetHud(this);
			bikeBuff.buffBlink.ReStart(5);
			bikeState.Fsm.processEvent(BikeFSM.Event.Reset);
			OnReset(this);
		}
		#endregion

		//		void OnGUI() {
		//		if (gameObject.layer != LayerMask.NameToLayer("15_Player")) {
		//			return;
		//		}
		//		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.dpi / 100f, (float)Screen.dpi / 100f, (float)Screen.dpi / 100f));
		//		GUI.color = new Color(1f, 0f, 0f);
		//
		//		GUI.Label(new Rect(100, 20, 500, 150), "Turn : " + (Turn + 1));
		//		GUI.Label(new Rect(100, 40, 500, 150), "Arrive : " + NowPoint.name);
		//		GUI.Label(new Rect(100, 60, 500, 150), "Actual : " + ActuallyPoint.name);
		//		GUI.Label(new Rect(100, 80, 500, 150), "Time : " + RunTime);
		//		GUI.Label(new Rect(100, 100, 500, 150), "Distance : " + Distance);
		//		GUI.Label(new Rect(100, 120, 500, 150), "Rank : " + Rank);
		//
		//		for (var i = 0; i < Result.Count; i++) {
		//			GUI.Label(new Rect(280, 20 + 20 * i, 500, 150), "Turn : " + Result[i].Turn);
		//			GUI.Label(new Rect(360, 20 + 20 * i, 500, 150), "Rank : " + Result[i].Rank);
		//			GUI.Label(new Rect(440, 20 + 20 * i, 500, 150), "Time : " + Result[i].RunTime);
		//		}
		//
		//		if (WrongDirection) {
		//			GUI.Label(new Rect(120, 140, 500, 150), "Wrong Direction !!!");
		//		}
		//		}
	}
}
