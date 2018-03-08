using UnityEngine.UI;

namespace GameUI{

	[System.Serializable]
	public class ImgTxt {
		public Image Single;
		public Image Decade;
		public Image Unit;

		public void SetTxt(int i, NumPicType color) {
			if (i < 10)
			{
				Single.gameObject.SetActive(true);
				Unit.gameObject.SetActive(false);
				Decade.gameObject.SetActive(false);

				Single.sprite = UIDataDef.GetNumPic(i, color);
			} else
			{
				Single.gameObject.SetActive(false);
				Unit.gameObject.SetActive(true);
				Decade.gameObject.SetActive(true);

				Unit.sprite = UIDataDef.GetNumPic(i / 10, color);
				Unit.sprite = UIDataDef.GetNumPic(i % 10, color);
			}
		}
	}
}

