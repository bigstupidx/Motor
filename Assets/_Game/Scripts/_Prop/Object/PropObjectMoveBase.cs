//
// PropObjectMoveBase.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using LTHUtility;

namespace Game {
	public class PropObjectMoveBase : PropObjectBase {

		public float Speed = 100f;
		public float Accel = 0.1f;
		public float MoveHeight = 2f;

		public FadeEffectGroup EffectMove;
		public FadeEffectGroup EffectStop;

		private float _speed;
		private Vector3 _targetPos;
		private WaypointNode _nowPoint;


		/// <summary>
		/// 根据当前普通路径点获取对应AI路径点
		/// </summary>
		/// <param name="wayPointNormal"></param>
		/// <returns></returns>
		public WaypointNode GetAiPointWithNormal(WaypointNode wayPointNormal, RaceLine raceLine) {
			var index = raceLine.WaypointManager.WaypointList.IndexOf(wayPointNormal);
			return index >= 0 ? raceLine.WaypointManagerAI.WaypointList[index] : null;
		}

		public override void OnStart() {
			base.OnStart();
			if (EffectMove != null) EffectMove.FadeIn(0.5f);
			_speed = this.Speed;
			if (User != null) {
				_nowPoint = GetAiPointWithNormal(User.racerInfo.ActuallyPoint, RaceManager.Ins.raceLine);
				_targetPos = _nowPoint.NextPoint.Position + Vector3.up * MoveHeight; ;
			}
		}

		public override void OnUpdate() {
			base.OnUpdate();
			// 提速
			_speed += Time.deltaTime * this.Accel;
			// 寻找目标
			if (Target == null) {
				var tar = BikeManager.Ins.FindNearstEneny(User, transform, 50f, 75f);
				if (tar != null && User.racerInfo.Before == tar.racerInfo) {
					Target = tar;
					Target.bikeAttack.OnBeLocked(User, Type);
				}
			}
			// 寻路
			if (Target != null) {
				// 跟踪敌人
				transform.rotation = Quaternion.LookRotation(Target.transform.position + Vector3.up * this.MoveHeight - transform.position);
			} else {
				// 沿路飞行
				if (WaypointMath.CheckPassWaypoint(transform.position, _nowPoint, _nowPoint.NextPoint)) {
					_nowPoint = _nowPoint.NextPoint;
					_targetPos = _nowPoint.NextPoint.Position + Vector3.up * MoveHeight;
				}
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_targetPos - transform.position), Time.deltaTime * 2);
			}
			transform.Translate(transform.forward * _speed * Time.deltaTime, Space.World);
			// 贴地飞行
//			var pos = WaypointMath.AttachRoadPoint(transform.position);
//			transform.SetPositionY(Mathf.Lerp(transform.position.y, pos.y + this.MoveHeight, 20 * Time.deltaTime));
		}

		public override void OnStop() {
			if (Target != null) {
				Target.bikeAttack.OnBeLockedCancel();
			}
			base.OnStop();
			if (EffectMove != null) {
				EffectMove.FadeOut(0.5f);
			}
			if (EffectStop != null) {
				EffectStop.gameObject.SetActive(true);
				//				EffectStop.FadeIn(0.1f);
			}
		}
	}

}
