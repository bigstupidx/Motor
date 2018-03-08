//
// HeroIcon.cs
//
// Author:
// [longtianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using GameUI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
	public class HUDText : MonoBehaviour {
		public UIFollowTarget followTargetCom;
		public Text NameText;
		public Text RankText;
		public Image RankArrow;
		public Transform Rank;
		public List<Sprite> RankArrowSprite;

		private int _preRank = -1;

		void OnValidate() {
			if (followTargetCom == null) {
				followTargetCom = GetComponent<UIFollowTarget>();
			}
			if (NameText == null) {
				NameText = GetComponent<Text>();
			}
		}

		public void SetNameActive(bool active) {
			NameText.gameObject.SetActive(active);
		}

		public void SetRankActive(bool active) {
			Rank.gameObject.SetActive(active);
		}

		public void SetRankArrow(bool isSelf) {
			if (isSelf) {
				RankArrow.sprite = RankArrowSprite[0];
			} else {
				RankArrow.sprite = RankArrowSprite[1];
			}
		}

		public void SetRank(int rank) {
			if (_preRank != rank) {
				_preRank = rank;
				RankText.text = rank.ToString();
			}
		}

		public void OnHUDTextSpawned(Transform target, string text) {
			if (followTargetCom != null) {
				followTargetCom.target = target;
			}
//			UIFollowTarget.SetToUIPos(target, transform);
			NameText.text = text;
		}

		public static HUDText Spawn(GameObject hud, Transform target, string text) {
			HUDText hudText = ModHUD.Ins.Spawn(hud).GetComponent<HUDText>();
			hudText.OnHUDTextSpawned(target, text);
			return hudText;
		}

	}
}