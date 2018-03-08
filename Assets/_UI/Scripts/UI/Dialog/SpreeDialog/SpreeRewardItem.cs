using UnityEngine;
using GameClient;
using UnityEngine.UI;

public class SpreeRewardItem : MonoBehaviour
{
	public Text Amount;
	public Image Icon;
	

	public void SetData(RewardItemInfo info)
	{
		if (info.Amount == 1)
		{
			Amount.gameObject.SetActive(false);
		}
		else
		{
			Amount.gameObject.SetActive(true);
			Amount.text = "x" + info.Amount;
		}
		
		Icon.sprite = info.Data.Icon.Sprite;
	}
}
