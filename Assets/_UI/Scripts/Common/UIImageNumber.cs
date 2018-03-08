//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HeavyDutyInspector;
using UnityEngine.UI;

namespace GameUI
{
	public static class IntegerExtension
	{
		public static int ValueOf(this int num, int index)
		{
			var len = num.Length();
			for (var i = 0; i < len - index - 1; i++)
			{
				num /= 10;
			}
			return num%10;
		}

		public static int Length(this int num)
		{
			if (num == 0) return 1;
			var len = 0;
			while (num != 0)
			{
				len++;
				num /= 10;
			}
			return len;
		}
	}

	public enum UIImageNumberAlignTypeH {
		Left = 0, Cneter = 1, Right = 2
	}

	public enum UIImageNumberAlignTypeV {
		Top = 0, Cneter = 1, Bottom = 2
	}

	[RequireComponent(typeof(RectTransform))]
	public class UIImageNumber : MonoBehaviour
	{
		[ComplexHeader("所属图集", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public string UiAtlas = "UI";
		[SerializeField]
		[HideInInspector]
		private string _uiAtlas;

		[ComplexHeader("图片前缀", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
		public string UiSpriteNamePre;
		[SerializeField]
		[HideInInspector]
		private string _uiSpriteNamePre;

		[ComplexHeader("位数 ( 1-9 )", Style.Box, Alignment.Left, ColorEnum.Green, ColorEnum.White)]
		public int NumSize = 5;
		[SerializeField]
		[HideInInspector]
		private int _numSize;

		[ComplexHeader("数值 ( >0 )", Style.Box, Alignment.Left, ColorEnum.Green, ColorEnum.White)]
		public int NumValue = 0;

		[ComplexHeader("尺寸", Style.Box, Alignment.Left, ColorEnum.Cyan, ColorEnum.White)]
		public Vector2 FontSize = new Vector2(100f, 150f);
		[ComplexHeader("间距", Style.Box, Alignment.Left, ColorEnum.Cyan, ColorEnum.White)]
		public float Padding = 10f;
		[ComplexHeader("对齐", Style.Box, Alignment.Left, ColorEnum.Cyan, ColorEnum.White)]
		public UIImageNumberAlignTypeH Horizontal = UIImageNumberAlignTypeH.Left;
		public UIImageNumberAlignTypeV Vertical = UIImageNumberAlignTypeV.Cneter;
		[ComplexHeader("补0", Style.Box, Alignment.Left, ColorEnum.Cyan, ColorEnum.White)]
		public bool PadZero = false;

		public Color Color {
			get { return _color; }
			set
			{ 
				if (_images == null)
				{
					return;
				}
				_color = value;
				for (var i = 0; i < _images.Count; i++)
				{
					_images[i].color = _color;
				}
			}
		}
		private Color _color;

		public int Text {
			get
			{
				return _text;
				
			}
			set
			{
				value = value < 0 ? 0 : value;
				if (value.Length() != _text.Length()) _isLengthChanged = true;
				_text = value;
				if (_text.Length() > _numSizeMax) _text = int.Parse(_text.ToString().Substring(0, _numSizeMax));
				NumValue = _text;
				UpdateValue();
			}
		}
		private int _text;
		private bool _isLengthChanged;
		private int _numSizeMax = 9;

		private List<Sprite> _sprites = new List<Sprite>();
		private List<Image> _images = new List<Image>();
		private RectTransform _rect;
		private List<RectTransform> _imageRect = new List<RectTransform>();

		private void OnValidate()
		{
//			var atlasChange = false;
//			if (_uiAtlas != UiAtlas)
//			{
//				_uiAtlas = UiAtlas;
//				atlasChange = true;
//			}
//			if (_uiSpriteNamePre != UiSpriteNamePre)
//			{
//				_uiSpriteNamePre = UiSpriteNamePre;
//				atlasChange = true;
//			}
//			if (atlasChange)
//			{
//				UpdateSprites();
//			}
//			var sizeChange = false;
//			if (_numSize != NumSize)
//			{
//				NumSize = Mathf.Clamp(NumSize, 1, _numSizeMax);
//				_numSize = NumSize;
//				sizeChange = true;
//			}
//			if (sizeChange)
//			{
//				UpdateNumSize();
//			}
			UpdateSprites();
			NumSize = Mathf.Clamp(NumSize, 1, _numSizeMax);
			UpdateNumSize();

			UpdateItemSize();
			UpdateLayout();
			NumValue = NumValue < 0 ? 0 : NumValue;
			Text = NumValue;
		}

		private void UpdateSprites()
		{
			var atlas = Resources.Load<Atlas>("Atlas/" + UiAtlas);
			if (atlas == null) return;
			_sprites.Clear();
			for (var i = 0; i <= _numSizeMax; i++)
			{
				var spriteName = UiSpriteNamePre + i;
				var index = atlas.SpriteNames.IndexOf(spriteName);
				if (index >= 0)
				{
					var sprite = atlas.Sprites[index];
					_sprites.Add(sprite);
				}
			}
		}

		private void UpdateNumSize()
		{
			var childs = transform.GetComponentsInChildren<Transform>(true).ToList();
			childs.Remove(transform);
			_images.Clear();
			for (var i = 0; i < childs.Count; i++)
			{
				childs[i].gameObject.SetActive(true);
				_images.Add(childs[i].GetComponent<Image>());
			}
			for (var i = childs.Count; i < _numSizeMax; i++)
			{
				var image = new GameObject("Num" + i).AddComponent<Image>();
				image.transform.SetParent(transform);
				image.transform.ResetLocal();
				image.sprite = null;
				_images.Add(image);
			}
			for (var i = NumSize; i < childs.Count; i++)
			{
				childs[i].gameObject.SetActive(false);
			}
			// 缓存Rect
			_rect = GetComponent<RectTransform>();
			_imageRect.Clear();
			for (var i = 0; i < _images.Count; i++)
			{
				_imageRect.Add(_images[i].GetComponent<RectTransform>());
			}
		}

		private void UpdateItemSize()
		{
			if(_imageRect == null) return;
			for (var i = 0; i < _imageRect.Count; i++)
			{
				var rect = _imageRect[i];
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FontSize.x);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, FontSize.y);
			}
		}

		private void UpdateLayout()
		{
			var corners = new Vector3[4];
			if(_rect != null) _rect.GetWorldCorners(corners);
			for (var i = 0; i < _images.Count; i++)
			{
				var image = _images[i];
				var rect = _imageRect[i];
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.one;
				var pivotX = 0.5f*(int) Horizontal;
				var pivotY = 0.5f*(2 - (int) Vertical);
				rect.pivot = new Vector2(pivotX, pivotY);
				switch (Vertical)
				{
					case UIImageNumberAlignTypeV.Top:
						// pivotY = 1f;
						image.transform.SetPositionY(corners[1].y);
						break;
					case UIImageNumberAlignTypeV.Cneter:
						// pivotY = 0.5f;
						var center = (corners[3].y + corners[1].y)/2;
						image.transform.SetPositionY(center);
						break;
					case UIImageNumberAlignTypeV.Bottom:
						// pivotY = 0f;
						image.transform.SetPositionY(corners[3].y);
						break;
				}
				switch (Horizontal)
				{
					case UIImageNumberAlignTypeH.Left:
						// pivotX = 0f ;
						image.transform.SetPositionX(corners[0].x);
						image.transform.SetLocalPositionX(image.transform.localPosition.x + (FontSize.x + Padding)*i);
						break;
					case UIImageNumberAlignTypeH.Cneter:
						// pivotX =0.5f;
						var center = (corners[3].x + corners[0].x)/2;
						image.transform.SetPositionX(center);
						var len = PadZero ? NumSize : Text.Length();
						len = NumSize < len ? NumSize : len;
						image.transform.SetLocalPositionX(center + (FontSize.x + Padding)*(i - NumSize + (len + 1)*0.5f));
						break;
					case UIImageNumberAlignTypeH.Right:
						// pivotX = 1f;
						image.transform.SetPositionX(corners[3].x);
						image.transform.SetLocalPositionX(image.transform.localPosition.x - (FontSize.x + Padding)*(-i - 1 + NumSize));
						break;
				}
			}
		}

		private void UpdateValue()
		{
			if(_sprites == null || _sprites.Count < 10) return;
			if (Horizontal == UIImageNumberAlignTypeH.Left && !PadZero)
			{
				var len = Text.Length();
				for (var i = 0; i < len; i++)
				{
					var num = Text.ValueOf(i);
					_images[i].enabled = true;
					_images[i].sprite = _sprites[num];
				}
				for (var i = len; i < _images.Count; i++)
				{
					_images[i].enabled = false;
				}
			}
			else
			{
				var value = NumValue;
				var index = NumSize - 1;
				do
				{
					var num = value%10;
					_images[index].enabled = true;
					_images[index].sprite = _sprites[num];
					value /= 10;
					index--;
				} while (value != 0 && index >= 0);
				for (var i = index; i >= 0; i--)
				{
					_images[i].enabled = PadZero;
					if (PadZero) _images[i].sprite = _sprites[0];
				}
			}
			if (Horizontal == UIImageNumberAlignTypeH.Cneter && _isLengthChanged)
			{
				UpdateLayout();
				_isLengthChanged = false;
			}
		}

		void Awake()
		{
			UpdateSprites();
			UpdateNumSize();
			UpdateItemSize();
		}

		void Start()
		{
			UpdateLayout();
			UpdateValue();
		}
	}

}
