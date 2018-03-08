using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BtnTopFunClick : MonoBehaviour,IPointerDownHandler ,IPointerUpHandler{
	private Image bgImage;

	void Awake(){
		bgImage = gameObject.GetComponent<Image> ();
		bgImage.enabled = false;
	}
	// Use this for initialization
	public void OnPointerDown (PointerEventData eventData){
		bgImage.enabled = true;
	}

	public void OnPointerUp (PointerEventData eventData){
		bgImage.enabled = false;
	}
}
