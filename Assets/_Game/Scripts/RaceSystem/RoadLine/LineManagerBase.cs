using System.Collections.Generic;
using UnityEngine;
using XPlugin.Update;

namespace Game {
	public class LineManagerBase<T, S> : MonoBehaviour where T : MonoBehaviour where S : LineManagerBase<T, S> {
		protected static S _instance;

		public static S Ins {
			get {
				return _instance;
			}
		}

		protected virtual void Awake() {
			_instance = (S)this;
		}

		protected virtual void OnDestroy() {
			_instance = null;
		}

		public T Current { get; private set; }

		public virtual T SpawnLine(string line) {
			var prefab = UResources.Load<GameObject>(line);
			if (prefab == null) {
				Debug.LogError("试图加载不存在的线路" + line);

			}
			var ins = (GameObject)Instantiate(prefab, transform, false);
			this.Current = ins.GetComponent<T>();
			return this.Current;
		}
	}
}