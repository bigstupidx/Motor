//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Game;
using HeavyDutyInspector;

public class TimeCheckPoint : MonoBehaviour {

	[ComplexHeader("正常通过恢复的时间 (s)", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
	public float RecoverTime = 15f;
	[ComplexHeader("完美通过额外奖励时间比率 (0-1)", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
	public float PerfectReward = 0.2f;

	[HideInInspector]
	public WaypointNode WayPoint;

	[ComplexHeader(" ", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
	public ColliderHelper Perfect;
	public ColliderHelper Normal;

	public GameObject Clock;

	void Start()
	{
		Perfect.TriggerEnter += OnPerfectPass;
		Normal.TriggerEnter += OnNormalPass;
		Normal.TriggerExit += OnNormalExit;
		if (WayPoint.Type != WayPointType.Normal)
		{
			Perfect.gameObject.SetActive(false);
			Clock.SetActive(false);
		}
	}

	private void OnNormalExit(Collider other) {
		if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag(Tags.Ins.Player)) return;
		if (other.attachedRigidbody.GetComponent<BikeControl>() == null) return;
		gameObject.SetActive(false);
	}

	private void OnNormalPass(Collider other)
	{
		if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag(Tags.Ins.Player)) return;
		if(other.attachedRigidbody.GetComponent<BikeControl>() == null) return;
		GameModeTiming.Ins.PassTimeCheckPoint(this);
		GameModeTiming.Ins.RecoverTime(this, RecoverTime);
	}

	private void OnPerfectPass(Collider other) {
		if (other.attachedRigidbody == null || !other.attachedRigidbody.CompareTag(Tags.Ins.Player)) return;
		if (other.attachedRigidbody.GetComponent<BikeControl>() == null) return;
		PerfectReward = Mathf.Clamp01(PerfectReward);
		GameModeTiming.Ins.RecoverTime(this, RecoverTime, PerfectReward);
	}
}
