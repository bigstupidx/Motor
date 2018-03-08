/*
 *		OrbbecUser.cs
 *	
 *		Use for the Skeleton data.
 * 
 *		Create By Sword
 *		2015_05_18				
 */
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Orbbec
{
	[System.Serializable]
	public class OrbbecUser
	{
		SkeletonType[] refAvailableJoints = null;

		public Vector3[]	BoneWorldPos	= null;
		public Vector3[]	BoneScreenPos	= null;
		public Quaternion[] BoneRotator		= null;
        
        public Vector3 COMWorldPos = Vector3.zero;
        public Vector3 COMSreenPos = Vector3.zero;

		public Point3D[]		BoneWorldP3Pos = null;
		public Point3D[]		BoneScreenP3Pos = null;
		public XnQuaternion[]	BoneXnQuat = null;

		IntPtr worldPosPtr	= IntPtr.Zero;
		IntPtr screenPosPtr = IntPtr.Zero;
		IntPtr rotatorPtr	= IntPtr.Zero;
		

		public int UserID = 0;

		bool m_IsInConfidence = true;

		public OrbbecUser(int userID)
		{
			UserID = userID;

			refAvailableJoints	= OrbbecManager.Instance.GetAvailableJointArray();
			BoneWorldPos		= new Vector3[refAvailableJoints.Length];
			BoneScreenPos		= new Vector3[refAvailableJoints.Length];

			BoneRotator			= new Quaternion[refAvailableJoints.Length];

			BoneWorldP3Pos	= new Point3D[refAvailableJoints.Length];
			BoneScreenP3Pos = new Point3D[refAvailableJoints.Length];
			BoneXnQuat		= new XnQuaternion[refAvailableJoints.Length];

			worldPosPtr		= Marshal.UnsafeAddrOfPinnedArrayElement(BoneWorldP3Pos, 0);
			screenPosPtr	= Marshal.UnsafeAddrOfPinnedArrayElement(BoneScreenP3Pos, 0);
			rotatorPtr		= Marshal.UnsafeAddrOfPinnedArrayElement(BoneXnQuat, 0);
		}					  

		public bool IsInConfidence()
		{
			return m_IsInConfidence && UserID != 0;
		}

		public void Update()
		{
			OrbbecManager refOrbbecManager = OrbbecManager.Instance;

			m_IsInConfidence = OrbbecNativeMethods.UpdateUserInfo(UserID, worldPosPtr, screenPosPtr, rotatorPtr);

			for (int i = 0; i < refAvailableJoints.Length; ++i )
			{
				Point3D.Point3ToVec3( ref BoneWorldPos[i],	ref BoneWorldP3Pos[i]);
				Point3D.Point3ToVec3( ref BoneScreenPos[i],	ref BoneScreenP3Pos[i]);
				XnQuaternion.XnQuatToQuat( ref BoneRotator[i],	ref BoneXnQuat[i]);
			}

            COMWorldPos = refOrbbecManager.GetUserCOM(UserID);
            COMSreenPos = refOrbbecManager.ConvertRealWorldToProjective(COMWorldPos);

		}
	}
}


