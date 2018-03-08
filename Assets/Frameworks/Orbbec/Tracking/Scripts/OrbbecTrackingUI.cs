using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Orbbec
{
	public class OrbbecTrackingUI : MonoBehaviour 
	{
		//显示的lable图
		public RawImage userLabelImg = null;

		//显示的标定图
		[SerializeField]
		private OrbbecTrackingPlayerImg player1Img = null;

		[SerializeField]
		private OrbbecTrackingPlayerImg player2Img = null;

		private bool _isSingleMode = false;
		public bool isSingleMode
		{
			get
			{
				return _isSingleMode;
			}
			set
			{
				_isSingleMode = value;
				if (_isSingleMode)
				{
					player1Img.rectTransform.anchoredPosition = new Vector2 (0f, 0f);
					player1Img.canvasGroup.alpha = 1f;
					player2Img.rectTransform.anchoredPosition = new Vector2 (0f, 0f);
					player2Img.canvasGroup.alpha = 0f;
				}
				else
				{
					player1Img.rectTransform.anchoredPosition = new Vector2 (-400f, 0f);
					player1Img.canvasGroup.alpha = 1f;
					player2Img.rectTransform.anchoredPosition = new Vector2 (400f, 0f);
					player2Img.canvasGroup.alpha = 1f;
				}
			}
		}

		public float player1Percent
		{
			get
			{
				return player1Img.fillAmount;
			}
			set
			{
				player1Img.fillAmount = value;
			}
		}

		public float player2Percent
		{
			get
			{
				return player2Img.fillAmount;
			}
			set
			{
				player2Img.fillAmount = value;
			}
		}
	}
}