using System.Collections.Generic;
using UnityEngine;
using XPlugin.Localization;

namespace XUI {

	public class UIMod<T> : UIMod where T : UIMod<T> {
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

	public class UIMod : UIStack {
		public string Current { get; private set; }

		public virtual GameObject Spawn(string name) {
			return Spawn(GetPrefab(name));
		}

		public virtual UIGroup Cover(IEnumerable<string> names, bool destroyBefore = false) {
			List<GameObject> group = new List<GameObject>();
			foreach (string t in names) {
				@group.Add(GetPrefab(t));
			}
			var result = Cover(group, destroyBefore);
			return result;
		}


		public virtual UIGroup Overlay(IEnumerable<string> names) {
			List<GameObject> group = new List<GameObject>();
			foreach (string t in names) {
				@group.Add(GetPrefab(t));
			}
			var result = Overlay(group);
			return result;
		}

		public static GameObject GetPrefab(string name) {
			return LResources.Load<GameObject>(name);
		}
	}
}