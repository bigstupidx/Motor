using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Orbbec;

namespace OrbbecGestures
{
	[StructLayout(LayoutKind.Sequential)]
	struct NativeGestureConfigParam
	{
		public int IsUsingSubThread;
		
		// This is a value in eReturnInfoType
		public int ReturnInfoType;

		// How many player will be use for gesture recognizing system.
		public int PlayerNum;
		
		// The gesture base on skeleton number.
		public int SkeletonNum;

		// The skeleton type which will be update as this array's order.
		public IntPtr SkeletonTypeArray;

		// Skeleton Velocity calculation by the frame of position.
		// This will decision how many frame data of position in this calculation.
		public int SkeletonVelocityFrames;

		// Skeleton Velocity Multiple Type
		// To decide the multiple data of calculate the current velocity.
		public int SkeletonVelocityMulType;

		public int IsGetVelocityData;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct GestureInfo_State
	{
		// IsRecognized, Recognized: 1 | Unrecognized: 0
		public int IsRecognized;

		public uint GestureIndex;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct GestureInfo_Seg_State
	{
		// IsRecognized, Recognized: 1 | Unrecognized: 0
		public int IsRecognized;

		public uint GestureIndex;

		public uint SegmentIndex;
	
		public uint IsSegmentSucceed;
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct GestureInfo_Seg_Time_State
	{
		// IsRecognized, Recognized: 1 | Unrecognized: 0
		public int IsRecognized;

		public uint GestureIndex;

		public int SegmentIndex;

		public uint IsSegmentSucceed;

		public uint GestureTime;

		public uint SegmentTime;
	};

	static class GestureNativeMethods
	{
		#region Native Function
#if UNITY_EDITOR_64
		private const string dllName = "GestureNative64";
#else
		private const string dllName = "GestureNative";
#endif
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetDebugInfo(bool bFlag);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitGestureNative(NativeGestureConfigParam Param);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	CloseGestureNative();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int SubThreadFPS();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool	LoadConfigFile(string XmlFilePath);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool	LoadConfigData(string XmlFileData);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool	StartGestureWork();

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void OnIdle();

		//-------------------------Gesture Function Start------------------------------------------------

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint GetGestureIndex(int PlayerIndex, string GestureName);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr GetGestureName(int PlayerIndex, uint GestureIndex);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  int GetGestureNum( int PlayerIndex);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	SetPlayerGesture(int PlayerIndex, uint GestureIndex, bool IsUpdate);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	SetPlayerGestureFromName(int PlayerIndex, string GestureName, bool IsUpdate);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	ResetPlayerGesture(int PlayerIndex, uint GestureIndex);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void ResetPlayerGestureFromName(int PlayerIndex, string GestureName);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	SetPlayer(int PlayerIndex, bool IsUpdate);
	
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetPlayerGestureImmediate(int PlayerIndex, uint GestureIndex, bool IsUpdate);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	SetPlayerGestureImmediateFromName(int PlayerIndex, string GestureName, bool IsUpdate);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	ResetPlayerGestureImmediate(int PlayerIndex, uint GestureIndex);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	ResetPlayerGestureImmediateFromName(int PlayerIndex, string GestureName);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void	SetPlayerImmediate(int PlayerIndex, bool IsUpdate);

		//-------------------------Gesture Function End-------------------------------------------------

		//-------------------------Player Function Start------------------------------------------------
		// When using sub thread to update gesture using this function to update skeleton data.
		// 当使用子线程更新姿势时调用此函数更新骨骼数据
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  void UpdateSketonData(int PlayerIndex, IntPtr pData, int DataSize);

		// When using main thread to update gesture using this function to update skeleton data.
		// 当使用主线程更新姿势时调用此函数更新骨骼数据
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool UpdateSketonDataImmediate(int PlayerIndex, IntPtr pData, int DataSize, IntPtr pGestureInfo, ref int InfoNum);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern  bool GetPlayerGestureState(int PlayerIndex, IntPtr pGestureInfo, ref int InfoNum);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool GetPlayerVelocityData(int PlayerIndex, IntPtr pVelocityDataArray, int SkeletonNum);
		//-------------------------Player Function End--------------------------------------------------
		#endregion
	}

	public class GestureWrapper
	{
		NativeGestureConfigParam m_NativeParam;
		int[] SkeletonIndexs = null;
		
		public void SetDebugInfo(bool bFlag)
		{
			GestureNativeMethods.SetDebugInfo(bFlag);
		}

		public int GetFPS()
		{
			if (m_NativeParam.IsUsingSubThread != 0)
				return GestureNativeMethods.SubThreadFPS();
			else
				return (int)(1.0f / Time.unscaledDeltaTime);
		}

		public void Init(GestureConfigParams Param)
		{
			m_NativeParam = new NativeGestureConfigParam();

			m_NativeParam.IsUsingSubThread = Convert.ToInt32(Param.IsUsingSubThread);

			m_NativeParam.ReturnInfoType = (int)Param.GestureReturnInfoType;

			m_NativeParam.PlayerNum = Param.PlayerNum;

			m_NativeParam.SkeletonVelocityFrames = Param.SkeletonVelocityFrames;

			m_NativeParam.SkeletonVelocityMulType = (int)Param.SkeletonVelocityMulType;

			m_NativeParam.SkeletonNum = OrbbecWrapper.availableJoints.Length;

			SkeletonIndexs = new int[m_NativeParam.SkeletonNum];
			for (int i = 0; i < SkeletonIndexs.Length; ++i )
			{
				SkeletonIndexs[i] = (int)OrbbecWrapper.availableJoints[i];
			}

			m_NativeParam.SkeletonTypeArray = Marshal.UnsafeAddrOfPinnedArrayElement( SkeletonIndexs, 0);

			m_NativeParam.IsGetVelocityData = Convert.ToInt32(Param.IsGetVelocityData);

			GestureNativeMethods.InitGestureNative(m_NativeParam);
		}

		public bool LoadConfigFile(string XmlFilePath)
		{
			return GestureNativeMethods.LoadConfigFile(XmlFilePath);
		}

		public bool LoadConfigData(string XmlFileData)
		{
			return GestureNativeMethods.LoadConfigData(XmlFileData);
		}

		public bool StartGestureWork()
		{
			return GestureNativeMethods.StartGestureWork();
		}

		public uint GetGestureIndex(int PlayerIndex, string GestureName)
		{
			return GestureNativeMethods.GetGestureIndex( PlayerIndex, GestureName);
		}

		public string GetGestureName(int PlayerIndex, uint GestureIndex)
		{
			IntPtr namePtr = GestureNativeMethods.GetGestureName( PlayerIndex, GestureIndex);

			string Name = Marshal.PtrToStringAnsi(namePtr);
			return Name;
		}

		public int GetGestureNum(int PlayerIndex)
		{
			return GestureNativeMethods.GetGestureNum(PlayerIndex);
		}

		public void SetPlayerGesture(int PlayerIndex, uint GestureIndex, bool IsUpdate)
		{
			if (m_NativeParam.IsUsingSubThread == 0)
			{
				GestureNativeMethods.SetPlayerGestureImmediate(PlayerIndex, GestureIndex, IsUpdate);
			}
			else
			{
				GestureNativeMethods.SetPlayerGesture(PlayerIndex, GestureIndex, IsUpdate);
			}

		}

		public void SetPlayerGestureFromName(int PlayerIndex, string GestureName, bool IsUpdate)
		{
			if (m_NativeParam.IsUsingSubThread == 0)
			{
				GestureNativeMethods.SetPlayerGestureFromName(PlayerIndex, GestureName, IsUpdate);
			}
			else
			{
				GestureNativeMethods.SetPlayerGestureImmediateFromName(PlayerIndex, GestureName, IsUpdate);
			}
		}

		public void ResetPlayerGesture(int PlayerIndex, uint GestureIndex)
		{
			if (m_NativeParam.IsUsingSubThread == 0)
			{
				GestureNativeMethods.ResetPlayerGestureImmediate(PlayerIndex, GestureIndex);
			}
			else
			{
				GestureNativeMethods.ResetPlayerGesture(PlayerIndex, GestureIndex);
			}
		}

		public void ResetPlayerGestureFromName(int PlayerIndex, string GestureName)
		{
			if (m_NativeParam.IsUsingSubThread == 0)
			{
				GestureNativeMethods.ResetPlayerGestureImmediateFromName(PlayerIndex, GestureName);
			}
			else
			{
				GestureNativeMethods.ResetPlayerGestureFromName(PlayerIndex, GestureName);
			}
		}

		public void SetPlayer(int PlayerIndex, bool IsUpdate)
		{
			if (m_NativeParam.IsUsingSubThread == 0)
			{
				GestureNativeMethods.SetPlayerImmediate(PlayerIndex, IsUpdate);
			}
			else
			{
				GestureNativeMethods.SetPlayer(PlayerIndex, IsUpdate);
			}
		}

		public void OnIdle()
		{
			GestureNativeMethods.OnIdle();
		}

		public void UpdateSkeleton(int PlayerIndex, Point3D[] SkeletonData)
		{
			IntPtr SkeletonPtr = Marshal.UnsafeAddrOfPinnedArrayElement(SkeletonData, 0);

			GestureNativeMethods.UpdateSketonData(PlayerIndex, SkeletonPtr, SkeletonData.Length);
		}

		public bool UpdateSkeletonImmediate(int PlayerIndex, Point3D[] SkeletonData, IntPtr pGestureInfo, ref int InfoNum)
		{
			IntPtr SkeletonPtr = Marshal.UnsafeAddrOfPinnedArrayElement(SkeletonData, 0);
			return GestureNativeMethods.UpdateSketonDataImmediate(PlayerIndex, SkeletonPtr, SkeletonData.Length, pGestureInfo, ref InfoNum);
		}

		public bool GetPlayerGestureState(int PlayerIndex, IntPtr pGestureInfo, ref int InfoNum)
		{
			return GestureNativeMethods.GetPlayerGestureState( PlayerIndex, pGestureInfo, ref InfoNum);
		}

		public bool GetPlayerVelocityData(int PlayerIndex, Point3D[] SkeletonVelocityInfo)
		{
			IntPtr SkeletonPtr = Marshal.UnsafeAddrOfPinnedArrayElement(SkeletonVelocityInfo, 0);
			return GestureNativeMethods.GetPlayerVelocityData(PlayerIndex, SkeletonPtr, SkeletonVelocityInfo.Length);
		}

		public void ShutDown()
		{
			GestureNativeMethods.CloseGestureNative();
		}

	}
}