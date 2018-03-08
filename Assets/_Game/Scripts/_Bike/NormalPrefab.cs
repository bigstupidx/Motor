using UnityEngine;
using XPlugin.Update;

[System.Serializable]
public class NormalPrefab {//TODO 继承normalasset

	public string ResourcePath;
	public GameObject _prefab;

	public GameObject Prefab {
		get {
			if (this._prefab != null) {
				return this._prefab;
			} else if (!string.IsNullOrEmpty(this.ResourcePath)) {
				return UResources.Load<GameObject>(this.ResourcePath);
			}
			return null;
		}
	}


	public GameObject Ins { get; protected set; }


	public virtual GameObject Spawn() {
		this.Ins = Object.Instantiate(this.Prefab);
		return Ins;
	}

	public virtual GameObject Spawn(Transform parent) {
		Spawn();
		Ins.transform.SetParent(parent, false);
		this.Ins.transform.localPosition = Vector3.zero;
		this.Ins.transform.localRotation = Quaternion.identity;
		this.Ins.transform.localScale = Vector3.one;
		return Ins;
	}

}
