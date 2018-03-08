using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Orbbec
{
	public class OrbbecPlayer 
	{
		public OrbbecUser bindUser;

		//标定成功后，记录的有效原始骨架信息
		private Dictionary<SkeletonType,Vector3> _trackingBoneWorldPosDict;
		private Dictionary<SkeletonType,Vector3> _trackingBoneScreenPercentPosDict;

		public Dictionary<SkeletonType, Vector3> trackingBoneWorldPosDict
		{
			get
			{
				return _trackingBoneWorldPosDict;
			}
			set
			{
				_trackingBoneWorldPosDict = value;
			}
		}

		public Dictionary<SkeletonType, Vector3> trackingBoneScreenPercentPosDict
		{
			get
			{
				return _trackingBoneScreenPercentPosDict;
			}
			set
			{
				_trackingBoneScreenPercentPosDict = value;
			}
		}

		//用户id
		public int UserId
		{
			get
			{
				if (bindUser != null)
				{
					return bindUser.UserID;
				}
				return 0;
			}
		}

		//是否有效
		public bool isActive
		{
			get
			{
				return OrbbecUtils.IsUserActive (bindUser);
			}
		}

		//得到标定时 初始的 镜像 转 正常 的骨架信息 世界坐标
		public Vector3 GetTrackingWorldPos(SkeletonType skeletonType)
		{
			if (_trackingBoneWorldPosDict != null && _trackingBoneWorldPosDict.Count > 0)
			{
				return _trackingBoneWorldPosDict [skeletonType];
			}
			return Vector3.zero;
		}

		//得到标定时 初始的 镜像 转 正常 的骨架信息 屏幕百分比坐标
		public Vector3 GetTrackingScreenPercentPos(SkeletonType skeletonType)
		{
			if (_trackingBoneScreenPercentPosDict != null && _trackingBoneScreenPercentPosDict.Count > 0)
			{
				return _trackingBoneScreenPercentPosDict [skeletonType];
			}
			return Vector3.zero;
		}

		//得到当前的 镜像 转 正常 的骨架信息 世界坐标
		public Vector3 GetWorldPos(SkeletonType skeletonType)
		{
			return OrbbecUtils.GetWorldPos (bindUser, skeletonType);
		}

		//得到当前的 镜像 转 正常 的骨架信息 屏幕百分比坐标
		public Vector3 GetScreenPercentPos(SkeletonType skeletonType)
		{
			return OrbbecUtils.GetScreenPercentPos (bindUser, skeletonType);
		}


	}
}
