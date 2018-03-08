//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using HeavyDutyInspector;
using UnityEngine;

namespace  Game
{
	public class TimeCheckerSpawner : MonoBehaviour
	{

		[ComplexHeader("时间恢复量 ( s )", Style.Box, Alignment.Left, ColorEnum.Green, ColorEnum.White)]
		public float RecoverTime = 15f;
		[ComplexHeader("完美通过时间奖励 ( 0 - 1 )", Style.Box, Alignment.Left, ColorEnum.Green, ColorEnum.White)]
		public float PerfectReward = 0.2f;
		
		[ComplexHeader("正常通过区域宽度", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public float NormalFieldWidth = 80;
		[ComplexHeader("完美通过区域宽度", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public float PerfectFieldWidth = 5;
		[ComplexHeader("完美通过区域偏移量", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public float PerfectFieldPosX = 0;

		[ComplexHeader("所属路径点", Style.Box, Alignment.Left, ColorEnum.White, ColorEnum.White)]
		public WaypointNode WayPoint;

		[HideInInspector]
		public TimeCheckPoint Checker;

		private string _prefab = "TimeCheckPoint";

		void OnValidate() {
			if (Checker == null)
			{
				var trans = transform.childCount > 0 ? transform.GetChild(0): null;
				if (trans != null) Checker = trans.GetComponent<TimeCheckPoint>();
			}
			CopyValue();
		}

#if UNITY_EDITOR
		[SerializeField]
		[Button("生成 时间检查点", "SpawnChecker")]
		private bool _btnCreateTimeCheck;
		public void SpawnChecker()
		{
			Spawn();
		}

		[SerializeField]
		[Button("重置 时间检查点", "ResetChecker")]
		private bool _btnResetChecker;
		public void ResetChecker()
		{
			if(Checker != null) ResetValue();
			else CreateChecker();
		}

		[SerializeField]
		[Button("销毁 时间检查点", "DestoryChecker")]
		private bool _btnDestoryChecker;
		public void DestoryChecker() {
			if (Checker != null) DestroyImmediate(Checker.gameObject);
		}
#endif

		public void Spawn()
		{
			if (Checker == null) CreateChecker();
			CopyValue();
		}

		public void CreateChecker() {
			if (Checker != null) return;
			Checker = GameObjectUtility.LoadAndIns(_prefab).GetComponent<TimeCheckPoint>();
			Checker.transform.SetParent(transform);
			Checker.transform.ResetLocal();
		}

		public void CopyValue()
		{
			if (Checker == null) return;
			Checker.RecoverTime = RecoverTime;
			Checker.PerfectReward = PerfectReward;
			Checker.WayPoint = WayPoint;
			Checker.Normal.transform.SetLocalScaleX(NormalFieldWidth);
			Checker.Perfect.transform.SetLocalScaleX(PerfectFieldWidth);
			Checker.Perfect.transform.SetLocalPositionX(PerfectFieldPosX);
			Checker.Clock.transform.SetLocalPositionX(PerfectFieldPosX);
		}

		public void ResetValue()
		{
			if (Checker == null) return;
			RecoverTime = Checker.RecoverTime;
			PerfectReward = Checker.PerfectReward;
			WayPoint = Checker.WayPoint;
			NormalFieldWidth = Checker.Normal.transform.localScale.x;
			PerfectFieldWidth = Checker.Perfect.transform.localScale.x;
			PerfectFieldPosX = Checker.Perfect.transform.localPosition.x;
		}
	}
}

