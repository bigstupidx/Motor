using System.Collections.Generic;
using GameClient;
using XUI;

namespace GameUI {

	public class UIModWithEvent<T> : UIModWithEvent where T : UIModWithEvent<T> {
		protected static T _instance;

		public static T Ins {
			get { return _instance; }
		}

		protected override void Awake() {
			base.Awake();
			_instance = (T)this;
		}

		protected virtual void OnDestroy() {
			_instance = null;
		}
	}

	public class UIModWithEvent : UIMod {

		public UIGroup Cover(IEnumerable<string> names, string uiName = "", bool destroyBefore = false) {
			var ret = base.Cover(names, destroyBefore);
			SendEvent(uiName);
			return ret;
		}

		public UIGroup Overlay(IEnumerable<string> names, string uiName = "") {
			var ret = base.Overlay(names);
			SendEvent(uiName);
			return ret;
		}

		private void SendEvent(string uiName) {
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, uiName);
		}

	}
}