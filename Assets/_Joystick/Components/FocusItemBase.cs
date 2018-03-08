using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joystick {

	/// <summary>
	/// 聚焦项基类
	/// </summary>
	public abstract class FocusItemBase : MonoBehaviour {

		[RenameAttr("强制优先聚焦")]
		[Tooltip("无视已经选中的聚焦项")]
		public bool ForceFirstFocus = false;       //强制优先获得聚焦
												   //		[System.NonSerialized]
		[RenameAttr("优先聚焦")]
		public bool FirstFocus = false;       //优先获得聚焦
		[System.NonSerialized]
		public FocusRoot Root;            //根节点

		public bool useLerpEffect = true;     //聚焦到该item小手是否使用插值动画

		//上下左右相邻的聚焦项目及其禁用状态
		public bool DisableLeft = false;
		public FocusItemBase Left;
		public bool DisableRight = false;
		public FocusItemBase Right;
		public bool DisableUp = false;
		public FocusItemBase Up;
		public bool DisableDown = false;
		public FocusItemBase Down;

		public Vector2 Offset;

		public float DisWeight = 1;
		public float AngleWeight = 1;
		public float Dot = 0.2f;


		protected virtual void Awake() {
		}

		protected virtual void OnEnable() {
			if (FocusManager.TVMode) {
				StartCoroutine(Register());
			}
		}
		protected virtual void OnDisable() {
			if (Root != null) {
				Root.UnRegister(Root, this);
			}
		}

		protected virtual void Reset() {
		}

		/// <summary>
		/// 注册节点
		/// </summary>
		/// <returns></returns>
		IEnumerator Register() {
			//为了防止某些时候由于界面的延迟，导致root找不到，这里延迟一会
			yield return null;
			FindRoot();
			Root.Register(Root, this);
		}

		/// <summary>
		/// 获取聚焦项根节点
		/// </summary>
		protected virtual void FindRoot() {
			this.Root = transform.GetComponentInParent<FocusRoot>();
		}

		/// <summary>
		/// 判断聚焦项是否可聚焦
		/// </summary>
		public virtual bool IsActive {
			get {
				return gameObject.activeInHierarchy;
			}
		}

		/// <summary>
		/// 获取指定方向的相邻的聚焦项目
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public virtual FocusItemBase Get(DirType dir, List<FocusItemBase> list) {
			var ret = this;
			switch (dir) {
				case DirType.Left:
					if (this.DisableLeft) {
						return this;
					}
					if (this.Left != null && this.Left.IsActive) {
						//释放优先聚焦权
						FirstFocus = false;
						return this.Left;
					}
					ret = Search(Vector3.left, list) ?? this;
					break;

				case DirType.Right:
					if (this.DisableRight) {
						return this;
					}
					if (this.Right != null && this.Right.IsActive) {
						//释放优先聚焦权
						FirstFocus = false;
						return this.Right;
					}
					ret = Search(Vector3.right, list) ?? this;
					break;

				case DirType.Up:
					if (this.DisableUp) {
						return this;
					}
					if (this.Up != null && this.Up.IsActive) {
						//释放优先聚焦权
						FirstFocus = false;
						return this.Up;
					}
					ret = Search(Vector3.up, list) ?? this;
					break;

				case DirType.Down:
					if (this.DisableDown) {
						return this;
					}
					if (this.Down != null && this.Down.IsActive) {
						//释放优先聚焦权
						FirstFocus = false;
						return this.Down;
					}
					ret = Search(Vector3.down, list) ?? this;
					break;
			}
			//释放优先聚焦权
			FirstFocus = false;
			return ret;
		}



		/// <summary>
		///       计算并选取最佳相邻聚焦项目
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		protected virtual FocusItemBase Search(Vector3 dir, List<FocusItemBase> list) {
			Vector3 myScreenPos = GetScreenPos();
			float weightMax = 0;
			//计算的最佳项目
			FocusItemBase ret = null;
			for (int i = 0; i < list.Count; ++i) {
				var item = list[i];
				//跳过自己本身和不活跃的
				if (item == this || !item.IsActive) {
					continue;
				}

				Vector3 navCenter = item.GetScreenPos();
				//目标对象与起点的方向向量
				Vector3 targetDir = navCenter - myScreenPos;
				//单位向量点积
				float dot = Vector3.Dot(dir, targetDir.normalized);
				//大于一定角度度跳过
				if (dot <= Dot) {
					continue;
				}

				float mag = targetDir.magnitude;
				if (mag == 0f) {
					Debug.LogError("找到完全重合的点，跳过" + item.name);
					continue;
				}

				float dis_weight = 1f / mag * this.DisWeight;
				float angle_weight = dot * this.AngleWeight;
				float total_weight = dis_weight + angle_weight;
#if UNITY_EDITOR
				string path = UnityEditor.AnimationUtility.CalculateTransformPath(item.transform,null);
				Debug.LogFormat("{0} , dis_weight={1} angle_weight={2} total={3}",path,dis_weight,angle_weight,total_weight);
#endif
				if (dot > 0.99f) {//如果角度极为一致
								  //判断是否属于同一组物体
					if (this.transform.parent == item.transform.parent) {
						total_weight += 1000f;
					}
				}
				if (total_weight > weightMax) {
					weightMax = total_weight;
					ret = item;
				}
			}
			return ret;
		}

		/// <summary>
		/// 被聚焦
		/// </summary>
		public virtual void OnFocused(FocusItemBase lastFocus) {
		}

		public virtual void OnLostFocuse(FocusItemBase newFocus) {
		}

		/// <summary>
		/// 被点击
		/// </summary>
		public virtual void OnConfirmDown() {
			//这样可以使主页面被点击的按钮优先聚焦
			FirstFocus = true;
		}

		public virtual void OnConfirmUp() {
		}

		public virtual Vector3 GetScreenPos() {
			return transform.position;
		}

		public virtual Vector3 GetFocusEffectPos() {
			return GetScreenPos();
		}


	}
}
