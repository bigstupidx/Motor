using UnityEngine;
using GameClient;
using GameUI;
using UnityEngine.UI;

public class ShopTag : MonoBehaviour
{

	public IAPType Type;
	public Image Bg, Txt;
	public string TextSpritName;

	void Start()
	{
		GetComponent<Button>().onClick.AddListener(OnBtnClick);
	}

	public void SetSelectState(bool isSelected)
	{
		Bg.sprite = isSelected ? UIDataDef.Sprite_Frame_BG_TaskTag_Yellow : UIDataDef.Sprite_Frame_BG_TaskTag_Blue;
		//string txt = isSelected ? TextSpritName + "1" : TextSpritName;
		//Txt.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, txt);
	}

	public void OnBtnClick()
	{

		if (Type == ShopBoard.CurrentShowType)
		{
			return;
		}
		ShopBoard.Ins.OnBtnTagClick(Type);
	}
}
