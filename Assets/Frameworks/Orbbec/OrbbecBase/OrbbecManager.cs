/*
 *		OrbbecManager.cs
 *			
 * 
 *		Create By Sword
 *		2015_05_18				
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Orbbec
{
	[System.Serializable]
	public class CustomResolution
	{
		public int DepthXSize = 320;
		public int DepthYSize = 240;
		public int ImageXSize = 640;
		public int ImageYSize = 480;
	}

	[StructLayout(LayoutKind.Sequential)]
	[System.Serializable]
	public class OrbbecManagerParam
	{
		/// <summary>
		/// Is Use the hands tracker 
		/// 是否使用手跟踪
		/// </summary>
		public bool IsUseHandsTracker = true;

		/// <summary>
		/// Is Use the User Generator
		/// 是否使用用户跟踪
		///  </summary>
		public bool IsUseUserGenerator = true;

		// Is Tracking the Skeleton of user, if this is true IsUseUserGenerator will be turned on.
		// 是否跟踪骨架 如果此项被开启 用户跟踪将被强制开启
		public bool IsTrackingSkeleton = true;

		/// <summary>
		/// Is Use IR Map
		/// </summary>
		public bool IsUseIR = false;

		/// <summary>
		/// Is Use Depth Map
		/// 是否更新深度图
		/// </summary>
		public bool IsUseDepth = true;

		/// <summary>
		/// Is Use Scene Depth Map
		/// 是否更新场景深度图（用于将深度信息传入显存做渲染使用）
		/// </summary>
		public bool IsUseSceneDepth = false;

		/// <summary>
		/// Is Use User Image Map.
		/// 是否更新彩色图
		/// </summary>
		public bool IsUseUserImage = true;

		public bool IsUseUVC = false;

		/// <summary>
		/// Is Update Back Removal Texture. PC will use another way to update, just for debug.
		/// 是否更新 抠图纹理 PC库将采用 另外的方式更新，仅为调试而设，不追求效果
		/// </summary>
		public bool IsUseBackRemoval = false;

		/// <summary>
		/// Is Update User Label Map Texture.
		/// 是否更新 用户标签纹理
		/// </summary>
		public bool IsUseUserLabel = true;

		/// <summary>
		/// Is Use Grab lib, only work on android.
		/// 是否开启抓握检测 仅限安卓有效
		/// </summary>
		public bool IsUseCatch = false;

		/// <summary>
		/// Is Exit the application when on pause.
		/// 是否按下暂停键时退出进程
		/// </summary>
		public bool IsExitOnPause = true;

		/// <summary>
		/// 当Back键按下时 是否退出
		/// </summary>
		public bool IsExitOnBack = true;

		/// <summary>
		/// Is application never sleep.
		/// 是否永不睡眠
		/// </summary>
		public bool IsNeverSleep = true;

        // 是否开启对深度图和Label图的偏移修正，默认不开启
        public bool IsUseOffsetCorrect = false;

		/// <summary>
		///  是否使用 Native 更新纹理
		/// </summary>
#if UNITY_EDITOR
		public bool IsNativeUpdateTexture = false;
#else
		public bool IsNativeUpdateTexture = true;
#endif

		/// <summary>
		/// Is reserve texture data from OpenNI, default will reserve the data to fixed unity.
		/// 是否翻转贴图,默认将翻转来匹配Unity的结果
		/// </summary>
		public bool IsReserveTexture = true;

		/// <summary>
		/// Custom Resolution, this may make something run not normal, just like catch.
		/// 自定义分辨率，这可能将造成一些功能运行不正常
		/// </summary>
		public bool IsCustomResolution = false;

		public CustomResolution CurResolution = new CustomResolution();

		/// <summary>
		/// Hand position will be created by this gesture.
		/// Now it can be "RaiseHand","Wave","Click".
		/// 手点将使用此姿势来创建 其值可以是 "RaiseHand"--抬手,"Wave"--挥手（左右挥手）,"Click"--点击（前后挥手）.
		/// </summary>
		public string[] HandGesture = {"RaiseHand"};

		/// <summary>
		/// Hand position max number. default is one, this version still have some not fixed bug for more than one hand.
		/// 手点数量，默认为一个手点，此版本对多手处理依旧存在一些尚未解决的问题
		/// </summary>
		public int MaxHandNum = 1;

		public delegate void OnOrbbecInitCallBackFunc();

		public delegate void OnOrbbecInitFailedCallBackFunc();

		public delegate void OnOrbbecExitApp();

		/// <summary>
		/// Callback function,it'll be called when the device is already to initialize.(After USB permission is permitted in android)
		/// 回调函数，在设备准备初始化时会调用(安卓会在获得USB授权后调用)
		/// </summary>
		public OnOrbbecInitCallBackFunc OrbbecInitResourceCallBack;


		/// <summary>
		/// Callback function,it'll be called when the device initialize failed.(After USB permission is permitted in android)
		/// 回调函数，在设备初始化失败时会调用(安卓会在获得USB授权后调用)
		/// </summary>
		public OnOrbbecInitFailedCallBackFunc OrbbecInitFailedCallBack;

		/// <summary>
		/// Callback function,it'll be called when you try to exit Application.
		/// 回调函数，在应用退出时会调用
		/// </summary>
		public OnOrbbecExitApp OnOrbbecExitAppCallBack;
	}

	/// <summary>
	/// Skeleton type.There have different count of skeleton when the low lib is different.
	/// 骨架枚举类型 在不同底层库支持 可跟踪到的骨架点的个数会有所不同
	/// </summary>
	public enum SkeletonType
	{
		Invalid			= 0,			 // 无效
		Head			= 1,			 // 头
		Neck			= 2,			 // 脖子
		Torso			= 3,			 // 胸
		Waist			= 4,			 // 腰
		LeftCollar		= 5,			 // 左衣领
		LeftShoulder	= 6,			 // 左肩
		LeftElbow		= 7,			 // 左手臂
		LeftWrist		= 8,			 // 左手腕
		LeftHand		= 9,			 // 左手心
		LeftFingertip	= 10,			 // 左手指
		RightCollar		= 11,			 // 右衣领
		RightShoulder	= 12,			 // 右肩
		RightElbow		= 13,			 // 右手臂
		RightWrist		= 14,			 // 右手腕
		RightHand		= 15,			 // 右手心
		RightFingertip	= 16,			 // 右手指
		LeftHip			= 17,			 // 左胯骨
		LeftKnee		= 18,			 // 左膝盖
		LeftAnkle		= 19,			 // 左脚踝
		LeftFoot		= 20,			 // 左脚心
		RightHip		= 21,			 // 右胯骨
		RightKnee		= 22,			 // 右膝盖
		RightAnkle		= 23,			 // 右脚踝
		RightFoot		= 24,			 // 右脚心
	}

	public enum CatchState
	{
		UnCatch = 0,
		Catching = 2,
		Release = 3,
	}
	public class OrbbecManager : MonoBehaviour
	{
		static OrbbecManager ms_Instance;

		static OrbbecManagerParam.OnOrbbecExitApp ms_OnOrbbecExitAppCallBack;

		#region public member
		// This will be true when the device is initlized.
		// 在设备完成初始化后 会被设置为true。
		public bool IsInited = false;

		// This will be true when the device is ready to call the initlized.
		// 当设备准备初始化时将被设置为true。
		public bool IsDeviceReady = false;

		#region Init Param
		public OrbbecManagerParam Param
		{
			get
			{
				return m_Param;
			}
		}
		#endregion

		#endregion

		#region User Skeleton

		/// <summary>
		/// The Skeleton of the user have the confidence point less than this number will stop tracking.
		/// 用户骨架当其可信点 低于 以下个数 将会停止跟踪用户
		/// </summary>
		public static int LostUserConfidenceNum
		{
			set
			{
				//OrbbecNativeMethods.SetLostUserConfidenceNum(value);
			}
			get
			{
				return OrbbecNativeMethods.GetLostUserConfidenceNum();
			}
		}

		//		public static bool  IsUpdateUserWorldPos = true;

		/// <summary>
		/// Is update the user skeleton screen position per frame.
		/// 是否每帧更新用户的屏幕坐标
		/// </summary>
		public static bool IsUpdateUserScreenPos = true;

		/// <summary>
		/// The max tracking user number.
		/// 最大的跟踪用户数量
		/// </summary>
		public int MaxTrackingUserNum = 2;

		/// <summary>
		/// Current tracking user number.
		/// 当前的跟踪用户数量
		/// </summary>
		public int CurTrackingUserNum = 0;

		/// <summary>
		/// The table of tracking user.
		/// 跟踪用户的列表 列表数据个数 必须与CurTrackingUserNum保持一致 否则将出现逻辑错误
		/// </summary>
		public Dictionary<int, OrbbecUser> TrackingUsers = new Dictionary<int, OrbbecUser>();

		LinkedList<int> m_RemoveUserList = new LinkedList<int>();

		#endregion

		#region User Hand Tracking
		public Dictionary<int, OrbbecHand> TrackingHands = new Dictionary<int, OrbbecHand>();
		#endregion

		delegate void OrbbecUpdateFunc();

		OrbbecWrapper m_Wrapper = new OrbbecWrapper();

		/// <summary>
		/// BackRemoval Blur value, it should between 1~255
		/// </summary>
		public void SetBackRemovalBlurValue(int newValue)
		{
			m_Wrapper.SetBackRemovalBlurValue(newValue);
		}

		/// <summary>
		/// LabelMap Blur value, it should between 0~255
		/// </summary>
		public void SetLabelMapBlurValue(int newValue)
		{
			m_Wrapper.SetLabelMapBlurValue(newValue);
		}

		public void SetStreamFlag(int StreamType, bool Flag)
		{
			m_Wrapper.SetStreamFlag(StreamType, Flag);
		}

		public bool GetStreamFlag(int StreamType)
		{
			return m_Wrapper.GetStreamFlag(StreamType);
		}

		/// <summary>
		///  When the device is ready, it will be call to initlize the custom resource.
		/// </summary>
		public HashSet<int> BackRemovalUserIDFilter = null;

		OrbbecManagerParam m_Param = null;


		OrbbecUpdateFunc m_OrbbecCallback;

		public static bool HasOrbbecDevice()
		{
			return OrbbecWrapper.HasOrbbecDevice();
		}
		
		/// <summary>
		///  Create orbbec manager
		///  创建此管理类单件方法 
		/// </summary>
		public static OrbbecManager CreateOrbbecManager(OrbbecManagerParam param)
		{
			if (ms_Instance != null)
				return ms_Instance;

			GameObject obj = new GameObject("OrbbecManager");

			obj.AddComponent<OrbbecManager>();
			ms_Instance.m_Param = param;
			ms_Instance.Init();

			ms_OnOrbbecExitAppCallBack = param.OnOrbbecExitAppCallBack;

			DontDestroyOnLoad(obj);

			return null;
		}

		/// <summary>
		///  管理类单件
		/// </summary>
		public static OrbbecManager Instance
		{
			get
			{
				return ms_Instance;
			}
		}

		void Awake()
		{
			//Init();
		}

		protected void Init()
		{
			if (null == m_Param)
			{
				m_Param = new OrbbecManagerParam();
			}

			if (m_Param.IsTrackingSkeleton)
			{
				m_Param.IsUseUserGenerator = true;
			}

			if (m_Param.IsUseUVC)
			{
				m_Param.IsUseUserImage = false;
			}

			m_Wrapper.Init(m_Param);

			if (m_Param.IsUseHandsTracker)
			{
				m_Wrapper.OnHandsUpdate = OnHandsUpdate;
			}

			if (ms_Instance.m_Param.IsNeverSleep)
				Screen.sleepTimeout = SleepTimeout.NeverSleep;

			if (m_Param.IsTrackingSkeleton)
			{
				m_Wrapper.UserEnterCallBack += OnUserEnter;
				m_Wrapper.UserLeaveCallBack += OnUserLeave;
				m_Wrapper.UserUpdateCallBack += OnUserUpdate;
			}

			m_OrbbecCallback = WaitForDevice;

		}

		protected OrbbecManager()
		{
			ms_Instance = this;
		}

		void WaitForDevice()
		{
			if (!IsDeviceReady)
				return;

			m_Wrapper.InitResource();
			m_OrbbecCallback = UpdateData;
			IsInited = true;

			if (m_Param != null)
			{
				if (m_Param.OrbbecInitResourceCallBack != null)
					m_Param.OrbbecInitResourceCallBack();
			}
		}

		void UpdateData()
		{
			m_Wrapper.UpdateData();
		}

		void Update()
		{
			if (m_OrbbecCallback == null)
				return;

			m_OrbbecCallback();

			if (m_Param.IsExitOnBack && (Input.GetKeyDown(KeyCode.Escape)))
			{
				//....
				Log.Print(Log.Level.Log, "OpenNI Destroy In Escape Start");
				DoDestroy();
				DoExit();
				Log.Print(Log.Level.Log, "OpenNI Destroy In Escape End");
			}
			else if (Param.IsExitOnPause && Input.GetKeyDown(KeyCode.Home))
			{
				// TO DO:
				// 部分厂商 将Home键当做keycode.home，并且由于u3d捕捉此事件导致home机制无效
				// 为了让Home键能够正常生效退出到主界面，只能在此添加代码退出
				Log.Print(Log.Level.Log, "OpenNI Destroy In Home Start");
				DoDestroy();
				DoExit();
				Log.Print(Log.Level.Log, "OpenNI Destroy In Home End");
			}
		}

		/// <summary>
		/// Destroy the Manager instance, use for close the device.
		/// 销毁此管理类单件 用于关闭设备
		/// </summary>
		public void DoDestroy()
		{
			m_OrbbecCallback = null;
			if (IsInited)
			{
				m_Wrapper.ShutDown();
				IsInited = false;
			}
			GameObject.Destroy(gameObject);
			ms_Instance = null;
		}

		void OnDestroy()
		{
			if (IsInited)
			{
				m_OrbbecCallback = null;
				m_Wrapper.ShutDown();
				IsInited = false;
			}	
		}


		/// <summary>
		/// Do exit the application. in android, 
		/// some machine can't exit normally when call ApplicationQuit() function.
		/// Call the DoExit instead.
		/// 退出应用。 在安卓 一些机器使用ApplicationQuit()无法正常退出，那么需要使用DoExit方法。
		/// </summary>
		public static void DoExit()
		{
			if (ms_OnOrbbecExitAppCallBack != null)
				ms_OnOrbbecExitAppCallBack();
			OrbbecWrapper.DoExit();
		}

		/// <summary>
		/// To send a back key press message to android system in order to close screen savers.
		/// 向安卓底层发送回退键消息。用于关闭屏幕保护避免退出时在一些系统出现黑屏(其实是屏幕保护程序没正常开启挂在那黑了)的问题
		/// </summary>
        public static void AndroidVirualBack()
        {
            OrbbecWrapper.AndroidVirualBack();
        }

		/// <summary>
		/// Remove the full screen mode tip.After Android 4.4 many stupid platform will show a terrible tip
		/// to tell user this is a full screen mode. Click ok to close tip.The problem is it only can be close by mouse or touch!!
		/// Use this function to exit the mode(it also full screen too) can close tip.
		/// 移除全屏模式提示。在安卓4.4之前一些傻X平台会在应用第一次打开的时候弹出一个很烦人的提示告诉用户这是全屏模式
		/// 可以通过点击OK关闭。问题此消息只能通过鼠标或触屏关闭。可使用此函数关闭(关闭后仍然是全屏)提示框
		/// </summary>
		public static void RemoveFullScreenTip()
		{
			OrbbecWrapper.RemoveFullScreenTip();
		}
		
		public void OnApplicationPause(bool pause)
		{
			ms_IsInPause = pause;
			if ( ms_IsHomeKeyDown )
			{
				Log.Print(Log.Level.Log, "OnApplicationPause Exit On Pause");
				Log.Print(Log.Level.Log, "OrbbecManager ShutDown On Pause Start");
				m_Wrapper.ShutDown();
				IsInited = false;
				Log.Print(Log.Level.Log, "OrbbecManager ShutDown On Pause End");

				Log.Print(Log.Level.Log, "DoExit() Start");
				DoExit();
				Log.Print(Log.Level.Log, "DoExit() End");
			}
		}

		static bool ms_IsHomeKeyDown = false;
		static bool ms_IsInPause = false;

		public static void OnHomeKeyDown()
		{
			ms_IsHomeKeyDown = true;
			if (!ms_IsInPause)
			{
				return;
			}

			Log.Print(Log.Level.Log, "OnApplicationPause Exit On Home Down");
			Log.Print(Log.Level.Log, "OrbbecManager ShutDown On Pause Start");
			Instance.m_Wrapper.ShutDown();
			Instance.IsInited = false;
			Log.Print(Log.Level.Log, "OrbbecManager ShutDown On Pause End");


			Log.Print(Log.Level.Log, "DoExit() Start");
			DoExit();
			Log.Print(Log.Level.Log, "DoExit() End");

		}

		public CatchState GetHandCatchState(int HandID)
		{
			if (!Param.IsUseCatch)
				return CatchState.UnCatch;
			return m_Wrapper.GetHandCatchState(HandID);
		}

		#region Map Texture Interface
		/// <summary>
		/// Get Depth Map texture object.
		/// 获取深度图纹理对象 
		/// </summary>
		public Texture2D GetDepthMap()
		{
			return m_Wrapper.GetDepthMap();
		}

		public Texture2D GetSceneDepthMap()
		{
			return m_Wrapper.GetSceneDepthMap();
		}

		/// <summary>
		/// Get Depth Map texture size.The ZSize is the max of depth number.
		/// 获取深度图尺寸， Z值是 深度的最大值
		/// </summary>
		public void GetDepthSize(out int XSize, out int YSize, out int ZSize)
		{
			m_Wrapper.GetDepthSize(out XSize, out YSize, out ZSize);
		}

        public float GetDepthVerticalFoV()
        {
            return m_Wrapper.GetDepthVerticalFoV();
        }

		/// <summary>
		/// 将真实世界坐标 转换成屏幕（投影）坐标
		/// </summary>
		public Vector3 ConvertRealWorldToProjective(Vector3 worldPos)
		{
			return m_Wrapper.ConvertRealWorldToProjective(worldPos);
		}

		/// <summary>
		/// 将屏幕（投影）坐标 转换成 真实世界坐标
		/// </summary>
		/// <param name="projPos">投影坐标</param>
		/// <returns>真实世界坐标</returns>
		public Vector3 ConvertProjectiveToRealWorld(Vector3 projPos)
		{
			return m_Wrapper.ConvertProjectiveToRealWorld(projPos);
		}

		/// <summary>
		/// Get User Label Map texture object.
		/// 获取用户标签信息纹理对象
		/// </summary>
		public Texture2D GetUserLabelMap()
		{
			return m_Wrapper.GetUserLabelMap();
		}

		public short[] GetUserLabel()
		{
			return m_Wrapper.GetUserLabel();
		}

		public short[] GetDepth()
		{
			return m_Wrapper.GetDepth();
		}

		/// <summary>
		/// Get User Image Map texture object.
		/// 获取彩色图纹理对象 
		/// </summary>
		public Texture GetImageMap()
		{
			return m_Wrapper.GetImageMap();
		}

		public Texture2D GetIRMap()
		{
			return m_Wrapper.GetIRMap();
		}

		/// <summary>
		/// Get Back Removed Map texture object.
		/// 获取背景剔除后的彩色图纹理对象 
		/// </summary>
		public Texture GetBackRemovalMap()
		{
			return m_Wrapper.GetBackRemovalMap();
		}
		#endregion

		public bool GetHandInfo(int HandID, ref HandInfo Info)
		{
			return m_Wrapper.GetHandInfo(HandID, ref Info);
		}

		public Vector3 GetOriginHandsPos(int HandID)
		{
			return m_Wrapper.GetOriginHandsPos(HandID);
		}

		public Vector3 GetCurHandsPos(int HandID)
		{
			return m_Wrapper.GetCurHandsPos(HandID);
		}

		public Vector3 GetHandsScreenPercent(int HandID)
		{
			return m_Wrapper.GetHandsScreenPercent(HandID);
		}

		public bool IsTrackingHand(int HandID)
		{
			return m_Wrapper.IsTrackingHand(HandID);
		}

		public void OnHandsUpdate()
		{
			Dictionary<int, OrbbecHand>.Enumerator Elem = TrackingHands.GetEnumerator();
			while (Elem.MoveNext())
			{
				int HandID = Elem.Current.Key;
				if (!IsTrackingHand(HandID))
				{
					Elem.Current.Value.HandID = 0;
				}

				Elem.Current.Value.Update();
			}
		}

		public Vector3 GetUserCOM(int UserID)
		{
			return m_Wrapper.GetUserCOM(UserID);
		}

		public void ClearTrackingUsers()
		{
			TrackingUsers.Clear();
			CurTrackingUserNum = 0;
		}

		void OnUserEnter(int UserID)
		{
			if (CurTrackingUserNum >= MaxTrackingUserNum)
				return;

			OrbbecUser newUser = new OrbbecUser(UserID);
			TrackingUsers.Add( UserID, newUser);

			++CurTrackingUserNum;
		}

		void OnUserLeave(int UserID)
		{
			OrbbecUser curUser;
			if (TrackingUsers.TryGetValue(UserID, out curUser))
			{
				curUser.UserID = 0;
				TrackingUsers.Remove(UserID);
				--CurTrackingUserNum;
			}
		}

		void OnUserUpdate()
		{
			LookForNewUser();

			m_RemoveUserList.Clear();

			Dictionary<int, OrbbecUser>.Enumerator elem = TrackingUsers.GetEnumerator();
			while (elem.MoveNext())
			{
				OrbbecUser cur = elem.Current.Value;
				cur.Update();
				if (!cur.IsInConfidence())
				{
					cur.UserID = 0;
					--CurTrackingUserNum;
					m_RemoveUserList.AddLast(elem.Current.Key);
				}		
			}

			LinkedList<int>.Enumerator removeItem = m_RemoveUserList.GetEnumerator();
			while (removeItem.MoveNext())
			{
				TrackingUsers.Remove(removeItem.Current);
			}
			
		}

		void LookForNewUser()
		{
			if (CurTrackingUserNum >= MaxTrackingUserNum)
				return;

			int[] UserArray = GetUserArray();

			for (int i = 0; i < UserArray.Length && CurTrackingUserNum < MaxTrackingUserNum; ++i)
			{
				int UserID = UserArray[i];
				if (!IsTrackingUser(UserID))
					continue;

				if (TrackingUsers.ContainsKey(UserID))
					continue;

				if (!IsRisingHands(UserID))
					continue;

				OrbbecUser newUser = new OrbbecUser(UserID);
				TrackingUsers.Add(UserID, newUser);

				++CurTrackingUserNum;
			}
		}

		public bool IsRisingHands(int UserID)
		{
			float Confidence = 0.0f;

			Vector3 HeadPos = GetSkeletonWorldPos(UserID, SkeletonType.Head, ref Confidence);
			Vector3 LHandPos = GetSkeletonWorldPos(UserID, SkeletonType.LeftHand, ref Confidence);
			Vector3 RHandPos = GetSkeletonWorldPos(UserID, SkeletonType.RightHand, ref Confidence);

			return LHandPos.y > HeadPos.y && RHandPos.y > HeadPos.y;
		}

		public bool IsTrackingUser(int UserID)
		{
			return m_Wrapper.IsTrackingUser(UserID);
		}

		public bool IsSkeletonAvailable(SkeletonType Type)
		{
			return m_Wrapper.IsSkeletonAvailable(Type);
		}

		[Obsolete("List<SkeletonType> GetAvailableJoints is obsolete. Use SkeletonType[] GetAvailableJointArray Instead.")]
		public List<SkeletonType> GetAvailableJoints()
		{
			List<SkeletonType> list = new List<SkeletonType>(OrbbecWrapper.availableJoints.Length);
			for (int i = 0; i < OrbbecWrapper.availableJoints.Length; ++i )
			{
				list.Add(OrbbecWrapper.availableJoints[i]);
			}		
			return list;
		}

		public SkeletonType[] GetAvailableJointArray()
		{
			return OrbbecWrapper.availableJoints;
		}

		public Dictionary<SkeletonType, int> GetJointToIntDict()
		{
			return OrbbecWrapper.jointToIntDict;
		}

		/// <summary>
		/// 获取骨架世界坐标
		/// </summary>
		/// <param name="UserID">用户ID</param>
		/// <param name="Type">骨架枚举类型</param>
		/// <param name="Confidence">坐标点可信度</param>
		/// <returns></returns>
		public Vector3 GetSkeletonWorldPos(int UserID, SkeletonType Type, ref float Confidence)
		{
			return m_Wrapper.GetSkeletonWorldPos( UserID, Type, ref Confidence);
		}

		/// <summary>
		/// 获取骨架屏幕百分比坐标
		/// </summary>
		/// <param name="UserID">用户ID</param>
		/// <param name="Type">骨架枚举类型</param>
		/// <param name="Confidence">坐标点可信度</param>
		/// <returns></returns>
		public Vector3 GetSkeletonScreenPercentPos(int UserID, SkeletonType Type, ref float Confidence)
		{
			return m_Wrapper.GetSkeletonScreenPercentPos(UserID, Type, ref Confidence);
		}

		/// <summary>
		/// 获取骨架的旋转信息
		/// </summary>
		/// <param name="UserID">用户ID</param>
		/// <param name="Type">骨架枚举类型</param>
		/// <param name="Confidence">坐标点可信度</param>
		/// <param name="flip">是否反转</param>
		/// <returns></returns>
		public Quaternion GetSkeletonRotation(int UserID, SkeletonType Type, ref float Confidence, bool flip)
		{
			return m_Wrapper.GetSkeletonRotation(UserID, Type, ref Confidence, flip);
		}

		/// <summary>
		/// Get all the user list, contain the user which is not tracking.
		/// 获取所有被识别用户列表 包括没有进行骨架跟踪的用户
		/// </summary>
		/// <returns></returns>
		public int[] GetUserArray()
		{
			return m_Wrapper.UserArray;
		}

// 		void OnApplicationQuit()
// 		{
// 			OrbbecNativeMethods.setLogCallBack(null);
// 		}
	}
}
