using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityExtension {



	public static int IndexOf<T>(this T[] list, T value) {
		if (list == null || list.Length == 0) return -1;
		for (var i = 0; i < list.Length; i++) {
			if (list[i].Equals(value)) return i;
		}
		return -1;
	}

	public static TValue Random<TKey, TValue>(this Dictionary<TKey, TValue> dic) {
		var index = UnityEngine.Random.Range(0, dic.Count);
		var i = 0;
		var result = default(TValue);
		foreach (var value in dic.Values) {
			i++;
			if (i < index) continue;
			result = value;
			break;
		}
		return result;
	}

	public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value) {
		if (dic.ContainsKey(key)) {
			dic[key] = value;
		} else {
			dic.Add(key, value);
		}
	}

	public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key) {
		if (dic.ContainsKey(key)) {
			return dic[key];
		}
		return default(TValue);
	}

	public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value) {
		if (!dic.ContainsKey(key)) {
			dic.Add(key, value);
		}
	}

	public static void AddNotRepeat<T>(this List<T> list, T value) {
		if (!list.Contains(value)) list.Add(value);
	}

	public static T Random<T>(this List<T> list) {
		if (list.Count < 1) return default(T);
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T Has<T>(this IEnumerable<T> source, Predicate<T> predicate) {
		foreach (var item in source) {
			if (predicate(item)) {
				return item;
			}
		}
		return default(T);
	}

	#region Monobehaviour Extenstion
	public delegate bool When();

	/// <summary>
	/// 当条件满足返回true时，执行Action
	/// </summary>
	/// <param name="monoBehaviour">MonoBehaviour</param>
	/// <param name="action">事件</param>
	/// <param name="condition">条件</param>
	/// <returns>结果</returns>
	public static Coroutine ExecuteWhen(this MonoBehaviour monoBehaviour, Action action, When condition) {
		return monoBehaviour.StartCoroutine(ExecuteWhenCoroutine(action, condition));
	}

	private static IEnumerator ExecuteWhenCoroutine(Action action, When condition) {
		while (condition != null && !condition())
			yield return null;
		action();
	}
	#endregion
}


