using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class ReduceStamina : MonoBehaviour {
		public TweenCanvasGroupAlpha Twa;
		public TweenPosition Twp;
		public Text Txt;

		public void Show(int count)
		{
			Txt.text = "- " + count;
			Twa.ResetToBeginning();
			Twp.ResetToBeginning();
			Twa.PlayForward();
			Twp.PlayForward();
		}

		public void Init()
		{
			Twa.alpha = 0;
		}
	}

}

