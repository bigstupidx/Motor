using System.Collections.Generic;
using UnityEngine;

namespace Joystick {
	public class FocusRoot : MonoBehaviour {

		public int ComputPriorityLevel = 0;
		public FocusManager Manager;
		//		[System.NonSerialized]
		public List<FocusItemBase> Items = new List<FocusItemBase>();
		//		[System.NonSerialized]
		public List<CancelKeyHandlerBase> CancelHandlers = new List<CancelKeyHandlerBase>();
		//		[System.NonSerialized]
		public List<MenuKeyHandlerBase> MenuKeyHandlers = new List<MenuKeyHandlerBase>();


		void Awake() {
			if (Manager == null) {
				if (FocusManager.TVMode) {
					Manager = FocusManager.Ins;
				}
			}
		}

		void OnEnable() {
			if (Manager != null) {
				Manager.Containers.Add(this);
			}
		}

		void OnDisable() {
			if (Manager != null) {
				Manager.Containers.Remove(this);
			}
		}

		/// <summary>
		/// 注册聚焦项
		/// </summary>
		public void Register(FocusRoot root, FocusItemBase item) {
			this.Items.Add(item);
			FocusManager.Ins.OnUpdateList();
		}

		/// <summary>
		/// 解除聚焦项的注册
		/// </summary>
		public void UnRegister(FocusRoot root, FocusItemBase item) {
			this.Items.Remove(item);
			FocusManager.Ins.OnUpdateList();
		}

		public void RegisterCancelKey(FocusRoot root, CancelKeyHandlerBase item) {
			this.CancelHandlers.Add(item);
		}

		public void UnRegisterCancelKey(FocusRoot root, CancelKeyHandlerBase item) {
			this.CancelHandlers.Remove(item);
		}

		public void UnRegisterMenuKey(FocusRoot root, MenuKeyHandlerBase item) {
			this.MenuKeyHandlers.Add(item);
		}

		public void RegisterMenuKey(FocusRoot root, MenuKeyHandlerBase item) {
			this.MenuKeyHandlers.Remove(item);
		}
	}
}