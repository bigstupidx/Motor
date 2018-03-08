//
// BuffBlink.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using System.Collections.Generic;
using LTHUtility;

namespace Game {
	/// <summary>
	/// BUFF - 闪烁
	/// </summary>
	public class BuffBlink : BuffBaseWithEffect {
		private List<Renderer> _renderers;

		private float _timer;
		private float _interval = 0.1f;
		private bool _enable;

		public bool ReStart(float buffTime) {
			_renderers = null;
			return Start(buffTime);
		}

		public override bool Start(float buffTime) {
			if (isAffect) {
				time = buffTime;
				return true;
			}

			var result = base.Start(buffTime);

			if (_renderers == null || _renderers.Count == 0) {
				var rs = bike.transform.GetComponentsInChildren<Renderer>();
				_renderers = new List<Renderer>();
				foreach (var r in rs) {
					if (r.enabled) {
						_renderers.Add(r);
					}
				}
			}

			_timer = 0;
			bike.gameObject.SetLayerRecursion(Layers.Ins.Blink);
			return result;
		}

		public override void UpdateWhenAffect() {
			base.UpdateWhenAffect();
			_timer += Time.deltaTime;
			if (_timer >= _interval) {
				_timer -= _interval;
				_enable = !_enable;
				SetRendererActive(_enable);
			}
		}

		private void SetRendererActive(bool enable) {
			if (_renderers == null || _renderers.Count == 0) {
				return;
			}
			for (var index = 0; index < _renderers.Count; index++) {
				var r = _renderers[index];
				if (r != null)
					r.enabled = enable;
			}
		}

		public override void OnBuffStop() {
			SetRendererActive(true);
			if (bike.gameObject.CompareTag(Tags.Ins.Player)) {
				bike.gameObject.SetLayerRecursion(Layers.Ins.Player);
			} else {
				bike.gameObject.SetLayerRecursion(Layers.Ins.Enemy);
			}

			base.OnBuffStop();
		}
	}

}

