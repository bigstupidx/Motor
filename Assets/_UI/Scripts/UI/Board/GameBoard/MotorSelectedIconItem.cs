using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameUI
{
	public class MotorSelectedIconItem : MonoBehaviour
	{
		public LayoutElement layoutElement;
		public Image icon;
		public RectTransform rectContent;
		// Use this for initialization
		public void SetData(bool isSelect,float layoutRate,float scaleY,bool Unlock){
			if(isSelect){
				rectContent.localScale = new Vector3 (1.0f, scaleY, 1.0f);
				layoutElement.flexibleWidth = layoutRate;
				icon.sprite = AtlasManager.GetSprite (UIDataDef.Atlas_UI_Name, "Frame_MotorInfo_Statu_1");
                icon.color = Color.white;

            }
			else{
                if (Unlock)
                {
                    rectContent.localScale = Vector3.one;
                    layoutElement.flexibleWidth = 1.0f;
                    icon.sprite = null;
                    icon.color = UIDataDef.MotorUnlock;
                }
                else
                {
                    rectContent.localScale = Vector3.one;
                    layoutElement.flexibleWidth = 1.0f;
                    icon.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_MotorInfo_Statu_0");
                    icon.color = Color.white;
                }
			}
		}
	}

}
