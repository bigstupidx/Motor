//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using GameClient;
using UnityEngine;

namespace Game {
	public enum AbilityType {
		Menu,
		Game
	}

	public class AbilityBase : MonoBehaviour {
		public virtual AbilityType Type {
			get { return AbilityType.Game; }
		}

		public float Value { get; private set; }

		private bool _isActive = false;

		public virtual void Init(float value) {
			Value = value;
		}

		public virtual void SetActive(PlayerInfo target, bool active) {
			if (active) Enable(target);
			else Disable(target);
		}

		public virtual void Enable(PlayerInfo target) {
			if (_isActive) return;
			_isActive = true;
		}

		public virtual void Disable(PlayerInfo target) {
			if (!_isActive) return;
			_isActive = false;
		}
	}

}
