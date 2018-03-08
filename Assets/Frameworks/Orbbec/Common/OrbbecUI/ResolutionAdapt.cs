using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ResolutionAdapt : MonoBehaviour
{
	RectTransform rectTransform;
 
	public Vector2 SizePercent;
	public Vector2 PosPercent;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();

		if (rectTransform == null)
			return;

		UpdateRect();
	}

	void Update()
	{
#if UNITY_EDITOR
		UpdateRect();
#endif
	}

	public void UpdateRect()
	{
		if (rectTransform == null)
			return;

		rectTransform.anchorMin = Vector2.one * 0.5f;
		rectTransform.anchorMax = Vector2.one * 0.5f;

		Vector2 Position = Vector2.one;
		Vector2 Size = Vector2.one;

		Position.x *= -0.5f * Screen.width + Screen.width * PosPercent.x;
		Position.y *= -0.5f * Screen.height + Screen.height * PosPercent.y;

		Size.x *= Screen.width * SizePercent.x;
		Size.y *= Screen.height * SizePercent.y;

		rectTransform.anchoredPosition = Position;
		rectTransform.sizeDelta = Size;
	}
}

