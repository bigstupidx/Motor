//
// LTHLuminanceHolder.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;

namespace Game {
	public class LTHLuminanceHolder : MonoBehaviour {
		public static LTHLuminanceHolder ins;

		private AnimationCurve curve;
		private float time;
		private float timer;
		private bool disableAfterAnim;


		void Awake() {
			ins = this;
			enabled = false;
		}

		public void StartAnim(AnimationCurve curve, float time, bool disableAfterAnim) {
			this.curve = curve;
			this.time = time;
			timer = 0f;
			enabled = true;
			this.disableAfterAnim = disableAfterAnim;
			Luminance.ins.enabled = true;
		}

		void Update() {
			timer += Time.deltaTime;
			Luminance.ins.saturation = curve.Evaluate(timer / time);
			if (timer >= time) {
				if (disableAfterAnim) {
					Luminance.ins.enabled = false;
				}
				enabled = false;
			}

		}

	}
}
