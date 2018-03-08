using System;
using System.Collections.Generic;
using UnityEngine;

namespace Joystick {

	/// <summary>
	/// 摇杆和tv控制器的模拟
	/// </summary>
	public class JoystickInput : Singleton<JoystickInput> {

		public bool UseArrowKey = true;

		//上下左右确认返回菜单事件代理
		public event Action OnLeft = delegate { };
		public event Action OnRight = delegate { };
		public event Action OnUp = delegate { };
		public event Action OnDown = delegate { };
		public event Action OnConfirmDown = delegate { };
		public event Action OnConfirmUp = delegate { };
		public event Action OnBack = delegate { };
		public event Action OnMenu = delegate { };

		public List<KeyCode> ConfirmKeyCodes = new List<KeyCode>() {
			KeyCode.Return,
			KeyCode.Joystick1Button0,
			(KeyCode)10,//小米遥控器
		};
		public List<KeyCode> CancelKeyCodes = new List<KeyCode>() {
			KeyCode.Escape,
			KeyCode.Pause,
			KeyCode.JoystickButton11,
			KeyCode.JoystickButton1,
		};

		public List<KeyCode> MenuKeyCodes = new List<KeyCode>() {
			KeyCode.Menu,
			KeyCode.RightControl,
		};

		public bool IsConfirmDown {
			get {
				for (var i = 0; i < this.ConfirmKeyCodes.Count; i++) {
					var key = this.ConfirmKeyCodes[i];
					if (Input.GetKeyDown(key)) {
						return true;
					}
				}
				return false;
			}
		}

		public bool IsConfirmUp {
			get {
				for (var i = 0; i < this.ConfirmKeyCodes.Count; i++) {
					var key = this.ConfirmKeyCodes[i];
					if (Input.GetKeyUp(key)) {
						return true;
					}
				}
				return false;
			}
		}

		public bool IsCancelDown {
			get {
				for (var i = 0; i < this.CancelKeyCodes.Count; i++) {
					var key = this.CancelKeyCodes[i];
					if (Input.GetKeyDown(key)) {
						return true;
					}
				}
				return false;
			}
		}

		public bool IsMenuDown {
			get {
				for (var i = 0; i < this.MenuKeyCodes.Count; i++) {
					var key = this.MenuKeyCodes[i];
					if (Input.GetKeyDown(key)) {
						return true;
					}
				}
				return false;
			}
		}


		private bool x1;
		private bool x_1;
		private bool y1;
		private bool y_1;

		//输入检测
		void Update() {
			if (!UseArrowKey) {
				var x = Input.GetAxis("Horizontal");
				var y = Input.GetAxis("Vertical");
				if (x >= 0.1) {
					if (!x1) {
						x1 = true;
						this.OnRight();
					}
				} else {
					x1 = false;
				}
				if (y >= 0.1) {
					if (!this.y1) {
						this.y1 = true;
						this.OnUp();
					}
				} else {
					this.y1 = false;
				}

				if (x <= -0.1) {
					if (!this.x_1) {
						this.x_1 = true;
						this.OnLeft();
					}
				} else {
					this.x_1 = false;
				}

				if (y <= -0.1) {
					if (!this.y_1) {
						this.y_1 = true;
						this.OnDown();
					}
				} else {
					this.y_1 = false;
				}
			} else {
				if (Input.GetKeyDown(KeyCode.UpArrow)) {
					this.OnUp();
				} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
					this.OnLeft();
				} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
					this.OnRight();
				} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
					this.OnDown();
				}
			}


			if (IsConfirmDown) {
				this.OnConfirmDown();
			}
			if (IsConfirmUp) {
				this.OnConfirmUp();
			}
			if (IsCancelDown) {
				this.OnBack();
			}
			if (IsMenuDown) {
				this.OnMenu();
			}

		}




	}
}
