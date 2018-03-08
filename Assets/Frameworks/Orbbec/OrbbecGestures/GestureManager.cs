using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Orbbec;

namespace OrbbecGestures
{
	public enum ReturnInfoType
	{
		// When has gesture been recognized, return the list include.
		RIT_STATE_ONLY = 0,				// only send the state back. See GestureInfo_State
		RIT_SEGMENTID_STATE = 1,		// send the state and segment id back. See GestureInfo_Seg_State
		RIT_SEGMENTID_TIME_STATE = 2,	// send the gesture state, segment state, gesture time, segment time back. See GestureInfo_Seg_Time_State
	};

	public enum VelocityMulType
	{
		VMT_AVERAGE	= 0, // average				 求均值
		VMT_GAUSS = 1,	 // Gaussian coefficient 高斯系数 	
	};

	public class GestureConfigParams
	{
		/// <summary>
		/// Is Using subthread to update gesture logical.
		/// 是否使用子线程更新姿势逻辑
		/// </summary>
		public bool IsUsingSubThread = true;

		/// <summary>
		/// This is a value in eReturnInfoType, to setting the gesture back info type.
		/// 姿势返回信息类型
		/// </summary>
		public ReturnInfoType GestureReturnInfoType = ReturnInfoType.RIT_STATE_ONLY;

		/// <summary>
		/// How many player will be use for gesture recognizing system.
		/// 姿势用户数 
		/// </summary>
		public int PlayerNum = 1;

		/// <summary>
		/// Skeleton Velocity calculation by the frame of position.
		/// This will decision how many frame data of position in this calculation. 
		/// 骨骼速度计算帧，这决定了有多少帧的点数据用于速度计算
		/// </summary>
		public int SkeletonVelocityFrames = 1;

		/// <summary>
		/// Skeleton Velocity Multiple Type
		/// To decide the multiple data of calculate the current velocity.
		/// 骨架速度 系数类型
		/// 这决定了计算当前速度时 每帧的乘法系数的类型
		/// </summary>
		public VelocityMulType SkeletonVelocityMulType = VelocityMulType.VMT_AVERAGE;

		/// <summary>
		/// the call back function when you get the gesture info.
		/// 姿势识别回调函数
		/// </summary>
		public GestureManager.OnGetGestureInfoCallback_State GetStateCallback = null;

		/// <summary>
		/// To decide get the velocity from gesture system or not.if true velocity in BindPlayers.SkeletonVelocityInfo will work.
		/// 是否从姿势库底层中将其计算后的速度传递出来。当值为真时，BindPlayers.SkeletonVelocityInfo 将会生效
		/// </summary>
		public bool IsGetVelocityData = false;
	}


	public class GestureManager : MonoBehaviour
	{
		public static GestureManager Instance
		{
			get
			{
				return m_Instance;
			}
		}

		static GestureManager m_Instance							= null;

		GestureWrapper m_Wrapper									= null;

		public PlayerStateBase[] BindPlayers						= null;
		int m_BindPlayerNum											= 0;

		public GestureConfigParams Param							= null;

		public delegate void OnGetGestureInfoCallback_State(int PlayerIndex, PlayerStateBase playerState);

		public static GestureManager Create(GestureConfigParams Param)
		{
			if (OrbbecManager.Instance == null)
			{
				Log.Print(Log.Level.Error, "OrbbecManager is null, you should create GestureManager after OrbbecManager has been created.");
				return null;
			}

			if (!OrbbecManager.Instance.IsInited)
			{
				Log.Print(Log.Level.Error, "OrbbecManager still no be initialized, you should create GestureManager after OrbbecManager has been initialized.");
				return null;
			}

			GameObject obj = new GameObject("GestureManager");
			obj.AddComponent<GestureManager>();

			Instance.Init(Param);

			return Instance;
		}

		/// <summary>
		/// Setting is log the debug information or not
		/// 设置是否记录调试信息
		/// </summary>
		/// <param name="bFlag"></param>
		public void SetDebugInfo(bool bFlag)
		{
			m_Wrapper.SetDebugInfo(bFlag);
		}

		/// <summary>
		/// Get Gesture FPS. if use subthread, it will return subthread FPS, or it will return main FPS
		/// 获取姿势库FPS
		/// </summary>
		/// <returns></returns>
		public int GetFPS()
		{
			return m_Wrapper.GetFPS();
		}

		/// <summary>
		///  Give a path to read the xml config file.
		///  通过XML文件路径来加载配置
		/// </summary>
		/// <param name="XmlFilePath">xml config file path</param>
		/// <returns></returns>
		public bool LoadConfigFile(string XmlFilePath)
		{
			return m_Wrapper.LoadConfigFile(XmlFilePath);
		}

		/// <summary>
		///  Read the xml config from memory.
		///  加载XML数据格式配置
		/// </summary>
		/// <param name="XmlFileData">Xml Data in memory</param>
		/// <returns></returns>
		public bool LoadConfigData(string XmlFileData)
		{
			return m_Wrapper.LoadConfigData(XmlFileData);
		}

		/// <summary>
		/// Compiler and linking all config file and start gesture recognizing system.
		/// 统筹配置文件启动姿势识别
		/// </summary>
		/// <returns></returns>
		public bool StartGestureWork()
		{
			if (!m_Wrapper.StartGestureWork())
			{
				return false;
			}

			for (int i = 0; i < BindPlayers.Length; ++i )
			{
				BindPlayers[i].ResetGestureInfoSize(GetGestureNum(i));
			}

			

			return true;
		}

		public uint GetGestureIndex(int PlayerIndex, string GestureName)
		{
			return m_Wrapper.GetGestureIndex(PlayerIndex,GestureName);
		}

		public string GetGestureName(int PlayerIndex, uint GestureIndex)
		{
			return m_Wrapper.GetGestureName(PlayerIndex, GestureIndex);
		}

		public int GetGestureNum(int PlayerIndex)
		{
			return m_Wrapper.GetGestureNum(PlayerIndex);
		}

		public void SetPlayerGesture(int PlayerIndex, uint GestureIndex, bool IsUpdate)
		{
			m_Wrapper.SetPlayerGesture(PlayerIndex, GestureIndex, IsUpdate);
		}

		public void SetPlayerGestureFromName(int PlayerIndex, string GestureName, bool IsUpdate)
		{
			m_Wrapper.SetPlayerGestureFromName(PlayerIndex, GestureName, IsUpdate);
		}

		public void ResetPlayerGesture(int PlayerIndex, uint GestureIndex)
		{
			m_Wrapper.ResetPlayerGesture(PlayerIndex, GestureIndex);
		}

		public void ResetPlayerGestureFromName(int PlayerIndex, string GestureName)
		{
			m_Wrapper.ResetPlayerGestureFromName(PlayerIndex, GestureName);
		}

		public bool BindPlayerUserID(int UserID, int PlayerIndex, bool IsForceBind)
		{
			bool IsAdd = true;

			if (PlayerIndex >= BindPlayers.Length)
			{
				return false;
			}

			if (!IsForceBind 
				&&( BindPlayers[PlayerIndex].PlayerBindUser != null 
					&& BindPlayers[PlayerIndex].PlayerBindUser.IsInConfidence()))
			{
				return false;
			}

			OrbbecUser User = null;
			if (!OrbbecManager.Instance.TrackingUsers.TryGetValue( UserID, out User))
			{
				return false;
			}

			if (!User.IsInConfidence())
			{
				return false;
			}

			IsAdd = BindPlayers[PlayerIndex].PlayerBindUser == null
				|| !BindPlayers[PlayerIndex].PlayerBindUser.IsInConfidence();

			BindPlayers[PlayerIndex].PlayerBindUser = User;
			m_Wrapper.SetPlayer(PlayerIndex, true);
			BindPlayers[PlayerIndex].IsPlayerUpdating = true;

			if (IsAdd)
				++m_BindPlayerNum;

			return true;
		}

		void Awake()
		{
			if (m_Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			m_Instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}

		delegate void OnGetGestureInfoCallback(int PlayerIndex);

		OnGetGestureInfoCallback m_GetGestureInfoFunc;

		void Init(GestureConfigParams Param)
		{
			if (Param == null)
			{
				this.Param = new GestureConfigParams();
			}
			else
			{
				this.Param = Param;
			}

			BindPlayers = new PlayerStateBase[this.Param.PlayerNum];

			switch (this.Param.GestureReturnInfoType)
			{
				case ReturnInfoType.RIT_STATE_ONLY:
					for (int i = 0; i < Instance.BindPlayers.Length; ++i)
					{
						Instance.BindPlayers[i] = new PlayerState_State();
					}
					break;
				case ReturnInfoType.RIT_SEGMENTID_STATE:
					for (int i = 0; i < Instance.BindPlayers.Length; ++i)
					{
						Instance.BindPlayers[i] = new PlayerState_SegState();
					}
					break;
				case ReturnInfoType.RIT_SEGMENTID_TIME_STATE:
					for (int i = 0; i < Instance.BindPlayers.Length; ++i)
					{
						Instance.BindPlayers[i] = new PlayerState_SegTimeState();
					}
					break;
			}

			if (this.Param.IsGetVelocityData)
			{
				for (int i = 0; i < Instance.BindPlayers.Length; ++i)
				{
					Instance.BindPlayers[i].SkeletonVelocityInfo = new Point3D[OrbbecManager.Instance.GetAvailableJointArray().Length];
				}
			}

			if (this.Param.IsUsingSubThread)
			{
				m_GetGestureInfoFunc = OnGetGestureInfo;
			}
			else
			{
				m_GetGestureInfoFunc = UpdateSkeletonImmediate;
			}	

			m_Wrapper = new GestureWrapper();
			m_Wrapper.Init(this.Param);
		}
	
		void OnGetGestureInfo( int PlayerIndex)
		{
			UpdateSkeleton(PlayerIndex);
			m_Wrapper.OnIdle();
			int curNum = 0;
			
			if (!m_Wrapper.GetPlayerGestureState(PlayerIndex, BindPlayers[PlayerIndex].GetGestureInfoPtr(), ref curNum))
				return;

			BindPlayers[PlayerIndex].ResetGestureInfoSize(curNum);

			if (Param.GetStateCallback != null)
				Param.GetStateCallback(PlayerIndex, BindPlayers[PlayerIndex]);

			if (Param.IsGetVelocityData)
			{
				m_Wrapper.GetPlayerVelocityData(PlayerIndex, BindPlayers[PlayerIndex].SkeletonVelocityInfo);
			}
		}

		void Update()
		{
			int i = 0;
			for (; i < BindPlayers.Length; ++i)
			{
				if (!BindPlayers[i].IsPlayerUpdating)
				{
					continue;
				}

				if (BindPlayers[i].PlayerBindUser == null || !BindPlayers[i].PlayerBindUser.IsInConfidence())
				{
					BindPlayers[i].PlayerBindUser = null;
					BindPlayers[i].IsPlayerUpdating = false;
					--m_BindPlayerNum;

					m_Wrapper.SetPlayer(i, false);
					Log.Print(Log.Level.Log, string.Format("Player{0} stop update", i));
					continue;
				}

				m_GetGestureInfoFunc(i);
			}
		}

		bool IsPosDataOK(ref Point3D[] PosArray)
		{
			for (int i = 0; i < PosArray.Length; ++i )
			{
				if (PosArray[i].z <= 0.0f)
				{
					return false;
				}
			}

			return true;
		}

		void UpdateSkeleton(int PlayerIndex)
		{
			OrbbecUser user = BindPlayers[PlayerIndex].PlayerBindUser;
			if (user == null || user.UserID == 0 || !user.IsInConfidence())
				return;

			if (!IsPosDataOK(ref user.BoneWorldP3Pos))
				return;

		//	Log.Print(Log.Level.Log, "Do UpdateSkeleton");
		//	Log.Print(Log.Level.Log, string.Format("lhand {0},{1},{2}", user.BoneWorldP3Pos[5].x, user.BoneWorldP3Pos[5].y, user.BoneWorldP3Pos[5].z));
			m_Wrapper.UpdateSkeleton(PlayerIndex, user.BoneWorldP3Pos);
		}

		void UpdateSkeletonImmediate(int PlayerIndex)
		{
			int curNum = 0;

			OrbbecUser user = BindPlayers[PlayerIndex].PlayerBindUser;
			if (user == null || user.UserID == 0 || !user.IsInConfidence())
				return;

			m_Wrapper.UpdateSkeletonImmediate(PlayerIndex, 
				BindPlayers[PlayerIndex].PlayerBindUser.BoneWorldP3Pos,
				BindPlayers[PlayerIndex].GetGestureInfoPtr(), ref curNum);

			if (Param.GetStateCallback != null)
				Param.GetStateCallback(PlayerIndex, BindPlayers[PlayerIndex]);

			if (Param.IsGetVelocityData)
			{
				m_Wrapper.GetPlayerVelocityData(PlayerIndex, BindPlayers[PlayerIndex].SkeletonVelocityInfo);
			}
		}

		void OnDestroy()
		{
			if (m_Instance != this)
				return;

			m_Wrapper.ShutDown();
			m_Instance = null;
		}
	}
}
