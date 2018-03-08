//
// TweenScaleXyz.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;

public class TweenScaleXyz : MonoBehaviour {
	public Vector3 from = Vector3.zero;
	public Vector3 to = Vector3.one;
	public float duration = 1f;
	public AnimationCurve animationCurveX = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public AnimationCurve animationCurveY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public AnimationCurve animationCurveZ = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public Vector3 value { get; private set; }
	public bool isFinished { get; private set; }
	public Transform target { get; private set; }
	public bool IsFixedUpdate = false;
	private float _timer;

	void Start ()
	{
		isFinished = true;
	}

	void Update() {
		if (!isFinished && !IsFixedUpdate)
		{
			_timer += Time.deltaTime;
			Caculate();
		}
	}

	void FixedUpdate () {
		if (!isFinished && IsFixedUpdate)
		{
			_timer += Time.fixedDeltaTime;
			Caculate();
		}
	}

	public void Caculate()
	{
		if (_timer >= duration)
		{
			Stop();
		}
		var val = _timer / duration;
		var factor = animationCurveX.Evaluate(val);
		var x = (from.x * (1 - factor) + to.x * factor);
		factor = animationCurveY.Evaluate(val);
		var y = (from.y * (1 - factor) + to.y * factor);
		factor = animationCurveZ.Evaluate(val);
		var z = (from.z * (1 - factor) + to.z * factor);
		value = new Vector3(x, y, z);
		target.localScale = value;
	}

	public void Play(Transform go)	
	{
		target = go;

		isFinished = false;
		_timer = 0;
		value = this.from;
	}

	public void Stop()
	{
		isFinished = true;
		value = to;
	}
}
