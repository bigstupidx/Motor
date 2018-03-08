using UnityEngine;
using System.Collections.Generic;


namespace GameUI
{
	public class UpdateMax : MonoBehaviour
	{
		public List<GameObject> noMaxObjs;
		public List<GameObject> maxObjs;
		// Use this for initialization
		public void SetData(bool isMax){
			foreach(GameObject obj in noMaxObjs){
				obj.SetActive (!isMax);
			}
			foreach(GameObject obj in maxObjs){
				obj.SetActive (isMax);
			}
		}
	}
}
