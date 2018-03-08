using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Orbbec;
using OrbbecGestures;

public class GesturesSample : MonoBehaviour
{
	public static GesturesSample Instance = null;

	OrbbecManagerParam Param = null;
	GestureConfigParams gestureParam = null;

	public UnityEngine.UI.RawImage ImageTex;
	public UnityEngine.UI.RawImage LabelTex;
	public UnityEngine.UI.RawImage DepthTex;
	public UnityEngine.UI.RawImage BackRemovalTex;
	public UnityEngine.UI.RawImage SceneDepthTex;

	public ShowSkeletonPosition ShowSkeletonPositionController = null;

	public Text MsgText		= null;
	public Text GestureText = null;
	public Text FPSText		= null;
	float FPSUpdateTime = 0.0f;

	public OrbbecShowFPS MainFPS = null;

	public bool IsInit3DCameraRes = false;

	LinkedList<string> logBuffer = new LinkedList<string>();

	void Awake()
	{
		Instance = this;

#if CHANNEL_PERSEELITE
        Application.targetFrameRate = 30;
		Screen.SetResolution(1280, 720, true);
#endif

		if (OrbbecManager.HasOrbbecDevice())
		{
			Debug.Log("Find orbbec device!");
		}
		else
		{
			Debug.LogError("Can't find orbbec device.");
		}

#if SAMPLE_CONFIG
		if (SampleConfig.Instance != null && SampleConfig.Instance.Param != null)
		{
			Param = SampleConfig.Instance.Param;
		}
		else
		{
#endif
			Param = new OrbbecManagerParam();
			Param.IsUseCatch = false;
			Param.IsUseHandsTracker = false;
			Param.IsUseSceneDepth = false;
			Param.IsUseDepth = false;
			Param.IsUseBackRemoval = true;
			Param.IsUseUserLabel = true;
			Param.IsUseUserImage = true;
#if SAMPLE_CONFIG
		}
#endif

		Log.MaxBufferLogNum = 15;
		Log.LogBufferList = logBuffer;
		Log.msType = Log.LogType.ToBuffer;

		if (GesturesSampleConfig.Instance)
		{
			gestureParam = GesturesSampleConfig.Instance.gestureParam;
		}
		else
		{
			gestureParam = new GestureConfigParams();

			gestureParam.IsUsingSubThread = true;
			gestureParam.PlayerNum = 1;
			gestureParam.IsGetVelocityData = true;
		}

		gestureParam.GestureReturnInfoType = ReturnInfoType.RIT_SEGMENTID_TIME_STATE;
		gestureParam.GetStateCallback = OnGetGestureInfoCallback;
		
	}

	void OnGetGestureInfoCallback(int PlayerIndex, PlayerStateBase playerState)
	{
		PlayerState_SegTimeState SegTimeState = playerState as PlayerState_SegTimeState;
		if (SegTimeState == null)
			return;

		//bool HasRecognized = false;
		for ( int  i = 0; i < SegTimeState.GestureInfoData.ItemNum; ++i)
		{
			GestureInfo_Seg_Time_State state = SegTimeState.GestureInfoData[i];
			if (state.IsRecognized != 0)
			{
				//HasRecognized = true;
				if (GestureText != null)
				{
					string info = string.Format("Gesture {0}, Index:{1} Recognized", GestureManager.Instance.GetGestureName(PlayerIndex,state.GestureIndex), state.GestureIndex);
					GestureText.text = info;
					Log.Print(Log.Level.Log, info);
				}
			}
			else if (state.IsSegmentSucceed != 0)
			{
				if (GestureText != null)
				{
					string info = string.Format("Gesture {0}, Segment: {1} OK", GestureManager.Instance.GetGestureName(PlayerIndex, state.GestureIndex), state.SegmentIndex);
					//GestureText.text = info;
					Log.Print(Log.Level.Log, info);
				}
			}
			else
			{
				if (GestureText != null)
				{
					//string info = string.Format("Gesture {0}, Index:{1} Segment: {2} Failed", GestureManager.Instance.GetGestureName(state.GestureIndex), state.GestureIndex, state.SegmentIndex);
					//GestureText.text = info;
					//Log.Print(Log.Level.Log, info);
				}
			}
		}

// 		if (!HasRecognized)
// 		{
// 			GestureText.text = "No Gesture has been recognized.";
// 		}
	}

	// Use this for initialization
	void Start () 
	{
		Param.OrbbecInitResourceCallBack += OnDeviceReady;

		OrbbecManager.CreateOrbbecManager(Param);


	
	}


	void OnDeviceReady()
	{
		Quaternion rotation = Quaternion.identity;
		if (!Param.IsReserveTexture)
		{
			rotation = Quaternion.Euler(0, 0, 180.0f);
		}

		if (DepthTex != null)
		{
			DepthTex.texture = OrbbecManager.Instance.GetDepthMap();
			DepthTex.gameObject.SetActive(Param.IsUseDepth);
			DepthTex.gameObject.transform.localRotation = rotation;
		}

		if (SceneDepthTex != null)
		{
			SceneDepthTex.texture = OrbbecManager.Instance.GetSceneDepthMap();
			SceneDepthTex.gameObject.SetActive(Param.IsUseSceneDepth);
			SceneDepthTex.gameObject.transform.localRotation = rotation;
		}

		if (ImageTex != null)
		{
			ImageTex.texture = OrbbecManager.Instance.GetImageMap();
			ImageTex.gameObject.SetActive(Param.IsUseUserImage);
			ImageTex.gameObject.transform.localRotation = rotation;
		}

		if (LabelTex != null)
		{
			LabelTex.texture = OrbbecManager.Instance.GetUserLabelMap();
			LabelTex.gameObject.SetActive(Param.IsUseUserLabel);
			LabelTex.gameObject.transform.localRotation = rotation;
		}

		if (BackRemovalTex != null)
		{
			BackRemovalTex.texture = OrbbecManager.Instance.GetBackRemovalMap();
			BackRemovalTex.gameObject.SetActive(Param.IsUseBackRemoval);
			BackRemovalTex.gameObject.transform.localRotation = rotation;
		}

		OrbbecManager.LostUserConfidenceNum = 1;

		IsInit3DCameraRes = true;
		InitGesture();
	}

	void InitGesture()
	{
		GestureManager.Create(gestureParam);
		HashSet<uint> GestureTypeTable = new HashSet<uint>();

		TextAsset GestureXml = Resources.Load("GesturesConfig") as TextAsset;

		if (GestureXml != null)
		{
			GestureManager.Instance.LoadConfigData( GestureXml.text);
		}

		if (GesturesSampleConfig.Instance)
		{
			GestureManager.Instance.SetDebugInfo(GesturesSampleConfig.Instance.IsShowDebugInfo != 0);
		}


		GestureManager.Instance.StartGestureWork();

		for (int i = 0; i < gestureParam.PlayerNum; ++i)
		{
			int Num = GestureManager.Instance.GetGestureNum(i);

			if (Num > 0)
			{
				for (int j = 0; j < Num; ++j)
				{
					string gestureName = GestureManager.Instance.GetGestureName(i,(uint)j);
					Log.Print(Log.Level.Log, string.Format("Player:{0} Gesture: {1}|{2} Loaded.", i, gestureName, j));
					GestureManager.Instance.SetPlayerGesture(i, (uint)j, true);
				}
			}
			else
			{
				Log.Print(Log.Level.Error, "No Gesture has been loaded.");
			}
		}

		
	}

	List<int> BindedUserIDTabel = new List<int>();
	LinkedList<int> UnBindedIDIndexTabel = new LinkedList<int>();

	// Update is called once per frame
	void Update () 
	{
		if (FPSText != null)
		{
			FPSUpdateTime += Time.unscaledDeltaTime;
			if (FPSUpdateTime >= 1.0f && GestureManager.Instance != null)
			{
				FPSUpdateTime = 0.0f;

				FPSText.text = string.Format("MainFPS:{0},Gesture FPS:{1}", MainFPS.FPS, GestureManager.Instance.GetFPS());
			}
		}

		if (MsgText != null && Log.HasNewLogInBuffer)
		{
			MsgText.text = Log.GetBufferLogTotal();
		}
// 
		if (GestureManager.Instance == null)
			return;

		BindedUserIDTabel.Clear();
		UnBindedIDIndexTabel.Clear();
		for ( int i = 0; i < GestureManager.Instance.BindPlayers.Length; ++i)
		{
			if (GestureManager.Instance.BindPlayers[i].IsPlayerUpdating)
			{
				BindedUserIDTabel.Add(GestureManager.Instance.BindPlayers[i].PlayerBindUser.UserID);
				continue;
			}

			UnBindedIDIndexTabel.AddLast(i);
		}

		if (BindedUserIDTabel.Count >= GestureManager.Instance.BindPlayers.Length)
			return;

		Dictionary<int, OrbbecUser>.Enumerator Iter = OrbbecManager.Instance.TrackingUsers.GetEnumerator();

		while (Iter.MoveNext() && UnBindedIDIndexTabel.Count > 0)
		{
			OrbbecUser user = Iter.Current.Value;

			if (BindedUserIDTabel.Contains(user.UserID))
				continue;

			int unbindedIndex = UnBindedIDIndexTabel.First.Value;

			if (!GestureManager.Instance.BindPlayerUserID(user.UserID, unbindedIndex, false))
				continue;

			if (ShowSkeletonPositionController != null)
			{
				ShowSkeletonPositionController.UserDatas[unbindedIndex].SetUser(user);
			}

			UnBindedIDIndexTabel.RemoveFirst();
			Log.Print(Log.Level.Log, string.Format("GestureManager.AddUser({0}) To Index:{1}", Iter.Current.Value.UserID, unbindedIndex));

		}
	}
}
