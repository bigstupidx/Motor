using UnityEngine;
using System.Collections;

public class HeroIdleController : MonoBehaviour {
	private Animator animator;
	float _timer;
	void Awake() {
		animator = gameObject.GetComponent<Animator>();
	}

//	void OnEnable() {
//		StartCoroutine(startIdle());
//	}

	// Update is called once per frame

	private bool now1 = true;
	void Update() {
//		this._timer += Time.unscaledDeltaTime;
//		if (this._timer > 30f) {
//			this._timer = 0f;
//			this.now1 = !this.now1;
//			this.animator.CrossFade("stand" + (this.now1 ? "01" : "02"),0.15f);
//		}

	}

	//	IEnumerator startIdle(){
	//		yield return new WaitForSeconds (8.0f);
	//		int i = Random.Range (0, 3);
	//		animator.SetInteger ("Idle01", i);
	//		//yield return new WaitForSeconds (1.0f);
	//		//animator.SetInteger ("Idle01", -1);
	//		StartCoroutine (StartStand ());
	//	}
	//
	//	IEnumerator StartStand ()
	//	{
	//		yield return new WaitForSeconds (5.0f);
	//		animator.SetInteger ("Idle01", -1);
	//		StartCoroutine (startIdle ());
	//	}

}
