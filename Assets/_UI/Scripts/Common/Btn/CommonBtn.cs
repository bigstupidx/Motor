using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public enum BtnColor {
		Yellow,
		Green
	}

	public class CommonBtn : MonoBehaviour {
		public BtnColor Color = BtnColor.Yellow;
		public Button Btn;
		public Image Bg;
		public Text Txt;

		public void SetEnable(bool enable, string txt = null) {
			Btn.enabled = enable;
			if (enable) {
				switch (Color) {
					case BtnColor.Yellow:
						Bg.sprite = UIDataDef.Sprite_Button_Yellow;
						break;
					case BtnColor.Green:
						Bg.sprite = UIDataDef.Sprite_Button_Green;
						break;
				}
			} else {
				Bg.sprite = UIDataDef.Sprite_Button_Grey;
			}
			if (!string.IsNullOrEmpty(txt)) {
				Txt.text = txt;
			}
		}
	}

}

