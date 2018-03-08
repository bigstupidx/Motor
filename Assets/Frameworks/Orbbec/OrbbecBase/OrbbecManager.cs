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
		/// �Ƿ�ʹ���ָ���
		/// </summary>
		public bool IsUseHandsTracker = true;

		/// <summary>
		/// Is Use the User Generator
		/// �Ƿ�ʹ���û�����
		///  </summary>
		public bool IsUseUserGenerator = true;

		// Is Tracking the Skeleton of user, if this is true IsUseUserGenerator will be turned on.
		// �Ƿ���ٹǼ� ���������� �û����ٽ���ǿ�ƿ���
		public bool IsTrackingSkeleton = true;

		/// <summary>
		/// Is Use IR Map
		/// </summary>
		public bool IsUseIR = false;

		/// <summary>
		/// Is Use Depth Map
		/// �Ƿ�������ͼ
		/// </summary>
		public bool IsUseDepth = true;

		/// <summary>
		/// Is Use Scene Depth Map
		/// �Ƿ���³������ͼ�����ڽ������Ϣ�����Դ�����Ⱦʹ�ã�
		/// </summary>
		public bool IsUseSceneDepth = false;

		/// <summary>
		/// Is Use User Image Map.
		/// �Ƿ���²�ɫͼ
		/// </summary>
		public bool IsUseUserImage = true;

		public bool IsUseUVC = false;

		/// <summary>
		/// Is Update Back Removal Texture. PC will use another way to update, just for debug.
		/// �Ƿ���� ��ͼ���� PC�⽫���� ����ķ�ʽ���£���Ϊ���Զ��裬��׷��Ч��
		/// </summary>
		public bool IsUseBackRemoval = false;

		/// <summary>
		/// Is Update User Label Map Texture.
		/// �Ƿ���� �û���ǩ����
		/// </summary>
		public bool IsUseUserLabel = true;

		/// <summary>
		/// Is Use Grab lib, only work on android.
		/// �Ƿ���ץ�ռ�� ���ް�׿��Ч
		/// </summary>
		public bool IsUseCatch = false;

		/// <summary>
		/// Is Exit the application when on pause.
		/// �Ƿ�����ͣ��ʱ�˳�����
		/// </summary>
		public bool IsExitOnPause = true;

		/// <summary>
		/// ��Back������ʱ �Ƿ��˳�
		/// </summary>
		public bool IsExitOnBack = true;

		/// <summary>
		/// Is application never sleep.
		/// �Ƿ�����˯��
		/// </summary>
		public bool IsNeverSleep = true;

        // �Ƿ��������ͼ��Labelͼ��ƫ��������Ĭ�ϲ�����
        public bool IsUseOffsetCorrect = false;

		/// <summary>
		///  �Ƿ�ʹ�� Native ��������
		/// </summary>
#if UNITY_EDITOR
		public bool IsNativeUpdateTexture = false;
#else
		public bool IsNativeUpdateTexture = true;
#endif

		/// <summary>
		/// Is reserve texture data from OpenNI, default will reserve the data to fixed unity.
		/// �Ƿ�ת��ͼ,Ĭ�Ͻ���ת��ƥ��Unity�Ľ��
		/// </summary>
		public bool IsReserveTexture = true;

		/// <summary>
		/// Custom Resolution, this may make something run not normal, just like catch.
		/// �Զ���ֱ��ʣ�����ܽ����һЩ�������в�����
		/// </summary>
		public bool IsCustomResolution = false;

		public CustomResolution CurResolution = new CustomResolution();

		/// <summary>
		/// Hand position will be created by this gesture.
		/// Now it can be "RaiseHand","Wave","Click".
		/// �ֵ㽫ʹ�ô����������� ��ֵ������ "RaiseHand"--̧��,"Wave"--���֣����һ��֣�,"Click"--�����ǰ����֣�.
		/// </summary>
		public string[] HandGesture = {"RaiseHand"};

		/// <summary>
		/// Hand position max number. default is one, this version still have some not fixed bug for more than one hand.
		/// �ֵ�������Ĭ��Ϊһ���ֵ㣬�˰汾�Զ��ִ������ɴ���һЩ��δ���������
		/// </summary>
		public int MaxHandNum = 1;

		public delegate void OnOrbbecInitCallBackFunc();

		public delegate void OnOrbbecInitFailedCallBackFunc();

		public delegate void OnOrbbecExitApp();

		/// <summary>
		/// Callback function,it'll be called when the device is already to initialize.(After USB permission is permitted in android)
		/// �ص����������豸׼����ʼ��ʱ�����(��׿���ڻ��USB��Ȩ�����)
		/// </summary>
		public OnOrbbecInitCallBackFunc OrbbecInitResourceCallBack;


		/// <summary>
		/// Callback function,it'll be called when the device initialize failed.(After USB permission is permitted in android)
		/// �ص����������豸��ʼ��ʧ��ʱ�����(��׿���ڻ��USB��Ȩ�����)
		/// </summary>
		public OnOrbbecInitFailedCallBackFunc OrbbecInitFailedCallBack;

		/// <summary>
		/// Callback function,it'll be called when you try to exit Application.
		/// �ص���������Ӧ���˳�ʱ�����
		/// </summary>
		public OnOrbbecExitApp OnOrbbecExitAppCallBack;
	}

	/// <summary>
	/// Skeleton type.There have different count of skeleton when the low lib is different.
	/// �Ǽ�ö������ �ڲ�ͬ�ײ��֧�� �ɸ��ٵ��ĹǼܵ�ĸ�����������ͬ
	/// </summary>
	public enum SkeletonType
	{
		Invalid			= 0,			 // ��Ч
		Head			= 1,			 // ͷ
		Neck			= 2,			 // ����
		Torso			= 3,			 // ��
		Waist			= 4,			 // ��
		LeftCollar		= 5,			 // ������
		LeftShoulder	= 6,			 // ���
		LeftElbow		= 7,			 // ���ֱ�
		LeftWrist		= 8,			 // ������
		LeftHand		= 9,			 // ������
		LeftFingertip	= 10,			 // ����ָ
		RightCollar		= 11,			 // ������
		RightShoulder	= 12,			 // �Ҽ�
		RightElbow		= 13,			 // ���ֱ�
		RightWrist		= 14,			 // ������
		RightHand		= 15,			 // ������
		RightFingertip	= 16,			 // ����ָ
		LeftHip			= 17,			 // ����
		LeftKnee		= 18,			 // ��ϥ��
		LeftAnkle		= 19,			 // �����
		LeftFoot		= 20,			 // �����
		RightHip		= 21,			 // �ҿ��
		RightKnee		= 22,			 // ��ϥ��
		RightAnkle		= 23,			 // �ҽ���
		RightFoot		= 24,			 // �ҽ���
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
		// ���豸��ɳ�ʼ���� �ᱻ����Ϊtrue��
		public bool IsInited = false;

		// This will be true when the device is ready to call the initlized.
		// ���豸׼����ʼ��ʱ��������Ϊtrue��
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
		/// �û��Ǽܵ�����ŵ� ���� ���¸��� ����ֹͣ�����û�
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
		/// �Ƿ�ÿ֡�����û�����Ļ����
		/// </summary>
		public static bool IsUpdateUserScreenPos = true;

		/// <summary>
		/// The max tracking user number.
		/// ���ĸ����û�����
		/// </summary>
		public int MaxTrackingUserNum = 2;

		/// <summary>
		/// Current tracking user number.
		/// ��ǰ�ĸ����û�����
		/// </summary>
		public int CurTrackingUserNum = 0;

		/// <summary>
		/// The table of tracking user.
		/// �����û����б� �б����ݸ��� ������CurTrackingUserNum����һ�� ���򽫳����߼�����
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
		///  �����˹����൥������ 
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
		///  �����൥��
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
				// ���ֳ��� ��Home������keycode.home����������u3d��׽���¼�����home������Ч
				// Ϊ����Home���ܹ�������Ч�˳��������棬ֻ���ڴ���Ӵ����˳�
				Log.Print(Log.Level.Log, "OpenNI Destroy In Home Start");
				DoDestroy();
				DoExit();
				Log.Print(Log.Level.Log, "OpenNI Destroy In Home End");
			}
		}

		/// <summary>
		/// Destroy the Manager instance, use for close the device.
		/// ���ٴ˹����൥�� ���ڹر��豸
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
		/// �˳�Ӧ�á� �ڰ�׿ һЩ����ʹ��ApplicationQuit()�޷������˳�����ô��Ҫʹ��DoExit������
		/// </summary>
		public static void DoExit()
		{
			if (ms_OnOrbbecExitAppCallBack != null)
				ms_OnOrbbecExitAppCallBack();
			OrbbecWrapper.DoExit();
		}

		/// <summary>
		/// To send a back key press message to android system in order to close screen savers.
		/// ��׿�ײ㷢�ͻ��˼���Ϣ�����ڹر���Ļ���������˳�ʱ��һЩϵͳ���ֺ���(��ʵ����Ļ��������û�������������Ǻ���)������
		/// </summary>
        public static void AndroidVirualBack()
        {
            OrbbecWrapper.AndroidVirualBack();
        }

		/// <summary>
		/// Remove the full screen mode tip.After Android 4.4 many stupid platform will show a terrible tip
		/// to tell user this is a full screen mode. Click ok to close tip.The problem is it only can be close by mouse or touch!!
		/// Use this function to exit the mode(it also full screen too) can close tip.
		/// �Ƴ�ȫ��ģʽ��ʾ���ڰ�׿4.4֮ǰһЩɵXƽ̨����Ӧ�õ�һ�δ򿪵�ʱ�򵯳�һ���ܷ��˵���ʾ�����û�����ȫ��ģʽ
		/// ����ͨ�����OK�رա��������Ϣֻ��ͨ���������رա���ʹ�ô˺����ر�(�رպ���Ȼ��ȫ��)��ʾ��
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
		/// ��ȡ���ͼ������� 
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
		/// ��ȡ���ͼ�ߴ磬 Zֵ�� ��ȵ����ֵ
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
		/// ����ʵ�������� ת������Ļ��ͶӰ������
		/// </summary>
		public Vector3 ConvertRealWorldToProjective(Vector3 worldPos)
		{
			return m_Wrapper.ConvertRealWorldToProjective(worldPos);
		}

		/// <summary>
		/// ����Ļ��ͶӰ������ ת���� ��ʵ��������
		/// </summary>
		/// <param name="projPos">ͶӰ����</param>
		/// <returns>��ʵ��������</returns>
		public Vector3 ConvertProjectiveToRealWorld(Vector3 projPos)
		{
			return m_Wrapper.ConvertProjectiveToRealWorld(projPos);
		}

		/// <summary>
		/// Get User Label Map texture object.
		/// ��ȡ�û���ǩ��Ϣ�������
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
		/// ��ȡ��ɫͼ������� 
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
		/// ��ȡ�����޳���Ĳ�ɫͼ������� 
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
		/// ��ȡ�Ǽ���������
		/// </summary>
		/// <param name="UserID">�û�ID</param>
		/// <param name="Type">�Ǽ�ö������</param>
		/// <param name="Confidence">�������Ŷ�</param>
		/// <returns></returns>
		public Vector3 GetSkeletonWorldPos(int UserID, SkeletonType Type, ref float Confidence)
		{
			return m_Wrapper.GetSkeletonWorldPos( UserID, Type, ref Confidence);
		}

		/// <summary>
		/// ��ȡ�Ǽ���Ļ�ٷֱ�����
		/// </summary>
		/// <param name="UserID">�û�ID</param>
		/// <param name="Type">�Ǽ�ö������</param>
		/// <param name="Confidence">�������Ŷ�</param>
		/// <returns></returns>
		public Vector3 GetSkeletonScreenPercentPos(int UserID, SkeletonType Type, ref float Confidence)
		{
			return m_Wrapper.GetSkeletonScreenPercentPos(UserID, Type, ref Confidence);
		}

		/// <summary>
		/// ��ȡ�Ǽܵ���ת��Ϣ
		/// </summary>
		/// <param name="UserID">�û�ID</param>
		/// <param name="Type">�Ǽ�ö������</param>
		/// <param name="Confidence">�������Ŷ�</param>
		/// <param name="flip">�Ƿ�ת</param>
		/// <returns></returns>
		public Quaternion GetSkeletonRotation(int UserID, SkeletonType Type, ref float Confidence, bool flip)
		{
			return m_Wrapper.GetSkeletonRotation(UserID, Type, ref Confidence, flip);
		}

		/// <summary>
		/// Get all the user list, contain the user which is not tracking.
		/// ��ȡ���б�ʶ���û��б� ����û�н��йǼܸ��ٵ��û�
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
