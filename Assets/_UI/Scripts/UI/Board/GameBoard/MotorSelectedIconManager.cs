using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameUI
{
	public class MotorSelectedIconManager : MonoBehaviour
	{
		public HorizontalLayoutGroup layoutGroup;
		public float lineLength;
		public float lineRate;
		public float scaleRate;
		public GameObject selectItem;
		[System.NonSerialized]
		public List<MotorSelectedIconItem> items = new List<MotorSelectedIconItem> ();
        [System.NonSerialized]
        public List<BikeListItemData> data;
        public void InitSelectInfo (int count, int index, List<BikeListItemData> _data)
		{
            data = _data;
			RectTransform tt = ((RectTransform)layoutGroup.transform);
			tt.sizeDelta = new Vector2 ((lineLength + layoutGroup.spacing) * (count - 1) + lineLength * lineRate,tt.sizeDelta.y);
			if (items.Count == 0) {
				for (int i = 0; i < count; ++i) {
					MotorSelectedIconItem item = (Instantiate (selectItem) as GameObject).GetComponent<MotorSelectedIconItem> ();
					item.transform.SetParent (transform);
					item.transform.localScale = Vector3.one;
					items.Add (item);
				}
			}
			this.Refresh (index);
		}

		public void Refresh(int index){
			for (int i = 0; i < items.Count; ++i) {
				if (i == index) {
					items [i].SetData (true, lineRate, scaleRate,data[i].Info.isUnLock);
				} else {
					items [i].SetData (false, lineRate, scaleRate,data[i].Info.isUnLock);
				}
			}
		}


	}
}
