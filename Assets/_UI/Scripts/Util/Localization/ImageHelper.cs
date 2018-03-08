using UnityEngine;
using GameUI;
using UnityEngine.UI;

public class ImageHelper : MonoBehaviour {

	public string Key;
	public string AtlasName = UIDataDef.Atlas_UI_Name;

	void Start()
	{
		GetComponent<Image>().sprite = AtlasManager.GetLocalizedSprite(AtlasName, Key);
	}

#if UNITY_EDITOR
	[ContextMenu("赋值Key")]
	public void SetKey()
	{
		Key = GetComponent<Image>().sprite.name;
	}
#endif
}
