#define USE_OPENNI_NATIVE_DLL

#if UNITY_ANDROID
#define ANDROID_OPENNI_NATIVE
#else
#define WINDOWS_OPENNI_NATIVE
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

#if USE_OPENNI_NATIVE_DLL
namespace Orbbec
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BufferSize
	{
		public int DepthXSize;
		public int DepthYSize;
		public int DepthZSize;

		public int ImageXSize;
		public int ImageYSize;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct Point3D
	{
		public static Point3D ZeroPoint = new Point3D(0,0,0);

		public Point3D(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static void Vec3ToPoint3(ref Vector3 vec, ref Point3D pos)
		{
			pos.x = vec.x;
			pos.y = vec.y;
			pos.z = vec.z;
		}

		public static void Point3ToVec3(ref Vector3 vec, ref Point3D pos)
		{
			vec.x = pos.x;
			vec.y = pos.y;
			vec.z = pos.z;
		}

		public float x;
		public float y;
		public float z;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct XnQuaternion
	{
		public static XnQuaternion ZeroPoint = new XnQuaternion(0,0,0,0);

		public XnQuaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static void QuatToXnQuat(ref Quaternion quat, ref XnQuaternion xnquat)
		{
			xnquat.x = quat.x;
			xnquat.y = quat.y;
			xnquat.z = quat.z;
			xnquat.w = quat.w;
		}

		public static void XnQuatToQuat(ref Quaternion quat, ref XnQuaternion xnquat)
		{
			quat.x = xnquat.x;
			quat.y = xnquat.y;
			quat.z = xnquat.z;
			quat.w = xnquat.w;
		}

		public float x;
		public float y;
		public float z;
		public float w;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct HandInfo
	{
		public Point3D CurHandsPos;
		public Point3D OriginHandsPos;
		public Point3D ScreenPercent;
	};

	public static class EnumTextureType
	{
		public const int NTT_DEPTH			= 0;
		public const int NTT_LABEL			= 1;
		public const int NTT_IMAGE			= 2;
		public const int NTT_BACKREMOVAL	= 3;
		public const int NTT_SCENEDEPTH		= 4;
		public const int NTT_IR				= 5;
		public const int NTT_Count			= 6;
	}

	public static class SyncMode
	{
		public const int STRICT		= 0;
		public const int COLATION	= 1;
		public const int IGNORE		= 2;
	}

	public static class EnumTextureFormat
	{
		public const int OB_TEX_FORMAT_UNKNOWN	= 0;
		public const int OB_TEX_FORMAT_RGB24	= 1;
		public const int OB_TEX_FORMAT_RGBA		= 2;

		public static int GetFormat(TextureFormat u3dformat)
		{
			switch(u3dformat)
			{
				case TextureFormat.RGB24:
					return OB_TEX_FORMAT_RGB24;
				case TextureFormat.RGBA32:
					return OB_TEX_FORMAT_RGBA;
				default:
					return OB_TEX_FORMAT_UNKNOWN;				
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NativeRefString
	{
		public int Size;
		public IntPtr cPtr;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OrbbecManagerParams
	{
		// Is Use the hands tracker 
		// 是否使用手跟踪
		public int IsUseHandsTracker;

		// Is Use the User Generator
		// 是否使用用户跟踪
		public int IsUseUserGenerator;

		// Is Tracking the Skeleton of user, if this is true IsUseUserGenerator will be turned on.
		// 是否跟踪骨架 如果此项被开启 用户跟踪将被强制开启
		public int IsTrackingSkeleton;

		public int IsUseIR;

		// Is Use Depth Map
		// 是否更新深度图
		public int IsUseDepth;

		// Is Use Scene Depth Map
		// 是否更新场景深度图（用于将深度信息传入显存做渲染使用）
		public int IsUseSceneDepth;

		// Is Use User Image Map.
		// 是否更新彩色图
		public int IsUseUserImage;

		public int IsUseUVC;

		// Is Update Back Removal Texture. PC will use another way to update, just for debug.
		// 是否更新 抠图纹理 PC库将采用 另外的方式更新，仅为调试而设，不追求效果
		public int IsUseBackRemoval;

		// Is Update User Label Map Texture.
		// 是否更新 用户标签纹理
		public int IsUseUserLabel;

		// Is Use Grab lib, only work on android.
		// 是否开启抓握检测 仅限安卓有效
		public int IsUseCatch;

        // 是否开启对深度图和Label图的偏移修正，默认不开启
        public int IsUseOffsetCorrect;

		// 是否使用 Native 更新纹理
		public int IsNativeUpdateTexture;

		// Is reserve texture data from OpenNI, default will reserve the data to fixed unity.
		// 是否翻转贴图,默认将翻转来匹配Unity的结果
		public int IsReserveTexture;

		public int IsCustomResolution;

		public int CustomDepthXSize;
		public int CustomDepthYSize;

		public int CustomImageXSize;
		public int CustomImageYSize;

		// Hand position max number. default is one, this version still have some not fixed bug for more than one hand.
		// 手点数量，默认为一个手点，此版本对多手处理依旧存在一些尚未解决的问题
		public int MaxHandNum;
	};

	public delegate void OrbbecInitCallBack();
	public delegate void OnDeviceRemoveCallBack();
	public delegate void OnEventCallBack();
	public delegate void OnLogCallBack(int lev, string log);

	public delegate void UserEnterFunc(int UserID);
	public delegate void UserLostFunc(int UserID);
	public delegate void OnUserUpdate();

	public delegate void HandCreateFunc(int HandID);
	public delegate void HandLostFunc(int HandID);

	internal class OrbbecNativeMethods
	{
#region Native Function
#if UNITY_EDITOR_64
		private const string dllName = "OrbbecNative64";
#else
		private const string dllName = "OrbbecNative";
#endif

#if UNITY_EDITOR || !UNITY_ANDROID
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool HasOrbbecDevice();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetCurSensorType();
#endif
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetShowLog(bool Flag);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int setInitCallBack(OrbbecInitCallBack init);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int  setOnHomeKeyDownCallBack(OnEventCallBack eventCallBack);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int setLogCallBack(OnLogCallBack eventCallBack);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "InitOrbbecDevice")]
		public static extern bool InitOrbbecDevice(ref OrbbecManagerParams curParams, int HandGestrueSize, IntPtr szHandGestures);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool CloseOrbbecDevice();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitOrbbecRes();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void SettingNativeTextue(int type, IntPtr texHandle, int obFormat, int width, int height);

		// 获取当前的参数实例 可通过修改此参数 实时修改库中设备的运行状态
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetParames(IntPtr curParams);

		#region Buffer Function
		// 获取当前的参数实例 可通过修改此参数 实时修改库中设备的运行状态
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetBufferSize(ref BufferSize curBufferSize);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern float GetDepthVerticalFoV();

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern IntPtr GetDepth();

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern IntPtr GetLabel();

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern IntPtr GetImageRGBTexBuffer();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyDepth(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyLabel(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyImageRGBTexBuffer(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyBackRemovalTexBuffer(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyLabelTexBuffer(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyDepthTexBuffer(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopySceneDepthTexBuffer(IntPtr pData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void CopyIR(IntPtr pData);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern IntPtr GetLabelTexBuffer();

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern IntPtr GetDepthTexBuffer();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void LockBuffer();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void UnLockBuffer();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ConvertRealWorldToProjective(ref Point3D worldPos, ref Point3D projPos);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ConvertProjectiveToRealWorld(ref Point3D worldPos, ref  Point3D projPos);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool ConvertRealWorldToProjectiveArray(int count, Point3D[] worldPos, ref Point3D[] projPos);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool ConvertProjectiveToRealWorldArray(int count, Point3D[] worldPos, ref Point3D[] projPos);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetBackRemovalBlurValue(int newValue);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetLabelMapBlurValue(int newValue);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetBackRemovalFilter(int num, IntPtr filterID);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetStreamFlag(int FlagType, int flag);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetStreamFlag(int FlagType);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool SetIsReserveTexture(int IsReserve);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern float GetUsingTime(int Flag);
		#endregion

		#region User Function
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetLostUserConfidenceNum(int num);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetLostUserConfidenceNum();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetUserNum();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void GetUserArray(IntPtr UserIDArray, ref ushort UserNum);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetUserCOM( int UserID, ref Point3D Position);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool IsTrackingUser(int UserID);

		// ProcessUserData, call this function in main thread looping.
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void ProcessUserData();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SettingUserCallBack(UserEnterFunc EnterCallBack, UserLostFunc LostCallBack);

		// Update the user information. Return is confidence.
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool UpdateUserInfo(int UserID, IntPtr WorldPosArray, IntPtr ProjPosArray, IntPtr RotatorArray);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetAvailableJointNum();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void GetAvailableJoints(IntPtr JointsArray);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetSkeletonWorldPos(ref Point3D Position, int UserID, SkeletonType Type, ref float Confidence);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetSkeletonScreenPercentPos(ref Point3D Position, int UserID, SkeletonType Type, ref float Confidence);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool GetSkeletonRotation(ref XnQuaternion Quat, int UserID, SkeletonType Type, ref float Confidence, bool flip);

#if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CopyMovieData(int ID, IntPtr data, int len);
#endif
		#endregion

		#region Hand Function
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetMaxHandNum(int num);
		
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int  GetMaxHandNum();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int  GetCurHandNum();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SettingHandCallBack(HandCreateFunc EnterCallBack, HandLostFunc LostCallBack);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void ProcessHandsData();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool IsTrackingHand(int HandID);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool GetHandInfo(int HandID, ref HandInfo Info);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetCatchState(int HandID);

		// Travel hands table function.
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool  GetFirstHand(ref int HandID, ref HandInfo Info);
		
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool  GetNextHand(ref int HandID, ref HandInfo Info);
		#endregion

		#region Android UVC Function
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool StartUVCCamera();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void GetUVCData(IntPtr data, bool IsReserved, bool IsSync = false, UInt64 DepthStamp = 0);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool StopUVCCamera();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void GetCameraOriginalData(IntPtr data);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetSaveFramePath(String Path);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SaveUVCFrame(int FrameNum);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetUVCOpenMode(int Flag);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetSyncMode(int Flag);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SaveNIFrame(int FrameNum);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetCurNIFrame();

#if UNITY_ANDROID && !UNITY_EDITOR
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void FrameDepthToImage(int Time);
#endif
		#endregion

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern float GetThreadFPS();

#if UNITY_ANDROID && !UNITY_EDITOR
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern float GetUVCThreadFPS();
#endif

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr GetRenderEventFunc();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void UnityRenderEvent(int eventID);
#endregion
	}

	[System.Serializable]
	public class OrbbecWrapper
	{
		public enum SensorType
		{
			NO_DEFINE_TYPE,
			ASTRA_TYPE,
			ASTRA_S_TYPE,
			ASTRA_PRO_TYPE,
			ASTRA_MINI_TYPE,
		};

#region Interaction
		public CatchState GetHandCatchState(int HandID)
		{
			try
			{
				return (CatchState)(OrbbecNativeMethods.GetCatchState(HandID));
			}
			catch(Exception)
			{
				return CatchState.UnCatch;
			}
		}

		float UpdateYUVPassTime = 0.0f;

		static float ms_MinUpdateYUVTime = 1.0f / 30f;
		#endregion

		private static OrbbecInitCallBack myInitFun = new OrbbecInitCallBack(OnDeviceInit);

		static BufferSize ms_BufferSize = new BufferSize();

		OrbbecManagerParam m_Param;

		static short[] ms_Depth = null;
		static short[] ms_Label = null;

		static byte[] ms_SceneDepth = null;

		public Texture2D m_IRMapTex = null;

		public Texture2D m_DepthMapTex = null;

		public Texture2D m_LabelMapTex = null;

		public Texture2D m_ImageMapTex = null;

		public Texture2D m_BackRemovalTex = null;

		public Texture2D m_SceneDepthTex = null;

		public byte[] IRRGBA		= null;

		public byte[] DepthARGB		= null;
		public byte[] LabelRGBA		= null;

		public byte[] ImageRGB		= null;
		public byte[] ImageYUV422	= null;
		public byte[] BackRemovalImageRGBA		= null;

		static bool m_IsNeedUseUVC = false;
		static bool m_IsNeedUseImage = false;

		public UserEnterFunc UserEnterCallBack;
		public UserLostFunc UserLeaveCallBack;
		public OnUserUpdate UserUpdateCallBack;

		public static OnEventCallBack OnHomeKeyDownCallBack = null;

		// Renew per frame.
		public int[] UserArray = new int[0];

		public static SkeletonType[]			availableJoints;
		public static Dictionary<SkeletonType, int> jointToIntDict;
		public static int countAvailableSkeleton;

		int[] BackRemovalFilterArray = new int[0];

		public static float GetUsingTimeForTest(int Flag)
		{
#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			if (Flag < 0)
			{
				System.Object[] args = new System.Object[0];
				
				return activity.Call<float>("GetUVCDataUpdateTime", args);
			}

			// DEPTH,			0
			// LABEL,			1
			// RGB,				2
			// BACKREMOVAL,		3
			// V4L2UpdateTime,	4
#endif

			return OrbbecNativeMethods.GetUsingTime(Flag);
		}

		public static bool HasOrbbecDevice()
		{
#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			try
			{
				if (activity == null)
				{
					g_jc = new AndroidJavaClass("com.orbbec.unityadapt.UnityAdaptActivity");
					activity = g_jc.GetStatic<AndroidJavaObject>("m_Instance");
					Log.Print( Log.Level.Log, "Get the activity!");
				}

				System.Object[] args = new System.Object[0];
				return activity.Call<bool>("HasOrbbecDevice", args);

			}
			catch (Exception e)
			{
				Log.Print(Log.Level.Error, e.ToString());
			}
			return false;
#else
			return OrbbecNativeMethods.HasOrbbecDevice();
#endif
		}

		static void OnDeviceInit()
		{
			SettingsIsUVC();

			CheckAndroidSDKVersion();

			m_IsNeedUseUVC = OrbbecManager.Instance.Param.IsUseUVC;
			m_IsNeedUseImage = OrbbecManager.Instance.Param.IsUseUserImage
								|| OrbbecManager.Instance.Param.IsUseCatch
								|| OrbbecManager.Instance.Param.IsUseBackRemoval;

			

			InitOpenNIConext();

			if (m_IsNeedUseUVC && m_IsNeedUseImage)
			{
				InitUVCDevice();
			}
		}

		static void InitOpenNIConext()
		{
			OrbbecManagerParam curParam = OrbbecManager.Instance.Param;
			OrbbecManagerParams nativeParam = new OrbbecManagerParams();

			nativeParam.IsUseHandsTracker	= Convert.ToInt32(curParam.IsUseHandsTracker);
			nativeParam.IsUseUserGenerator	= Convert.ToInt32(curParam.IsUseUserGenerator);
			nativeParam.IsTrackingSkeleton	= Convert.ToInt32(curParam.IsTrackingSkeleton);
			nativeParam.IsUseIR				= Convert.ToInt32(curParam.IsUseIR);
			nativeParam.IsUseDepth			= Convert.ToInt32(curParam.IsUseDepth);
			nativeParam.IsUseSceneDepth		= Convert.ToInt32(curParam.IsUseSceneDepth);
			
			nativeParam.IsUseUserImage		= Convert.ToInt32(curParam.IsUseUserImage);
			nativeParam.IsUseUVC			= Convert.ToInt32(curParam.IsUseUVC);
			nativeParam.IsUseBackRemoval	= Convert.ToInt32(curParam.IsUseBackRemoval);
			
			nativeParam.IsUseUserLabel		= Convert.ToInt32(curParam.IsUseUserLabel);
			nativeParam.IsUseCatch			= Convert.ToInt32(curParam.IsUseCatch);
			nativeParam.IsCustomResolution	= Convert.ToInt32(curParam.IsCustomResolution);
            nativeParam.IsUseOffsetCorrect	= Convert.ToInt32(curParam.IsUseOffsetCorrect);
			nativeParam.IsNativeUpdateTexture = Convert.ToInt32(curParam.IsNativeUpdateTexture);
			nativeParam.IsReserveTexture	= Convert.ToInt32(curParam.IsReserveTexture);	

			nativeParam.CustomDepthXSize	= curParam.CurResolution.DepthXSize;
			nativeParam.CustomDepthYSize	= curParam.CurResolution.DepthYSize;

			nativeParam.CustomImageXSize	= curParam.CurResolution.ImageXSize;
			nativeParam.CustomImageYSize	= curParam.CurResolution.ImageYSize;

			nativeParam.MaxHandNum			= curParam.MaxHandNum;

			NativeRefString[] stringPtr = new NativeRefString[curParam.HandGesture.Length];
			char[][] charArray = new char[curParam.HandGesture.Length][];
		
			for (int i = 0; i < stringPtr.Length; ++i )
			{
				charArray[i] = curParam.HandGesture[i].ToCharArray();
				Log.Print(Log.Level.Log, "add hand gesture:" + curParam.HandGesture[i]);
				stringPtr[i].Size = curParam.HandGesture[i].Length;
				stringPtr[i].cPtr = Marshal.UnsafeAddrOfPinnedArrayElement(charArray[i], 0);  //Marshal.StringToBSTR(curParam.HandGesture[i]);
			}

			IntPtr stringPtrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(stringPtr, 0);

			OrbbecManager.Instance.IsDeviceReady = OrbbecNativeMethods.InitOrbbecDevice(ref nativeParam, curParam.HandGesture.Length, stringPtrPtr);
			//OrbbecManager.Instance.IsInited = OrbbecNativeMethods.InitOrbbecDevice(ref nativeParam, curParam.HandGesture.Length, stringPtrPtr);

			if (!OrbbecManager.Instance.IsDeviceReady && curParam.OrbbecInitFailedCallBack != null)
			{
				curParam.OrbbecInitFailedCallBack();
			}
		}

	

		public static void DoLogCallBack(int lev, string str)
		{
			Log.Print((Log.Level)lev, str);
		}

		public void Init(OrbbecManagerParam Param)
		{
			m_Param = Param;

			// This is a danger way to log, it is only for debug. May be crash in some situation,but may be
			// helpful for debug before the application died!
			// 这是个危险的方式来打印日志。仅限用于调试。在某些状态下可能会崩溃，但是在应用崩溃前可能有助于与查错
#if LOG_NATIVE_LOG_TO_UNITY //|| UNITY_EDITOR
			OrbbecNativeMethods.setLogCallBack(DoLogCallBack);
#endif

			if (Param.IsExitOnPause)
				OnHomeKeyDownCallBack = OrbbecManager.OnHomeKeyDown;

			OrbbecNativeMethods.setOnHomeKeyDownCallBack(OnHomeKeyDownCallBack);

#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			try
			{
				OrbbecNativeMethods.setInitCallBack(myInitFun);
				orbbecInitAll();
			}
			catch( Exception e)
			{
				Log.Print(Log.Level.Error, e.ToString());
				if (myInitFun != null)
					myInitFun();
			}
			
#else
			OnDeviceInit();
#endif
		}

		private static OrbbecInitCallBack GetPermissionCallBack = new OrbbecInitCallBack(OnDeviceInit);

#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
		private static AndroidJavaClass g_jc;
		private static AndroidJavaObject activity;
#endif

		public static void orbbecInitAll()
		{
#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			try
			{
				//g_jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				if (activity == null)
				{
					g_jc = new AndroidJavaClass("com.orbbec.unityadapt.UnityAdaptActivity");
					activity = g_jc.GetStatic<AndroidJavaObject>("m_Instance");
					Log.Print( Log.Level.Log, "Get the activity!");
				}

				System.Object[] args = new System.Object[0];
				activity.Call("requestGrant", args);
				Log.Print( Log.Level.Log, "Finish requestGrant!");
			}
			catch (Exception e)
			{
				Log.Print(Log.Level.Error, e.ToString());
			}
#endif
		}

		static void SettingsIsUVC()
		{
#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			Log.Print(Log.Level.Log, "GetCurSensorType");
			System.Object[] args = new System.Object[0];
			SensorType curType = (SensorType)activity.Call<int>("GetCurSensorType", args);

			Log.Print(Log.Level.Log, "GetCurSensorType, Type :" + curType.ToString());

			OrbbecManager.Instance.Param.IsUseUVC = /*curType == SensorType.ASTRA_PRO_S_TYPE ||*/ curType == SensorType.ASTRA_PRO_TYPE;
#else
			SensorType curType = (SensorType)OrbbecNativeMethods.GetCurSensorType();
			Log.Print(Log.Level.Log, "GetCurSensorType, Type :" + curType.ToString());
			OrbbecManager.Instance.Param.IsUseUVC = /*curType == SensorType.ASTRA_PRO_S_TYPE ||*/ curType == SensorType.ASTRA_PRO_TYPE;
#endif
		}

#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
		static int AndroidSDKVersion = 0;
#endif

		static void CheckAndroidSDKVersion()
		{
#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			Log.Print(Log.Level.Log, "CheckAndroidSDKVersion");
			System.Object[] args = new System.Object[0];
			AndroidSDKVersion = activity.Call<int>("GetSDKVersion", args);

			Log.Print(Log.Level.Log, "Cur Android SDK API Version :" + AndroidSDKVersion);

			if (AndroidSDKVersion >= 21) // Android 5.0 +
			{
				// On Android 5.0 the depth only support 160x120 other resolution may crash!!
// 				if (OrbbecManager.Instance.Param.IsCustomResolution)
// 				{
// 					OrbbecManager.Instance.Param.CurResolution.DepthXSize = 160;
// 					OrbbecManager.Instance.Param.CurResolution.DepthYSize = 120;
// 				}
// 				else
// 				{
// 					OrbbecManager.Instance.Param.IsCustomResolution = true;
// 					OrbbecManager.Instance.Param.CurResolution.ImageXSize = 320;
// 					OrbbecManager.Instance.Param.CurResolution.ImageYSize = 240;
// 					OrbbecManager.Instance.Param.CurResolution.DepthXSize = 160;
// 					OrbbecManager.Instance.Param.CurResolution.DepthYSize = 120;
// 				}
			}

			
#endif
		}

		static void InitUVCDevice()
		{
			// UVC Resolution can't be get, but can be setting here.
			if (OrbbecManager.Instance.Param.IsCustomResolution && !OrbbecManager.Instance.Param.IsUseCatch)
			{
				ms_BufferSize.ImageXSize = OrbbecManager.Instance.Param.CurResolution.ImageXSize;
				ms_BufferSize.ImageYSize = OrbbecManager.Instance.Param.CurResolution.ImageYSize;
			}
			else
			{
				ms_BufferSize.ImageXSize = 640;
				ms_BufferSize.ImageYSize = 480;
			}

#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			if (activity != null)
			{
				System.Object[] args = new System.Object[2];
				args[0] = ms_BufferSize.ImageXSize;
				args[1] = ms_BufferSize.ImageYSize;

				Log.Print( Log.Level.Log, "Do CreateUVCCamera");
				int rt = activity.Call<int>("CreateUVCCamera", args);
				if (rt != 0)
				{
					OrbbecNativeMethods.StartUVCCamera();
				}
			}
			else
			{
				Log.Print( Log.Level.Log, "activity == null");
			}
#else

#endif
		}

		public void InitResource()
		{
			OrbbecNativeMethods.GetBufferSize(ref ms_BufferSize);

			ms_Depth = new short[ms_BufferSize.DepthXSize * ms_BufferSize.DepthYSize];
			ms_Label = new short[ms_BufferSize.DepthXSize * ms_BufferSize.DepthYSize];

			if (m_Param.IsUseIR)
			{
				if (!m_Param.IsNativeUpdateTexture)
					IRRGBA = new byte[ms_BufferSize.DepthXSize * ms_BufferSize.DepthYSize * 4];
			}

			if (m_Param.IsUseDepth)
			{
				if (!m_Param.IsNativeUpdateTexture)
					DepthARGB = new byte[ms_BufferSize.DepthXSize * ms_BufferSize.DepthYSize * 4];
			}

			if (m_Param.IsUseUserLabel)
			{
				if (!m_Param.IsNativeUpdateTexture)
					LabelRGBA = new byte[ms_BufferSize.DepthXSize * ms_BufferSize.DepthYSize * 4];
			}
			
			if (m_Param.IsUseCatch)
			{
				if (!m_Param.IsNativeUpdateTexture)
					ImageYUV422 = new byte[ms_BufferSize.ImageXSize * ms_BufferSize.ImageYSize * 2];
			}

			if (m_Param.IsUseUserImage || m_Param.IsUseBackRemoval)
			{
				if (!m_Param.IsNativeUpdateTexture)
					ImageRGB = new byte[ms_BufferSize.ImageXSize * ms_BufferSize.ImageYSize * 3];
			}

			if (m_Param.IsUseBackRemoval)
			{
				if (!m_Param.IsNativeUpdateTexture)
					BackRemovalImageRGBA = new byte[ms_BufferSize.ImageXSize * ms_BufferSize.ImageYSize * 4];
			}

			m_DepthMapTex = new Texture2D(ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize, TextureFormat.RGBA32, false);
			m_DepthMapTex.wrapMode = TextureWrapMode.Clamp;

			m_LabelMapTex = new Texture2D( ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize, TextureFormat.RGBA32, false);
			m_LabelMapTex.wrapMode = TextureWrapMode.Clamp;

			if (m_Param.IsUseSceneDepth)
			{
				ms_SceneDepth = new byte[ms_BufferSize.DepthXSize * ms_BufferSize.DepthYSize * 4];
				m_SceneDepthTex = new Texture2D(ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize, TextureFormat.RGBA32, false);
				m_SceneDepthTex.wrapMode = TextureWrapMode.Clamp;
				m_SceneDepthTex.filterMode = FilterMode.Point;
			}

			m_ImageMapTex = new Texture2D(ms_BufferSize.ImageXSize, ms_BufferSize.ImageYSize, TextureFormat.RGB24, false);
			m_ImageMapTex.wrapMode = TextureWrapMode.Clamp;

			m_BackRemovalTex = new Texture2D(ms_BufferSize.ImageXSize, ms_BufferSize.ImageYSize, TextureFormat.RGBA32, false);
			m_BackRemovalTex.wrapMode = TextureWrapMode.Clamp;

			m_IRMapTex = new Texture2D(ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize, TextureFormat.RGBA32, false);
			m_IRMapTex.wrapMode = TextureWrapMode.Clamp;
			m_IRMapTex.filterMode = FilterMode.Point;

			if (m_Param.IsNativeUpdateTexture)
			{
				if (m_DepthMapTex)
				{
					OrbbecNativeMethods.SettingNativeTextue(EnumTextureType.NTT_DEPTH,
																		m_DepthMapTex.GetNativeTexturePtr(),
																		EnumTextureFormat.GetFormat(m_DepthMapTex.format),
																		ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize);
				}
				
				if (m_LabelMapTex)
				{
					OrbbecNativeMethods.SettingNativeTextue(EnumTextureType.NTT_LABEL,
													m_LabelMapTex.GetNativeTexturePtr(),
													EnumTextureFormat.GetFormat(m_LabelMapTex.format),
													ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize);
				}

				if (m_ImageMapTex)
				{
					OrbbecNativeMethods.SettingNativeTextue(EnumTextureType.NTT_IMAGE,
													m_ImageMapTex.GetNativeTexturePtr(),
													EnumTextureFormat.GetFormat(m_ImageMapTex.format),
													ms_BufferSize.ImageXSize, ms_BufferSize.ImageYSize);
				}

				if (m_BackRemovalTex)
				{
					OrbbecNativeMethods.SettingNativeTextue(EnumTextureType.NTT_BACKREMOVAL,
													m_BackRemovalTex.GetNativeTexturePtr(),
													EnumTextureFormat.GetFormat(m_BackRemovalTex.format),
													ms_BufferSize.ImageXSize, ms_BufferSize.ImageYSize);
				}
				

				if (m_SceneDepthTex)
				{
					OrbbecNativeMethods.SettingNativeTextue(EnumTextureType.NTT_SCENEDEPTH,
																		m_SceneDepthTex.GetNativeTexturePtr(),
																		EnumTextureFormat.GetFormat(m_SceneDepthTex.format),
																		ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize);
				}

				if (m_IRMapTex)
				{
					OrbbecNativeMethods.SettingNativeTextue(EnumTextureType.NTT_IR,
																		m_IRMapTex.GetNativeTexturePtr(),
																		EnumTextureFormat.GetFormat(m_IRMapTex.format),
																		ms_BufferSize.DepthXSize, ms_BufferSize.DepthYSize);
				}

				OrbbecManager.Instance.StartCoroutine(CallPluginAtEndOfFrames());
			}

			Log.Print( Log.Level.Log, "Do InitOrbbecRes");
			OrbbecNativeMethods.InitOrbbecRes();

			Log.Print(Log.Level.Log, "Do InitUserData");
			InitUserData();

			Log.Print(Log.Level.Log, "Do InitHandData");
			InitHandData();
		}

		void UpdateSceneDepthMap()
		{
			if (!m_Param.IsUseSceneDepth || !GetStreamFlag(EnumTextureType.NTT_SCENEDEPTH))
			{
				return;
			}
			/*
			int ByteIndex = 0;
			for (int i = 0; i < ms_Depth.Length; ++i)
			{
				short depth = ms_Depth[ms_Depth.Length - 1 - i];
				ms_SceneDepth[ByteIndex++] = (byte)(depth >> 8);
				ms_SceneDepth[ByteIndex++] = (byte)(depth & 0xFF);
				ms_SceneDepth[ByteIndex++] = 0;
				ms_SceneDepth[ByteIndex++] = 0xFF;
			}
			*/

			IntPtr headptr = Marshal.UnsafeAddrOfPinnedArrayElement(ms_SceneDepth, 0);
			OrbbecNativeMethods.CopySceneDepthTexBuffer(headptr);

			m_SceneDepthTex.LoadRawTextureData(ms_SceneDepth);
			m_SceneDepthTex.Apply();
		}

        public static void AndroidVirualBack()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
			if (activity == null)
				return;

			System.Object[] args = new System.Object[0];
			activity.Call("VirualBackKeyClick", args);
#endif
        }

		public static void RemoveFullScreenTip()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			if (activity == null)
				return;

			System.Object[] args = new System.Object[0];
			activity.Call("RemoveFullScreenTip", args);
#endif
		}

		void UpdateCSharpTextureBuffer()
		{
			IntPtr headptr;

			if (m_Param.IsUseIR && GetStreamFlag(EnumTextureType.NTT_IR))
			{
				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(IRRGBA, 0);
				OrbbecNativeMethods.CopyIR(headptr);
				//ptr = OrbbecNativeMethods.GetDepthTexBuffer();
				//if (ptr != IntPtr.Zero)
				//	Marshal.Copy( ptr, DepthARGB, 0, DepthARGB.Length);
				m_IRMapTex.LoadRawTextureData(IRRGBA);
				m_IRMapTex.Apply();
			}

			if (m_Param.IsUseDepth && GetStreamFlag(EnumTextureType.NTT_DEPTH))
			{
				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(DepthARGB, 0);
				OrbbecNativeMethods.CopyDepthTexBuffer(headptr);
				//ptr = OrbbecNativeMethods.GetDepthTexBuffer();
				//if (ptr != IntPtr.Zero)
				//	Marshal.Copy( ptr, DepthARGB, 0, DepthARGB.Length);
				m_DepthMapTex.LoadRawTextureData(DepthARGB);
				m_DepthMapTex.Apply();
			}

			if (m_Param.IsUseUserLabel && GetStreamFlag(EnumTextureType.NTT_LABEL))
			{
				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(LabelRGBA, 0);
				OrbbecNativeMethods.CopyLabelTexBuffer(headptr);
				//ptr = OrbbecNativeMethods.GetLabelTexBuffer();
				//if (ptr != IntPtr.Zero)
				//	Marshal.Copy(ptr, LabelRGBA, 0, LabelRGBA.Length);
				m_LabelMapTex.LoadRawTextureData(LabelRGBA);
				m_LabelMapTex.Apply();
			}

			if (m_Param.IsUseUserImage && GetStreamFlag(EnumTextureType.NTT_IMAGE))
			{
				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(ImageRGB, 0);
				OrbbecNativeMethods.CopyImageRGBTexBuffer(headptr);

				//ptr = OrbbecNativeMethods.GetImageRGBTexBuffer();
				//if (ptr != IntPtr.Zero)
				//	Marshal.Copy( ptr, ImageRGB, 0, ImageRGB.Length);
				m_ImageMapTex.LoadRawTextureData(ImageRGB);
				m_ImageMapTex.Apply();
			}

			if (m_Param.IsUseBackRemoval && GetStreamFlag(EnumTextureType.NTT_BACKREMOVAL))
			{
				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(BackRemovalImageRGBA, 0);
				OrbbecNativeMethods.CopyBackRemovalTexBuffer(headptr);

				m_BackRemovalTex.LoadRawTextureData(BackRemovalImageRGBA);
				m_BackRemovalTex.Apply();
				if (OrbbecManager.Instance.BackRemovalUserIDFilter != null)
				{
					if (BackRemovalFilterArray.Length != OrbbecManager.Instance.BackRemovalUserIDFilter.Count)
						BackRemovalFilterArray = new int[OrbbecManager.Instance.BackRemovalUserIDFilter.Count];
					OrbbecManager.Instance.BackRemovalUserIDFilter.CopyTo(BackRemovalFilterArray);

					IntPtr filter = Marshal.UnsafeAddrOfPinnedArrayElement(BackRemovalFilterArray, 0);

					OrbbecNativeMethods.SetBackRemovalFilter(BackRemovalFilterArray.Length, filter);
				}
				else
				{
					OrbbecNativeMethods.SetBackRemovalFilter(0, IntPtr.Zero);
				}
			}

			UpdateSceneDepthMap();
		}

		public void UpdateData()
		{
			try
			{
				IntPtr headptr;
				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(ms_Depth, 0);
				OrbbecNativeMethods.CopyDepth(headptr);
				//IntPtr ptr = OrbbecNativeMethods.GetDepth();
				//if (ptr != IntPtr.Zero)
				//	Marshal.Copy( ptr, ms_Depth, 0, ms_Depth.Length);

				headptr = Marshal.UnsafeAddrOfPinnedArrayElement(ms_Label, 0);
				OrbbecNativeMethods.CopyLabel(headptr);
				//ptr = OrbbecNativeMethods.GetLabel();
				//if (ptr != IntPtr.Zero)
				//	Marshal.Copy( ptr, ms_Label, 0, ms_Label.Length);

				if (m_Param.IsNativeUpdateTexture)
				{
					if (m_Param.IsUseBackRemoval)
					{
						if (OrbbecManager.Instance.BackRemovalUserIDFilter != null)
						{
							if (BackRemovalFilterArray.Length != OrbbecManager.Instance.BackRemovalUserIDFilter.Count)
								BackRemovalFilterArray = new int[OrbbecManager.Instance.BackRemovalUserIDFilter.Count];
							OrbbecManager.Instance.BackRemovalUserIDFilter.CopyTo(BackRemovalFilterArray);

							IntPtr filter = Marshal.UnsafeAddrOfPinnedArrayElement(BackRemovalFilterArray, 0);

							OrbbecNativeMethods.SetBackRemovalFilter(BackRemovalFilterArray.Length, filter);
						}
						else
						{
							OrbbecNativeMethods.SetBackRemovalFilter(0, IntPtr.Zero);
						}
					}
				}
				else
				{
					UpdateCSharpTextureBuffer();
				}
				

				GetUserArray(ref UserArray);

				if (m_Param.IsUseUserGenerator)
				{
					OrbbecNativeMethods.ProcessUserData();
					if (UserUpdateCallBack != null)
						UserUpdateCallBack();
				}

				if (m_Param.IsUseHandsTracker)
				{
					UpdateHandsData();
				}
			}
			catch( Exception ex)
			{
				Log.Print(Log.Level.Error, ex.ToString());
			}
			
		}

		#region CShare Buffer Function
		public Texture2D GetDepthMap()
		{
			return m_DepthMapTex;
		}

		public Texture2D GetSceneDepthMap()
		{
			return m_SceneDepthTex;
		}

		public short[] GetDepth()
		{
			return ms_Depth;
		}

		public void GetDepthSize(out int XSize, out int YSize, out int ZSize)
		{
			XSize = ms_BufferSize.DepthXSize;
			YSize = ms_BufferSize.DepthYSize;
			ZSize = ms_BufferSize.DepthZSize;
		}

		public float GetDepthVerticalFoV()
		{
			return OrbbecNativeMethods.GetDepthVerticalFoV();
		}

		public Vector3 ConvertRealWorldToProjective(Vector3 worldPos)
		{
			Point3D worldP3Pos = Point3D.ZeroPoint;
			Point3D projP3Pos = Point3D.ZeroPoint;
			Vector3 projPos = Vector3.zero;

			Point3D.Vec3ToPoint3(ref worldPos, ref worldP3Pos);

			if (!OrbbecNativeMethods.ConvertRealWorldToProjective(ref worldP3Pos, ref projP3Pos))
				return projPos;

			Point3D.Point3ToVec3(ref projPos, ref projP3Pos);
			return projPos;
		}

		public Vector3 ConvertProjectiveToRealWorld(Vector3 projPos)
		{
			Point3D worldP3Pos = Point3D.ZeroPoint;
			Point3D projP3Pos = Point3D.ZeroPoint;
			Vector3 worldPos = Vector3.zero;

			Point3D.Vec3ToPoint3(ref projPos, ref projP3Pos);

			if (!OrbbecNativeMethods.ConvertProjectiveToRealWorld(ref worldP3Pos, ref projP3Pos))
				return projPos;

			Point3D.Point3ToVec3(ref worldPos, ref worldP3Pos);
			return worldPos;
		}

		public Texture2D GetUserLabelMap()
		{
			return m_LabelMapTex;
		}

		public short[] GetUserLabel()
		{
			return ms_Label;
		}

		public Texture2D GetImageMap()
		{
			return m_ImageMapTex;
		}

		public Texture2D GetIRMap()
		{
			return m_IRMapTex;
		}

		public Texture2D GetBackRemovalMap()
		{
			return m_BackRemovalTex;
		}

		public static int BackRemovalBlurValue
		{ 
			get
			{ 
				return ms_BackRemovalBlurValue;
			}
		}

		static int ms_BackRemovalBlurValue = 7;
		public static bool IsUseNeon = true;

		public static int LabelMapBlurValue
		{ 
			get
			{
				return ms_LabelMapBlurValue;
			}
		}

		static int ms_LabelMapBlurValue = 0;

		public void SetBackRemovalBlurValue(int newValue)
		{
			ms_BackRemovalBlurValue = newValue;
			OrbbecNativeMethods.SetBackRemovalBlurValue(newValue);
		}

		public void SetLabelMapBlurValue(int newValue)
		{
			ms_LabelMapBlurValue = newValue;
			OrbbecNativeMethods.SetLabelMapBlurValue(newValue);
		}

		public void SetStreamFlag(int StreamType, bool Flag)
		{
			int flag = Flag?1:0;
			OrbbecNativeMethods.SetStreamFlag(StreamType, flag);
		}

		public bool GetStreamFlag(int StreamType)
		{
			return OrbbecNativeMethods.GetStreamFlag(StreamType) != 0;
		}
		#endregion

		#region CShare User Function
		public void InitUserData()
		{
			if (!m_Param.IsTrackingSkeleton)
				return;

			countAvailableSkeleton = OrbbecNativeMethods.GetAvailableJointNum();

			availableJoints = new SkeletonType[countAvailableSkeleton];

			IntPtr jointPtr = Marshal.UnsafeAddrOfPinnedArrayElement(availableJoints, 0);
			OrbbecNativeMethods.GetAvailableJoints(jointPtr);

			jointToIntDict = new Dictionary<SkeletonType,int>();

			for (int i = 0; i < availableJoints.Length; ++i )
			{
				jointToIntDict.Add(availableJoints[i], i);
			}

			OrbbecNativeMethods.SettingUserCallBack(UserEnterCallBack, UserLeaveCallBack);
		}

		public bool IsSkeletonAvailable(SkeletonType Type)
		{
			return jointToIntDict.ContainsKey(Type);
		}

		

		public Vector3 GetUserCOM(int UserID)
		{
			Vector3 v3Pos = Vector3.zero;
			Point3D pos = Point3D.ZeroPoint;
			if (OrbbecNativeMethods.GetUserCOM( UserID, ref pos))
			{
				v3Pos.x = pos.x;
				v3Pos.y = pos.y;
				v3Pos.z = pos.z;
			}

			return v3Pos;
		}

		public bool IsTrackingUser(int UserID)
		{
			return OrbbecNativeMethods.IsTrackingUser(UserID);
		}

		public Vector3 GetSkeletonWorldPos(int UserID, SkeletonType Type, ref float Confidence)
		{
			Point3D pos = Point3D.ZeroPoint;
			OrbbecNativeMethods.GetSkeletonWorldPos(ref pos, UserID, Type, ref Confidence);

			Vector3 rt = Vector3.zero;
			Point3D.Point3ToVec3(ref rt, ref pos);

			return rt;
		}

		public Vector3 GetSkeletonScreenPercentPos(int UserID, SkeletonType Type, ref float Confidence)
		{
			Point3D pos = Point3D.ZeroPoint;
			OrbbecNativeMethods.GetSkeletonScreenPercentPos(ref pos, UserID, Type, ref Confidence);

			Vector3 rt = Vector3.zero;
			Point3D.Point3ToVec3(ref rt, ref pos);

			return rt;
		}

		public Quaternion GetSkeletonRotation(int UserID, SkeletonType Type, ref float Confidence, bool flip)
		{
			XnQuaternion XnQuat = XnQuaternion.ZeroPoint;
			OrbbecNativeMethods.GetSkeletonRotation(ref XnQuat, UserID, Type, ref Confidence, flip);
			Quaternion Quat = Quaternion.identity;

			XnQuaternion.XnQuatToQuat(ref Quat, ref XnQuat);

			return Quat;
		}

		public int[] GetUserArray()
		{
			ushort curNum = (ushort)OrbbecNativeMethods.GetUserNum();
			int[] userArray = new int[curNum];

			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(userArray, 0);

			OrbbecNativeMethods.GetUserArray(ptr, ref curNum);

			return userArray;
		}

		public void GetUserArray(ref  int[] userArray)
		{
			ushort curNum = (ushort)OrbbecNativeMethods.GetUserNum();
			if (userArray == null || userArray.Length != curNum)
				userArray = new int[curNum];
			
			IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(userArray, 0);

			OrbbecNativeMethods.GetUserArray(ptr, ref curNum);

			return;
		}
		#endregion

		#region CShare Hand Function

		public delegate void OnHandsUpdateFunc();
		public OnHandsUpdateFunc OnHandsUpdate;

		static void OnHandCreate( int UserID)
		{
			OrbbecManager.Instance.TrackingHands.Add(UserID, new OrbbecHand(UserID));
		}

		static void OnHandLost(int UserID)
		{
			OrbbecManager.Instance.TrackingHands.Remove(UserID);
		}

		public bool GetHandInfo(int HandID, ref HandInfo Info)
		{
			return OrbbecNativeMethods.GetHandInfo(HandID, ref Info);
		}

		public Vector3 GetOriginHandsPos(int HandID)
		{
			HandInfo info = new HandInfo();
			
			Vector3 rt = Vector3.zero;

			if (OrbbecNativeMethods.GetHandInfo(HandID, ref info))
			{
				Point3D.Point3ToVec3(ref rt, ref info.OriginHandsPos);
			}

			return rt;
		}

		public Vector3 GetCurHandsPos(int HandID)
		{
			HandInfo info = new HandInfo();

			Vector3 rt = Vector3.zero;

			if (OrbbecNativeMethods.GetHandInfo(HandID, ref info))
			{
				Point3D.Point3ToVec3(ref rt, ref info.CurHandsPos);
			}

			return rt;
		}

		public Vector3 GetHandsScreenPercent(int HandID)
		{
			HandInfo info = new HandInfo();

			Vector3 rt = Vector3.zero;

			if (OrbbecNativeMethods.GetHandInfo(HandID, ref info))
			{
				Point3D.Point3ToVec3(ref rt, ref info.ScreenPercent);
			}

			return rt;
		}

		public bool IsTrackingHand(int HandID)
		{
			return OrbbecNativeMethods.IsTrackingHand(HandID);
		}

		public void UpdateHandsData()
		{
			// 			// In OpenNI 1.5.x can only tracking one hand.
			// 			if (!HandIDTable.Contains(ms_CurHandsID) && ms_CurHandsID != 0)
			// 			{
			// 				HandIDTable.Clear();
			// 				HandIDTable.Add(ms_CurHandsID);
			// 			}

			OrbbecNativeMethods.ProcessHandsData();

			if (OnHandsUpdate != null)
				OnHandsUpdate();
		}

		public void InitHandData()
		{
			if (!m_Param.IsUseHandsTracker)
				return;

			OrbbecNativeMethods.SettingHandCallBack(OnHandCreate, OnHandLost);
		}
		#endregion

		private IEnumerator CallPluginAtEndOfFrames()
		{
			while (true)
			{
				// Wait until all frame rendering is done
				yield return new WaitForEndOfFrame();

				// Issue a plugin event with arbitrary integer identifier.
				// The plugin can distinguish between different
				// things it needs to do based on this ID.
				// For our simple plugin, it does not matter which ID we pass here.

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
	#if UNITY_5_0 ||  UNITY_5_1 ||  UNITY_5_2 ||  UNITY_5_3 ||  UNITY_5_4
				GL.IssuePluginEvent(OrbbecNativeMethods.GetRenderEventFunc(), 1);
	#else
				GL.IssuePluginEvent(1);
	#endif
#elif UNITY_IPHONE || UNITY_ANDROID
				OrbbecNativeMethods.UnityRenderEvent(1);
#endif
			}
		}

		public void ShutDown()
		{
			if (HasOrbbecDevice())
				OrbbecNativeMethods.CloseOrbbecDevice();
		}

		/// <summary>
		/// This Function is unsafe function. just for reserver function
		/// It may been removed in future.
		/// </summary>
		/// <param name="Type"></param>
		public static void SetUVCType(int Type)
		{
#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			//AndroidJavaObject activity = null;
			
			if (activity == null)
			{
				g_jc = new AndroidJavaClass("com.orbbec.unityadapt.UnityAdaptActivity");
				activity = g_jc.GetStatic<AndroidJavaObject>("m_Instance");
				Log.Print( Log.Level.Log, "Get the activity!");
			}

			// OpenNI can't run normally in some Android machine when you call release OpenNI and
			// Application.Quit() at the same time or later will make the app hold then it will 
			// call ANR error when you start this app next time. It have better call the exit function
			// in JAVA to close the app process.
			System.Object[] args = new System.Object[1];
			args[0] = Type;
			activity.Call("SettingUVCType", args);
			Log.Print(Log.Level.Log, string.Format("SettingUVCType:{0}",Type));
#endif
			OrbbecNativeMethods.SetUVCOpenMode(Type);
		}

		public static void SetShowLog(bool Flag)
		{
			OrbbecNativeMethods.SetShowLog(Flag);
		}

		public static void DoExit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif

#if ANDROID_OPENNI_NATIVE && !UNITY_EDITOR
			if (activity == null)
			{
				Application.Quit();
				return;
			}

			// OpenNI can't run normally in some Android machine when you call release OpenNI and
			// Application.Quit() at the same time or later will make the app hold then it will 
			// call ANR error when you start this app next time. It have better call the exit function
			// in JAVA to close the app process.
			System.Object[] args = new System.Object[0];
			activity.CallStatic("ExitSystem", args);
			Log.Print(Log.Level.Log, "DoExit");
#else
			Application.Quit();
#endif
		}
	}
}
#endif