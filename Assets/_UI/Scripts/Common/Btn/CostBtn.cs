using UnityEngine;
using GameUI;
using UnityEngine.UI;

public class CostBtn : CommonBtn
{
	public Image CostIcon;
	public Text CostTxt;

	public void SetData(Sprite icon, int cost, string txt)
	{
		CostIcon.sprite = icon;
		CostTxt.text = cost.ToString();
		Txt.text = txt;
	}
}
