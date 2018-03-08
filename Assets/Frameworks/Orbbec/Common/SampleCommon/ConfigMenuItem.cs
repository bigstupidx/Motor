using UnityEngine;
using System.Collections;

public class ConfigMenuItem : MonoBehaviour 
{
	public GameObject Select = null;
	public UnityEngine.UI.Text ItemText = null;

	public UpdateTextFunc UpdateFunc;

	public delegate void UpdateTextFunc(ConfigMenuItem Item);

	public void SetSelect(bool Flag)
	{
		if (Select == null)
			return;

		Select.SetActive(Flag);
	}

	public void DoUpdateText()
	{
		UpdateFunc(this);
	}

	public void SetText(string Text)
	{
		if (ItemText == null)
			return;

		ItemText.text = Text;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
