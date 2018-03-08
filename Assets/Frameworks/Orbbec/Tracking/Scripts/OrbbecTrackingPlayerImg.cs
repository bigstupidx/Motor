using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Orbbec
{
	public class OrbbecTrackingPlayerImg : MonoBehaviour
	{
		public CanvasGroup canvasGroup = null;

		public CanvasGroup barCanvasGroup = null;

		public Image barImg = null;

		private float _fillAmount = 0f;

		private RectTransform _rectTransform = null;

		public RectTransform rectTransform
		{
			get 
			{
				if (_rectTransform == null)
				{
					_rectTransform = transform as RectTransform;
				}
				return _rectTransform;
			}
		}

		public float fillAmount 
		{
			get
			{
				return _fillAmount;
			}
			set
			{
				_fillAmount = value;
				if (barCanvasGroup != null)
				{
					if (_fillAmount <= 0.001f)
					{
						barCanvasGroup.alpha = 0f;
					}
					else
					{
						barCanvasGroup.alpha = 1f;
					}
				}
				
				if (barImg != null)
				{
					barImg.fillAmount = _fillAmount;
				}
			}
		}
	}

}