using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game {
	public class TestBike : MonoBehaviour {
		private BikeControl bike;
		void Start() {
			bike = FindObjectOfType<BikeControl>();
			TestManager.Ins.CurrentBike = bike;
			bike.gameObject.AddComponent<BikeInputTouch>();
			var cam= FindObjectOfType<BikeCamera>();
			cam.SetTarget(bike);
//			cam.enabled = false;
//			var follow= cam.gameObject.AddComponent<SmoothFollow>();
//			follow.target = bike.transform;
			
			ProjectorShadow.ProjectorShadow.Ins.Target = bike.transform;
			bike.ActiveControl = true;
		}


		void OnGUI() {
			GUILayout.Button(this.bike.Speed.ToString());
			GUILayout.Button(this.bike.BoostingEnergy.ToString());
		}


		void Update() {
			this.bike.BoostingEnergy = 100;
		}
	}
}
