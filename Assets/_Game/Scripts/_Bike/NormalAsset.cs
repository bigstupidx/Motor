
using UnityEngine;
using XPlugin.Update;

[System.Serializable]
public class NormalAsset<T> where T : Object {

	public string ResourcePath;
	public T _asset;

	public T Assets {
		get {
			if (this._asset != null) {
				return this._asset;
			} else if (!string.IsNullOrEmpty(this.ResourcePath)) {
				return UResources.Load<T>(this.ResourcePath);
			}
			return null;
		}
	}

}
