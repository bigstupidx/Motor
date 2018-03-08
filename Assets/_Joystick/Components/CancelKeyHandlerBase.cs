using System.Collections;
using UnityEngine;

namespace Joystick {
	public abstract class CancelKeyHandlerBase : MonoBehaviour {
		public FocusRoot Root;

		protected virtual void OnEnable() {
			if (FocusManager.TVMode) {
				StartCoroutine(Register());
			}
		}
		protected virtual void OnDisable() {
			if (Root != null) {
				Root.UnRegisterCancelKey(Root, this);
			}
		}

		protected virtual void Reset() {
		}

		IEnumerator Register() {
			//为了防止某些时候由于界面的延迟，导致root找不到，这里延迟一会
			yield return null;
			FindRoot();
			Root.RegisterCancelKey(Root, this);
		}

		/// <summary>
		/// 获取聚焦项根节点
		/// </summary>
		protected virtual void FindRoot() {
			this.Root = transform.GetComponentInParent<FocusRoot>();
		}

		public virtual void OnCancel() {
		}

	}
}