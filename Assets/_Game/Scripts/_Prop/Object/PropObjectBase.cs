//
// PropObjectBase.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System;
using GameClient;
using UnityEngine;

namespace Game {
	public class PropObjectBase : MonoBehaviour {
		//		[NonSerialized]
		public BikeBase User;
		//		[NonSerialized]
		public BikeBase Target;
		[NonSerialized]
		public Transform Renderer;
		[NonSerialized]
		public CapsuleCollider Collider;

		public float _time;
		private float _timer;

		private bool _isAffect = false;

		public virtual PropType Type {
			get { return PropType.None; }
		}

		void Start() {
			_timer = 0;
			_isAffect = true;
			Renderer = transform.FindInAllChild("Renderer");
			Collider = GetComponent<CapsuleCollider>();
			OnStart();
		}

		void Update() {
			if (!_isAffect) {
				return;
			}
			_timer += Time.deltaTime;
			if (_timer >= _time && _time > 0) {
				OnStop();
			}
			OnUpdate();
		}

		void OnTriggerEnter(Collider other) {
			if (other.attachedRigidbody != null) {
				var bike = other.attachedRigidbody.GetComponent<BikeBase>();
				if (bike != null && bike.bikeHealth.IsAlive) {
					OnCatch(bike);
				}
			}

		}

		public virtual void OnCatch(BikeBase bike) {
		}

		public virtual void OnStart() {
		}

		public virtual void OnUpdate() {
		}

		public virtual void OnStop() {
			_isAffect = false;
			if (Renderer != null) Renderer.gameObject.SetActive(false);
			if (Collider != null) Collider.enabled = false;
			// 等待特效放完后销毁
			this.DelayInvoke(() => { Destroy(gameObject); }, 2f);
		}
	}
}

