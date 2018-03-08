//
// ModEventManager.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameClient {
	public class ModEventManager : ClientModule {

		public Game_Boosting GameBoosting = delegate { };
		public Game_Drifting GameDrifting = delegate { };
		public Game_Boost GameBoost= delegate { };
		public Game_Drift GameDrift = delegate { };


		protected Dictionary<EventEnum, Action<EventEnum, object[]>> systemListener = new Dictionary<EventEnum, Action<EventEnum, object[]>>();
		protected Dictionary<EventEnum, Action<EventEnum, object[]>> normalListener = new Dictionary<EventEnum, Action<EventEnum, object[]>>();

		/// <summary>
		/// 注册系统级事件监听（注销时不会被清空，需要自行移除）
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="listener"></param>
		public void AddSysListener(EventEnum eventType, Action<EventEnum, object[]> listener) {
			if (!systemListener.ContainsKey(eventType)) {
				systemListener.Add(eventType, listener);
			} else {
				systemListener[eventType] += listener;
			}
		}

		/// <summary>
		/// 移除系统级事件监听
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="listener"></param>
		public void RemoveSysListener(EventEnum eventType, Action<EventEnum, object[]> listener) {
			if (systemListener.ContainsKey(eventType)) {
				systemListener[eventType] -= listener;
			}
		}

		/// <summary>
		/// 注册事件监听（注销时会被清空）
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="listener"></param>
		public void AddListener(EventEnum eventType, Action<EventEnum, object[]> listener) {
			if (!normalListener.ContainsKey(eventType)) {
				normalListener.Add(eventType, listener);
			} else {
				normalListener[eventType] += listener;
			}
		}

		/// <summary>
		/// 移除事件监听
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="listener"></param>
		public void RemoveListener(EventEnum eventType, Action<EventEnum, object[]> listener) {
			if (normalListener.ContainsKey(eventType)) {
				normalListener[eventType] -= listener;
			}
		}

		/// <summary>
		/// 发送事件
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="args"></param>
		public void SendEvent(EventEnum eventType, params object[] args) {
			if (eventType == EventEnum.None || eventType == EventEnum.AllEvent) {
				return;
			}

			InvokeListener(systemListener, eventType, eventType, args);
			InvokeListener(systemListener, EventEnum.AllEvent, eventType, args);

			InvokeListener(normalListener, eventType, eventType, args);
			InvokeListener(normalListener, EventEnum.AllEvent, eventType, args);
		}

		protected void InvokeListener(Dictionary<EventEnum, Action<EventEnum, object[]>> list, EventEnum listenerType, EventEnum eventType, object[] args) {
			Action<EventEnum, object[]> act;
			list.TryGetValue(listenerType, out act);
			if (act != null) {
				try {
					act(eventType, args);
				} catch (Exception e) {
					Debug.LogException(e);
				}
			}
		}

		public override void ResetData() {
			normalListener.Clear();
		}
	}
}