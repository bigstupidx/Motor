using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Orbbec;
using OrbbecGestures;

public class GesturesSampleConfig : MonoBehaviour
{
	public static GesturesSampleConfig Instance = null;

	public ConfigMenuItem ItemCopy = null;
	public GameObject ContentRoot = null;
	ConfigMenuItem[] MenuItems = null;

	public GameObject GestureSampleMain = null;

	public enum ConfigState
	{
		CS_IsUsingSubThread,
		CS_IsShowDebugInfo,
		CS_PlayerNum,
		CS_SkeletonVelocityFrames,
		CS_SkeletonVelocityMulType,
		CS_IsGetVelocityData,
	}

	int IsUsingSubThread;
	public int IsShowDebugInfo;
	int PlayerNum;
	int SkeletonVelocityFrames = 1;
	VelocityMulType SkeletonVelocityMulType = VelocityMulType.VMT_AVERAGE;
	int IsGetVelocityData;

	VelocityMulType[] VelocityMulTypeArray;

	ConfigState[] typeArray;
	int TypeIndex = 0;

	public GestureConfigParams gestureParam = new GestureConfigParams();

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		LoadSetting();
		InitSelectArray();

		CreateConfigMenuItems();
		UpdateContext();
		DontDestroyOnLoad(gameObject);
	}

	void LoadSetting()
	{
		IsUsingSubThread = PlayerPrefs.GetInt("IsUsingSubThread", 0);
		IsShowDebugInfo		= PlayerPrefs.GetInt("IsShowDebugInfo", 0);
		PlayerNum			= PlayerPrefs.GetInt("PlayerNum", 1);
		SkeletonVelocityFrames = PlayerPrefs.GetInt("SkeletonVelocityFrames", 1);
		SkeletonVelocityMulType = (VelocityMulType)PlayerPrefs.GetInt("SkeletonVelocityMulType", 0);
		IsGetVelocityData	= PlayerPrefs.GetInt("IsGetVelocityData", 1);
	}

	void SaveSetting()
	{
		PlayerPrefs.SetInt("IsUsingSubThread", IsUsingSubThread);
		PlayerPrefs.SetInt("IsShowDebugInfo", IsShowDebugInfo);
		PlayerPrefs.SetInt("PlayerNum", PlayerNum);
		PlayerPrefs.SetInt("SkeletonVelocityFrames", SkeletonVelocityFrames);
		PlayerPrefs.SetInt("SkeletonVelocityMulType", (int)SkeletonVelocityMulType);
		PlayerPrefs.SetInt("IsGetVelocityData", IsGetVelocityData);

		PlayerPrefs.Save();
	}


	void InitSelectArray()
	{
		typeArray = (ConfigState[])System.Enum.GetValues(typeof(ConfigState));
		VelocityMulTypeArray = (VelocityMulType[])Enum.GetValues(typeof(VelocityMulType));
	}


	void CreateConfigMenuItems()
	{
		if (ItemCopy == null || ContentRoot == null)
			return;
		GameObject obj = null;
		ConfigMenuItem Item = null;

		float YOffset = 80.0f;

		MenuItems = new ConfigMenuItem[Enum.GetValues(typeof(ConfigState)).Length];

		int i = 0;

		obj = GameObject.Instantiate(ItemCopy.gameObject);
		obj.transform.SetParent(ContentRoot.transform);
		obj.transform.localPosition = new Vector3(0, -i * YOffset, 0);
		obj.transform.localScale = Vector3.one;
		obj.SetActive(true);
		Item = obj.GetComponent<ConfigMenuItem>();
		Item.UpdateFunc = (ConfigMenuItem curItem) => { curItem.ItemText.text = string.Format("IsUsingSubThread:{0}\n", IsUsingSubThread != 0); };
		MenuItems[i++] = Item;

		obj = GameObject.Instantiate(ItemCopy.gameObject);
		obj.transform.SetParent(ContentRoot.transform);
		obj.transform.localPosition = new Vector3(0, -i * YOffset, 0);
		obj.transform.localScale = Vector3.one;
		obj.SetActive(true);
		Item = obj.GetComponent<ConfigMenuItem>();
		Item.UpdateFunc = (ConfigMenuItem curItem) => { curItem.ItemText.text = string.Format("IsShowDebugInfo:{0}\n", IsShowDebugInfo != 0); };
		MenuItems[i++] = Item;

		obj = GameObject.Instantiate(ItemCopy.gameObject);
		obj.transform.SetParent(ContentRoot.transform);
		obj.transform.localPosition = new Vector3(0, -i * YOffset, 0);
		obj.transform.localScale = Vector3.one;
		obj.SetActive(true);
		Item = obj.GetComponent<ConfigMenuItem>();
		Item.UpdateFunc = (ConfigMenuItem curItem) => { curItem.ItemText.text = string.Format("PlayerNum:{0}\n", PlayerNum); };
		MenuItems[i++] = Item;

		obj = GameObject.Instantiate(ItemCopy.gameObject);
		obj.transform.SetParent(ContentRoot.transform);
		obj.transform.localPosition = new Vector3(0, -i * YOffset, 0);
		obj.transform.localScale = Vector3.one;
		obj.SetActive(true);
		Item = obj.GetComponent<ConfigMenuItem>();
		Item.UpdateFunc = (ConfigMenuItem curItem) => { curItem.ItemText.text = string.Format("SkeletonVelocityFrames:{0}\n", SkeletonVelocityFrames); };
		MenuItems[i++] = Item;

		obj = GameObject.Instantiate(ItemCopy.gameObject);
		obj.transform.SetParent(ContentRoot.transform);
		obj.transform.localPosition = new Vector3(0, -i * YOffset, 0);
		obj.transform.localScale = Vector3.one;
		obj.SetActive(true);
		Item = obj.GetComponent<ConfigMenuItem>();
		Item.UpdateFunc = (ConfigMenuItem curItem) => { curItem.ItemText.text = string.Format("SkeletonVelocityFrames:{0}\n", SkeletonVelocityMulType.ToString()); };
		MenuItems[i++] = Item;

		obj = GameObject.Instantiate(ItemCopy.gameObject);
		obj.transform.SetParent(ContentRoot.transform);
		obj.transform.localPosition = new Vector3(0, -i * YOffset, 0);
		obj.transform.localScale = Vector3.one;
		obj.SetActive(true);
		Item = obj.GetComponent<ConfigMenuItem>();
		Item.UpdateFunc = (ConfigMenuItem curItem) => { curItem.ItemText.text = string.Format("IsGetVelocityData:{0}\n", IsGetVelocityData != 0); };
		MenuItems[i++] = Item;
	}

	void UpdateContext()
	{

		for (int i = 0; i < MenuItems.Length; ++i)
		{
			MenuItems[i].SetSelect(TypeIndex == i);
			MenuItems[i].DoUpdateText();
		}

		/*
		{
			float minHeight = 464.0f + (TypeIndex - (int)ConfigState.CS_UVCType) * 80.0f;
			Vector3 pos = ContentRoot.transform.localPosition;
			if (pos.y < minHeight)
			{
				pos.y = minHeight;
			}

			float Height = pos.y - TypeIndex * 80.0f;

			if (Height > 464.0f)
			{
				pos.y = 464.0f + TypeIndex * 80.0f;
			}

			ContentRoot.transform.localPosition = pos;
		}
		*/
	}

	void Start()
	{

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OrbbecManager.DoExit();
		}

		if (Input.GetKey(KeyCode.KeypadEnter)
					|| Input.GetKey(KeyCode.Return)
					|| Input.GetKey((KeyCode)10)
					|| Input.GetKey(KeyCode.Joystick1Button0)
					|| Input.GetKey(KeyCode.Joystick1Button1)
					|| Input.GetKey(KeyCode.Joystick2Button0)
					|| Input.GetKey(KeyCode.Joystick2Button1))
		{
			OnSetting();
			return;
		}

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			TypeIndex = (TypeIndex + typeArray.Length - 1) % typeArray.Length;
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			TypeIndex = (TypeIndex + 1) % typeArray.Length;
		}

		UpdateSelectState();

		if (Input.anyKeyDown)
		{
			UpdateContext();
		}	
	}

	void UpdateSelectState()
	{
		switch (typeArray[TypeIndex])
		{

			case ConfigState.CS_IsUsingSubThread:
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
					{
						IsUsingSubThread = (IsUsingSubThread + 1) % 2;
					}
				}
				break;
			case ConfigState.CS_IsShowDebugInfo:
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
					{
						IsShowDebugInfo = (IsShowDebugInfo + 1) % 2;
					}
				}
				break;
			case ConfigState.CS_PlayerNum:
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow) && PlayerNum > 1)
					{
						--PlayerNum;
					}
					else if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						++PlayerNum;
					}
				}
				break;

			case ConfigState.CS_SkeletonVelocityFrames:
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow) && SkeletonVelocityFrames > 1)
					{
						--SkeletonVelocityFrames;
					}
					else if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						++SkeletonVelocityFrames;
					}
				}
				break;
			case ConfigState.CS_SkeletonVelocityMulType:
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow))
					{
						SkeletonVelocityMulType = VelocityMulTypeArray[((int)SkeletonVelocityMulType + VelocityMulTypeArray.Length - 1) % VelocityMulTypeArray.Length];
					}
					else if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						SkeletonVelocityMulType = VelocityMulTypeArray[((int)SkeletonVelocityMulType + 1) % VelocityMulTypeArray.Length];
					}
				}
				break;
			case ConfigState.CS_IsGetVelocityData:
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
					{
						IsGetVelocityData = (IsGetVelocityData + 1) % 2;
					}
				}
				break;	
		}

	}


	public void OnSetting()
	{
		SaveSetting();

		gestureParam.IsUsingSubThread = IsUsingSubThread != 0;
		gestureParam.IsGetVelocityData = IsGetVelocityData != 0;
		gestureParam.PlayerNum = PlayerNum;

		gameObject.SetActive(false);

		if (GestureSampleMain)
			GestureSampleMain.SetActive(true);
	}
}