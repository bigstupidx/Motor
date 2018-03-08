//
// TrialsRotateItem.cs
//
// Author:
// [WeiHuajian]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//namespace GameUI{
//	public class TrialsRotateItem : RotateItem
//	{
//		public RawImage sprDark, sprIcon;
//		public Canvas canItem;
//		public GraphicRaycaster rayItem;
//		public float minAlpha = 0.4f, maxAlpha = 1f;
//		public float minScale = 0.8f, maxScale = 1.2f;
//		public int minDepth = 0;
//		public UIEffectDialog effect;
//		public UIEffect_Particle_Load particle;
//		public TweenPosition twePos;
//
//		void OnEnable() {
//			if (effect != null)
//			{
//				effect.onDone += Init;
//			}
//			else
//			{
//				Invoke("Init", 0.01f);
//			}
//		}
//
//		void OnDisable() {
//			if (effect != null)
//			{
//				effect.onDone -= Init;
//			}
//			Reset();
//		}
//
//		public override void Init() {
//			base.Init();
//			SetFront(IsFront);
//		}
//
//		public override void Reset()
//		{
//			base.Reset();
//		}
//
//		public override void SetFront(bool isFront)
//		{
//			base.SetFront(isFront);
//			rayItem.enabled = isFront;
//			if (particle != null) {
//				particle.Set(isFront);
//			}
//			if (twePos != null)
//			{
//				if (isFront)
//				{
//					twePos.style = UITweener.Style.PingPong;
//					twePos.PlayForward();
//				}
//				else
//				{
//					twePos.style = UITweener.Style.Once;
//					twePos.PlayReverse();
//				}
//			}
//		}
//
//		void LateUpdate()
//		{
//			float _z = (posZ + 1f) / 2f;
//			float alpha = Mathf.Lerp(minAlpha, maxAlpha, 1f - _z);
//			if(sprDark != null) {
//				sprDark.color = new Color(sprDark.color.r, sprDark.color.g, sprDark.color.b, alpha);
//			}
//			this.transform.localScale = Mathf.Lerp(minScale, maxScale, _z) * Vector3.one;
//			int _d = minDepth;
//			foreach (TrialsRotateItem item in rList)
//			{
// 				if(item != this){
//					if (item.posZ < this.posZ) {
//						_d+=2;
//					}
//				}
//			}
//			canItem.sortingOrder = _d;
//
//			this.transform.rotation = Quaternion.identity;
//		}
//	}
//}
