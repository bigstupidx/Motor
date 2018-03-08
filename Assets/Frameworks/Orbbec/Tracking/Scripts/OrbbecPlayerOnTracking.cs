using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Orbbec
{
	public class OrbbecPlayerOnTracking 
	{
		public OrbbecUser bindUser;

		//标定时间
		public float trackingTime;

		//丢失举手次数
		public int trackingMissCnt;

		//标定状态
		public enum TrackingState
		{
			none,
			tracking,
			tracked,
		}
		public TrackingState trackingState = TrackingState.none;

		//标定过程中原始骨架信息列表
		private List<Dictionary<SkeletonType,Vector3>> _trackingBoneWorldPosDictList = new List<Dictionary<SkeletonType,Vector3>>();
		private List<Dictionary<SkeletonType,Vector3>> _trackingBoneScreenPercentPosDictList = new List<Dictionary<SkeletonType,Vector3>>();

		//上一帧胸点，用于判定双人标定时在左还是在右
		public Vector3 torsoPos = Vector3.zero;

		//标定成功后，记录的有效原始骨架信息
		private Dictionary<SkeletonType,Vector3> _trackingBoneWorldPosDict = new Dictionary<SkeletonType, Vector3> ();
		private Dictionary<SkeletonType,Vector3> _trackingBoneScreenPercentPosDict = new Dictionary<SkeletonType, Vector3> ();

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

		public bool isActive
		{
			get
			{
				return OrbbecUtils.IsUserActive (bindUser);
			}
		}

		//开始标定
		public void StartTracking()
		{
			trackingState = TrackingState.tracking;
			trackingTime = 0f;
			trackingMissCnt = 0;
			_trackingBoneWorldPosDictList.Clear ();
			_trackingBoneScreenPercentPosDictList.Clear ();
			_trackingBoneWorldPosDict.Clear ();
			_trackingBoneScreenPercentPosDict.Clear ();
		}

		//取消标定
		public void CancelTracking()
		{
			trackingState = TrackingState.none;
			trackingTime = 0f;
			trackingMissCnt = 0;
			_trackingBoneWorldPosDictList.Clear ();
			_trackingBoneScreenPercentPosDictList.Clear ();
			_trackingBoneWorldPosDict.Clear ();
			_trackingBoneScreenPercentPosDict.Clear ();

			bindUser = null;
		}

		//每帧记录标定
		public void UpdateTracking(float dt)
		{
			trackingTime += dt;
			trackingMissCnt = 0;
			Dictionary<SkeletonType,Vector3> boneWorldPosDict = new Dictionary<SkeletonType, Vector3> ();
			Dictionary<SkeletonType,Vector3> boneScreenPercentPosDict = new Dictionary<SkeletonType, Vector3> ();
			foreach (KeyValuePair<SkeletonType, int> kv in OrbbecUtils.jointToIntDict)
			{
				if (kv.Key != SkeletonType.Invalid)
				{
					boneWorldPosDict.Add (kv.Key, OrbbecUtils.GetWorldPos (bindUser, kv.Key));
					boneScreenPercentPosDict.Add (kv.Key, OrbbecUtils.GetScreenPercentPos (bindUser, kv.Key));
				}
			}
			_trackingBoneWorldPosDictList.Add (boneWorldPosDict);
			_trackingBoneScreenPercentPosDictList.Add (boneScreenPercentPosDict);

			if (boneWorldPosDict != null && boneWorldPosDict.ContainsKey(SkeletonType.Torso))
			{
				torsoPos = boneWorldPosDict [SkeletonType.Torso];
			}
		}


		public void FinishTracking()
		{
			trackingState = TrackingState.tracked;

			//标定成功,将初始骨架信息记录保存,剔除过大过小值，取中间20%数据的平均值
			List<Vector3> worldPosList = new List<Vector3>();
			Vector3 worldPos;
			_trackingBoneWorldPosDict.Clear ();
			_trackingBoneScreenPercentPosDict.Clear ();

			foreach (KeyValuePair<SkeletonType, int> kv in OrbbecUtils.jointToIntDict)
			{
				if (kv.Key != SkeletonType.Invalid)
				{
					//取所有这个骨骼点数据，计算中间值记录
					worldPosList.Clear();
					foreach (Dictionary<SkeletonType,Vector3> dict in _trackingBoneWorldPosDictList)
					{
						worldPosList.Add (dict [kv.Key]);
					}
					worldPos = GetAveragePoint (worldPosList);
					_trackingBoneWorldPosDict.Add (kv.Key, worldPos);

					//取所有这个骨骼点数据，计算中间值记录
					worldPosList.Clear();
					foreach (Dictionary<SkeletonType,Vector3> dict in _trackingBoneScreenPercentPosDictList)
					{
						worldPosList.Add (dict [kv.Key]);
					}
					worldPos = GetAveragePoint (worldPosList);
					_trackingBoneScreenPercentPosDict.Add (kv.Key, worldPos);

				}
			}

			if (_trackingBoneWorldPosDict != null && _trackingBoneWorldPosDict.Count > 0)
			{
				torsoPos = _trackingBoneWorldPosDict [SkeletonType.Torso];
			}
		}

		//舍弃过大过小值，中间30%数据取平均值
		Vector3 GetAveragePoint(List<Vector3> list)
		{
			float qz = 0.3f;

			List<float> xList = new List<float> ();
			List<float> yList = new List<float> ();
			List<float> zList = new List<float> ();
			foreach (Vector3 vec3 in list)
			{
				xList.Add (vec3.x);
				yList.Add (vec3.y);
				zList.Add (vec3.z);
			}

			xList.Sort ();
			yList.Sort ();
			zList.Sort ();

			int useIdx = Mathf.Max (0, Mathf.FloorToInt (list.Count * (1 - qz) / 2));
			useIdx = Mathf.Clamp (useIdx, 0, list.Count - 1);
			int useCnt = Mathf.FloorToInt (list.Count * qz);
			useCnt = Mathf.Clamp (useCnt, 1, list.Count - useIdx);

			float rx = 0f, ry = 0f, rz = 0f;
			int i = 0 , cnt = 0;
			for (i = useIdx; i < useIdx + useCnt; i++)
			{
				rx += xList [i];
				ry += yList [i];
				rz += zList [i];
				cnt++;
			}

			rx = rx / (float)cnt;
			ry = ry / (float)cnt;
			rz = rz / (float)cnt;
			return new Vector3 (rx, ry, rz);
		}

		public OrbbecPlayer ToOrbbecPlayer()
		{
			OrbbecPlayer ret = new OrbbecPlayer ();
			ret.bindUser = bindUser;
			ret.trackingBoneWorldPosDict = _trackingBoneWorldPosDict;
			ret.trackingBoneScreenPercentPosDict = _trackingBoneScreenPercentPosDict;
			return ret;
		}

		public static OrbbecPlayerOnTracking FromOrbbecPlayer(OrbbecPlayer oplayer)
		{
			OrbbecPlayerOnTracking tplayer = new OrbbecPlayerOnTracking ();
			tplayer.bindUser = oplayer.bindUser;
			tplayer._trackingBoneWorldPosDict = oplayer.trackingBoneWorldPosDict;
			tplayer._trackingBoneScreenPercentPosDict = oplayer.trackingBoneScreenPercentPosDict;
			tplayer.torsoPos = oplayer.GetTrackingWorldPos (SkeletonType.Torso);
			tplayer.trackingState = TrackingState.tracked;
			return tplayer;
		}
	}

}