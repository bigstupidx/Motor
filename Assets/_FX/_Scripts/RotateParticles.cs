using UnityEngine;
using System.Collections;

public class RotateParticles : MonoBehaviour {

	public float magicParticlesRotation = 10.0f;
	public float mprSpeed = 1.75f;

	// Update is called once per frame
	void Update () {
		transform.Rotate(0, mprSpeed, 0 * magicParticlesRotation * Time.deltaTime, 0);
	}
}
