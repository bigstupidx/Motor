using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace GameUI
{

	public class BtnPrePareSelect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private Image bgImage;
        public Image Board;
		// Use this for initialization
		void Start ()
		{
			bgImage = gameObject.GetComponent<Image> ();
		}

		// Use this for initialization
		public void OnPointerDown (PointerEventData eventData)
		{
            Board.gameObject.SetActive(true);
            this.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
			//bgImage.sprite = AtlasManager.GetSprite (UIDataDef.Atlas_UI_Name, "Button_BG_Icon_Select");
		}

		public void OnPointerUp (PointerEventData eventData)
		{
            Board.gameObject.SetActive(false);
            this.transform.localScale = new Vector3(1, 1, 1);
            bgImage.sprite = AtlasManager.GetSprite (UIDataDef.Atlas_UI_Name, "Frame_BG_Prop_53");
		}
	}
}
