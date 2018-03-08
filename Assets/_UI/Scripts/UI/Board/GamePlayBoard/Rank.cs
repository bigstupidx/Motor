using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class Rank : MonoBehaviour
	{
		public ImgTxt Current;
		public ImgTxt Total;

		public void SetCurrent(int current) {
			Current.SetTxt(current, NumPicType.Yellow);
		}

		public void SetTotal(int total) {
			Total.SetTxt(total, NumPicType.Yellow);
		}

	}
}

