using System.Collections.Generic;
using UnityEngine;

namespace Game {
	public class PerformanceTest :MonoBehaviour{

		public static List<GameObject> Lists=new List<GameObject>();


		void Start() {
			Lists.Add(gameObject);
		}

		void OnDestroy() {
			Lists.Remove(this.gameObject);
		}
		
	}
}