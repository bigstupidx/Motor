using UnityEngine;
using System.Collections;
using XUI;

namespace GameUI {
	public class ModHUD : UIPool {

		public static ModHUD Ins { get; private set; }

		protected override void Awake() {
			base.Awake();
			Ins = this;
		}

		protected virtual void OnDestroy() {
			Ins = null;
		}

	}

}
