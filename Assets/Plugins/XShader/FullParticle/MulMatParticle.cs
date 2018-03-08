using UnityEngine;
using System.Collections;

public class MulMatParticle : MonoBehaviour {
	[Reorderable]
	public Material[] mats;

	[ContextMenu("OnValidate")]
	void OnValidate() {
		GetComponent<ParticleSystemRenderer>().materials = mats;
	}

}
