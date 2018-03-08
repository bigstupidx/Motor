using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace GameUI
{

	public class BtnPressEffect : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
	{
		public Color DefaultPressColor;
		public List<GameObject> objList = new List<GameObject> ();
		public List<UITweener> tweenList = new List<UITweener> ();
		public List<ACTextFX> acTextFxList = new List<ACTextFX> ();

		void OnEnable(){
			foreach (GameObject obj in objList) {
				obj.SetActive (false);
			}
			foreach (UITweener tween in tweenList) {
				tween.ResetToBeginning ();
			}
		}

		void OnDisable(){
			foreach (UITweener tween in tweenList) {
				tween.ResetToBeginning ();
				tween.enabled = false;
			}
		}


		public void OnPointerDown (PointerEventData eventData){
			foreach (GameObject obj in objList) {
				obj.SetActive (true);
			}
			foreach (UITweener tween in tweenList) {
				tween.enabled = true;
				tween.ResetToBeginning ();
				tween.PlayForward ();
			}
		}

		public void OnPointerUp (PointerEventData eventData){
			foreach (GameObject obj in objList) {
				obj.SetActive (false);
			}
			foreach (UITweener tween in tweenList) {
				tween.ResetToBeginning ();
			}
		}

		public void OnPointerExit (PointerEventData eventData){
			foreach (GameObject obj in objList) {
				obj.SetActive (false);
			}
			foreach (UITweener tween in tweenList) {
				tween.ResetToBeginning ();
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Pack")]
		public void GetConfigData ()
		{
			acTextFxList.Clear ();
			acTextFxList = new List<ACTextFX> ();
			transform.FindChild ("BtnPress");
			foreach(Transform t in transform){
				MaskableGraphic mg = t.GetComponent<Text> ();
				if(mg != null){
					ACTextFX act = new ACTextFX () {
						NormalColor = mg.color,
						PressColor = DefaultPressColor,
						MGObj = mg
					};
					acTextFxList.Add (act);
				}
			}
		}

		public void GetTransform(Transform childT){
			
		}

		public void GetInfo(){
			foreach (Transform t in transform) {
				MaskableGraphic mg = t.GetComponent<Text> ();
				if (mg != null) {
					ACTextFX act = new ACTextFX () {
						NormalColor = mg.color,
						PressColor = DefaultPressColor,
						MGObj = mg
					};
					acTextFxList.Add (act);
				}
			}
		}
#endif
		[System.Serializable]
		public class ACTextFX{
			[SerializeField]
			public Color NormalColor;
			[SerializeField]
			public Color PressColor;
			[SerializeField]
			public MaskableGraphic MGObj;

		}
	}

}