using GameUI;
using UnityEngine;
using UnityEngine.UI;

public class NetPlayerItem : MonoBehaviour {
	public Image Icon;
	public Text Name;
	public Image Top;

	public void SetData(Sprite img, string name, bool isSelf) {
		Icon.sprite = img;
		Name.text = name;
		Name.color = UIDataDef.OnlineName;
	}

	public void SetActive(bool isShow) {
		if (!isShow) {
			Name.text = (LString.NETPLAYERITEM_SETACTIVE).ToLocalized();
			Name.color = Color.white;
		}
		if (isShow) {
			Top.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Shop_Title");
			Top.color = Color.white;
		} else {
			Top.sprite = null;
			Top.color = new Color(109f / 256f, 125f / 256f, 136f / 256f);
		}
		Icon.gameObject.SetActive(isShow);

	}
}
