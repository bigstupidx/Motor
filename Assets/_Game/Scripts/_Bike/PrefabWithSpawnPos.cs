using UnityEngine;

[System.Serializable]
public class PrefabWithSpawnPos : NormalPrefab {
	public Transform SpawnPos;

	public override GameObject Spawn() {
		this.Ins = Object.Instantiate(this.Prefab);
		this.Ins.transform.SetParent(this.SpawnPos, false);
		this.Ins.transform.localPosition = Vector3.zero;
		this.Ins.transform.localRotation = Quaternion.identity;
		this.Ins.transform.localScale = Vector3.one;
		return Ins;
	}

}
