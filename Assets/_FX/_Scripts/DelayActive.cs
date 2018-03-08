using System.Collections;
using UnityEngine;
public class DelayActive : MonoBehaviour {

	public GameObject[] Objects;
	public float time;
	public bool useTimeScale;

	void OnEnable() {
		StartCoroutine(Delay());
	}

	IEnumerator Delay() {
		yield return null;//这里延迟一帧因为UIEffect_LoadOnStart需要在生成后获取renderer
		foreach (var o in this.Objects) {
			o.SetActive(false);
		}
		if (this.useTimeScale) {
			yield return new WaitForSeconds(this.time);
		} else {
			yield return new WaitForSecondsRealtime(this.time);
		}

		foreach (var o in this.Objects) {
			o.SetActive(true);
		}
	}



}
