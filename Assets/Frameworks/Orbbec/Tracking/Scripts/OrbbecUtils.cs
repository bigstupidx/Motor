using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Orbbec
{
	public static class OrbbecUtils 
	{
		//OpenNI数据是反的，再反一次还原回来
		public static SkeletonType GetFlipSkeletonType(SkeletonType skeletonType)
		{
			SkeletonType skeletonType2 = skeletonType;
			switch (skeletonType)
			{
				case SkeletonType.LeftCollar:
					skeletonType2 = SkeletonType.RightCollar;
					break;
				case SkeletonType.LeftShoulder:
					skeletonType2 = SkeletonType.RightShoulder;
					break;
				case SkeletonType.LeftElbow:
					skeletonType2 = SkeletonType.RightElbow;
					break;
				case SkeletonType.LeftWrist:
					skeletonType2 = SkeletonType.RightWrist;
					break;
				case SkeletonType.LeftHand:
					skeletonType2 = SkeletonType.RightHand;
					break;
				case SkeletonType.LeftFingertip:
					skeletonType2 = SkeletonType.RightFingertip;
					break;

				case SkeletonType.RightCollar:
					skeletonType2 = SkeletonType.LeftCollar;
					break;
				case SkeletonType.RightShoulder:
					skeletonType2 = SkeletonType.LeftShoulder;
					break;
				case SkeletonType.RightElbow:
					skeletonType2 = SkeletonType.LeftElbow;
					break;
				case SkeletonType.RightWrist:
					skeletonType2 = SkeletonType.LeftWrist;
					break;
				case SkeletonType.RightHand:
					skeletonType2 = SkeletonType.LeftHand;
					break;
				case SkeletonType.RightFingertip:
					skeletonType2 = SkeletonType.LeftFingertip;
					break;

				case SkeletonType.LeftHip:
					skeletonType2 = SkeletonType.RightHip;
					break;
				case SkeletonType.LeftKnee:
					skeletonType2 = SkeletonType.RightKnee;
					break;
				case SkeletonType.LeftAnkle:
					skeletonType2 = SkeletonType.RightAnkle;
					break;
				case SkeletonType.LeftFoot:
					skeletonType2 = SkeletonType.RightFoot;
					break;

				case SkeletonType.RightHip:
					skeletonType2 = SkeletonType.LeftHip;
					break;
				case SkeletonType.RightKnee:
					skeletonType2 = SkeletonType.LeftKnee;
					break;
				case SkeletonType.RightAnkle:
					skeletonType2 = SkeletonType.LeftAnkle;
					break;
				case SkeletonType.RightFoot:
					skeletonType2 = SkeletonType.LeftFoot;
					break;

				default:
					skeletonType2 = skeletonType;
					break;

			}
			return skeletonType2;
		}

		private static Vector3 _depthSize = Vector3.zero;
		public static Vector3 depthSize
		{
			get
			{
				if (_depthSize.Equals (Vector3.zero))
				{
					int x, y, z;
					OrbbecManager.Instance.GetDepthSize (out x, out y, out z);
					_depthSize = new Vector3 (x, y, z);
				}
				return _depthSize;
			}
		}

		private static Dictionary<SkeletonType, int> _jointToIntDict = null;
		public static Dictionary<SkeletonType, int> jointToIntDict
		{
			get
			{
				if (_jointToIntDict == null || _jointToIntDict.Count <= 0)
				{
					_jointToIntDict = OrbbecManager.Instance.GetJointToIntDict();
				}
				return _jointToIntDict;
			}
		}

		public static bool IsUserActive(OrbbecUser user)
		{
			return (user != null) && (user.UserID != 0) && OrbbecManager.Instance.IsTrackingUser (user.UserID);
		}

		//得到镜像 转 正常 的骨架信息 世界坐标
		public static Vector3 GetWorldPos(OrbbecUser user, SkeletonType skeletonType)
		{
			if (IsUserActive(user))
			{
				SkeletonType skeletonType2 = OrbbecUtils.GetFlipSkeletonType (skeletonType);
				Vector3 vec = user.BoneWorldPos[jointToIntDict[skeletonType2]];
				return vec;
			}
			return Vector3.zero;
		}

		//得到镜像 转 正常 的骨架的 屏幕百分比数据
		public static Vector3 GetScreenPercentPos(OrbbecUser user, SkeletonType skeletonType)
		{
			if (IsUserActive(user))
			{
				SkeletonType skeletonType2 = OrbbecUtils.GetFlipSkeletonType (skeletonType);
				Vector3 vec = user.BoneScreenPos[jointToIntDict[skeletonType2]];
				vec = new Vector3 (vec.x / depthSize.x, (depthSize.y - vec.y) / depthSize.y, 1f);
				return vec;
			}
			return Vector3.zero;
		}

	}

}