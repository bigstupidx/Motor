using UnityEngine;

namespace Joystick {
	/// <summary>
	/// 当该组件出现的时候，聚焦效果将隐藏，适用于教程中已经有聚焦效果的情况
	/// </summary>
	public class DisableFocusEffect : MonoBehaviour {

		public static DisableFocusEffect Ins { private set; get; }

		void OnEnable() {
			Ins = this;
		}

		void OnDisable() {
			Ins = null;
		}

	}
}