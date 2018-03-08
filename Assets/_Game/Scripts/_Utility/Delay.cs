using System;
using System.Collections;
using UnityEngine;

public static class Delay {
	/// <summary>
	/// 延迟到帧结束时执行
	/// </summary>
	/// <param name="host"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	public static Coroutine DelayEndOfFrame(this MonoBehaviour host, Action action) {
		return host.StartCoroutine(_delayEndOfFrame(action));
	}

	private static IEnumerator _delayEndOfFrame(Action action) {
		yield return new WaitForEndOfFrame();
		action();
	}


	/// <summary>
	/// 延迟指定时间指定某个方法
	/// </summary>
	/// <param name="action">方法</param>
	/// <param name="delaySeconds">时间</param>
	/// <param name="timeScale"></param>
	public static Coroutine DelayInvoke(this MonoBehaviour host, Action action, float delaySeconds, bool timeScale = false) {
		return host.StartCoroutine(_delay(action, delaySeconds, timeScale));
	}

	/// <summary>
	/// 延时执行
	/// </summary>
	/// <param name="action">方法</param>
	/// <param name="delaySeconds">时间</param>
	/// <param name="timeScale"></param>
	/// <returns></returns>
	private static IEnumerator _delay(Action action, float delaySeconds, bool timeScale = false) {
		if (!timeScale) {
			yield return new WaitForSecondsRealtime(delaySeconds);
		} else {
			yield return new WaitForSeconds(delaySeconds);
		}
		action();
	}
}
