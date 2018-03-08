using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

namespace Joystick {
	public enum DirType {
		Left, Right, Up, Down
	}

	public class FocusManager : Singleton<FocusManager> {

		public FocusItemBase CurrentFocusItem;
		public bool ShowDebug;
		public FocusEffect FocusEffect;
		public List<FocusRoot> Containers = new List<FocusRoot>();

		private bool _needUpdate = false;

		public static bool TVMode {
			get { return FocusManager.Ins != null && FocusManager.Ins.enabled; }
		}

		void Start() {
			enabled = Client.Config.TVMode;
			//开始关闭光标特效的显示
			this.FocusEffect.gameObject.SetActive(false);
			if (!this.enabled) {
				return;
			}

			//点击事件处理
			//选中点击确认
			JoystickInput.Ins.OnConfirmDown += () => {
				if (CurrentFocusItem != null) {
					this.CurrentFocusItem.OnConfirmDown();
				}
			};
			JoystickInput.Ins.OnConfirmUp += () => {
				if (CurrentFocusItem != null) {
					this.CurrentFocusItem.OnConfirmUp();
				}
			};

			//返回
			JoystickInput.Ins.OnBack += () => {
				var root = this.Containers[this.Containers.Count - 1];
				if (root.CancelHandlers.Count > 0) {
					root.CancelHandlers[root.CancelHandlers.Count - 1].OnCancel();
				}
			};

			//菜单键按下
			JoystickInput.Ins.OnMenu += () => {
				var root = this.Containers[this.Containers.Count - 1];
				if (root.MenuKeyHandlers.Count > 0) {
					root.MenuKeyHandlers[root.MenuKeyHandlers.Count - 1].OnMenuKey();
				}
			};

			//左
			JoystickInput.Ins.OnLeft += () => {
				if (CurrentFocusItem != null) {
					//只筛选和当前聚焦按钮在同一个父节点下的物体
					var root = this.Containers.Find(container => container == this.CurrentFocusItem.Root);
					//聚焦到合适的按钮
					this.Focus(CurrentFocusItem.Get(DirType.Left, root.Items));
				} else {
					OnUpdateList();
				}
			};

			//右
			JoystickInput.Ins.OnRight += () => {
				if (CurrentFocusItem != null) {
					var root = this.Containers.Find(container => container == CurrentFocusItem.Root);
					this.Focus(CurrentFocusItem.Get(DirType.Right, root.Items));
				} else {
					OnUpdateList();
				}
			};

			//上
			JoystickInput.Ins.OnUp += () => {
				if (CurrentFocusItem != null) {
					//只筛选和当前聚焦按钮在同一个父节点下的物体
					var root = this.Containers.Find(container => container == this.CurrentFocusItem.Root);
					//聚焦到合适的按钮
					this.Focus(CurrentFocusItem.Get(DirType.Up, root.Items));
				} else {
					OnUpdateList();
				}
			};

			//下
			JoystickInput.Ins.OnDown += () => {
				if (CurrentFocusItem != null) {
					//只筛选和当前聚焦按钮在同一个父节点下的物体
					var root = this.Containers.Find(container => container == this.CurrentFocusItem.Root);
					//聚焦到合适的按钮
					this.Focus(CurrentFocusItem.Get(DirType.Down, root.Items));
				} else {
					OnUpdateList();
				}
			};


		}

#if UNITY_EDITOR
		void OnGUI() {
			if (this.ShowDebug) {
				if (CurrentFocusItem != null) {
					var path = UnityEditor.AnimationUtility.CalculateTransformPath(CurrentFocusItem.transform, null);
					if (GUILayout.Button("CurrentFocus:" + path)) {
						UnityEditor.EditorGUIUtility.PingObject(CurrentFocusItem.gameObject);
					}
				}
			}
		}
#endif

		void LateUpdate() {
			if (!this._needUpdate) {
				return;
			}
			this._needUpdate = false;
			FocusItemBase result = null;
			for (var i = this.Containers.Count - 1; i >= 0; i--) {
				var root = this.Containers[i];
				root.Items.RemoveAll(item => item == null);
				if (root.Items.Count > 0) {
					//先找强制优先聚焦的项
					foreach (var item in root.Items) {
						if (item.ForceFirstFocus) {
							result = item;
							break;
						}
					}
					if (result == null) {
						if (root.Items.Contains(this.CurrentFocusItem)) {
							//如果找到当前聚焦的点，则优先聚焦它
							result = this.CurrentFocusItem;
							break;
						} else {
							result = root.Items[0]; //默认选取第一个
							foreach (var item in root.Items) {
								if (item.FirstFocus) {
									//如果设置了优先选取则选中优先选取
									result = item;
									break;
								}
							}
						}
					}
					break;
				}
			}

			if (result != null) {
				result.useLerpEffect = false;
			}
			Focus(result);
		}

		public void FocusDelayed(FocusItemBase item) {
			StartCoroutine(Coro_FocusDelayed(item));
		}

		IEnumerator Coro_FocusDelayed(FocusItemBase item) {
			yield return 0;
			yield return 0;
			yield return 0;
			yield return 0;
			this.Focus(item);
		}

		/// <summary>
		/// 聚焦到指定项目
		/// </summary>
		public void Focus(FocusItemBase item) {
			var lastFocus = this.CurrentFocusItem;
			if (CurrentFocusItem == item) {
				return;
			}

			//通知聚焦项失去聚焦
			if (this.CurrentFocusItem != null) {
				//如果失去焦点的item在栈顶则出栈
				//  if (CurrentFocusableItem == StackItems.Peek())
				//  {
				//      StackItems.Pop();
				//  }
				this.CurrentFocusItem.OnLostFocuse(item);
			}

			CurrentFocusItem = item;

			//通知聚焦项已经被聚焦
			if (CurrentFocusItem != null) {
				CurrentFocusItem.OnFocused(lastFocus);
			}

			//设置手指的更新显示
			if (CurrentFocusItem == null) {
				this.FocusEffect.gameObject.SetActive(false);
				return;
			} else {
				this.FocusEffect.Focus(CurrentFocusItem);
			}
		}

		/// <summary>
		/// 更新聚焦列表
		/// </summary>
		public void OnUpdateList() {
			this._needUpdate = true;
		}


	}
}
