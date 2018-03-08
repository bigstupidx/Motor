using UnityEngine;
using UnityEngine.UI;
using XPlugin.Localization;

public class RawImageHelper : MonoBehaviour
{
	public string Key;

	void Start ()
	{
		GetComponent<RawImage>().texture = LResources.Load<Texture>(Key);
	}
	
}
