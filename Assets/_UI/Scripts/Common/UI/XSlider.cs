using UnityEngine;

namespace GameUI
{
	public class XSlider : MonoBehaviour
	{
		public RectTransform Parent;
		public RectTransform Point;

		public float Value = 0f;
		
		void Update()
		{
			Point.anchoredPosition = new Vector2(GetTargetX(Value), Point.anchoredPosition.y);
		}

		private float GetTargetX(float val)
		{
			return Parent.sizeDelta.x*Value - (Parent.sizeDelta.x*0.5f);
		}
	}

}

