using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Orbbec
{
	public class OrbbecSensingManager : MonoBehaviour
	{
		private static OrbbecSensingManager _instance;

		public static OrbbecSensingManager instance
		{
			get
			{
				return _instance;
			}
		}

		#region region 初始化

		public static void Init ()
		{
			if (_instance == null)		
			{
				UnityEngine.Object PrefabObj = Resources.Load ("Prefabs/OrbbecSensingManager");
				GameObject obj = UnityEngine.Object.Instantiate (PrefabObj) as GameObject;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = Vector3.zero;
				_instance = obj.GetComponent<OrbbecSensingManager> ();
			}
		}

		public static bool HasInstance ()
		{
			return _instance != null;
		}

		public static void Destroy ()
		{
			if (_instance != null)
			{
				Destroy (_instance.gameObject);
				_instance = null;
			}
		}

		public void InitOrbbecDevice ()
		{
			if (OrbbecManager.HasOrbbecDevice ())
			{
				Debug.Log ("Find orbbec device!");
			}
			else
			{
				Debug.LogError ("Can't find orbbec device.");
			}

			OrbbecManagerParam param = new OrbbecManagerParam ();
			param.IsUseDepth = false;
			param.IsUseSceneDepth = false;
			param.IsUseUserImage = false; //true
			param.IsUseUVC = false;
			param.IsUseBackRemoval = false;
			param.IsUseUserLabel = true;
			param.IsUseCatch = false;
			param.IsUseHandsTracker = false;
			param.IsNativeUpdateTexture = false;
			param.IsUseOffsetCorrect = false;
			param.IsTrackingSkeleton = true;
			param.IsExitOnBack = false;
			#if !UNITY_EDITOR
			param.IsCustomResolution = true;
			param.CurResolution.DepthXSize = 320;
			param.CurResolution.DepthYSize = 240;
			param.CurResolution.ImageXSize = 320;
			param.CurResolution.ImageYSize = 240;
			//param.CurResolution.ImageXSize = 640;
			//param.CurResolution.ImageYSize = 320;
			#endif

			param.OrbbecInitResourceCallBack += OrbbecInitResourceCallBack;

			OrbbecManager.CreateOrbbecManager (param);
		}

		void OrbbecInitResourceCallBack ()
		{
			Debug.Log ("OrbbecInitResourceCallBack");
			if (deviceInitAction != null)
			{
				deviceInitAction ();
			}
		}

		#endregion

		#region region 标定逻辑

		#region region 参数变量

		public bool IsDeviceInited
		{
			get
			{
				return OrbbecManager.Instance != null && OrbbecManager.Instance.IsInited;
			}
		}

		//玩家1骨架数据
		private OrbbecPlayer _player1;
		public OrbbecPlayer player1
		{
			get
			{
				return _player1;
			}
			set
			{
				_player1 = value;
			}
		}

		//玩家2骨架数据
		private OrbbecPlayer _player2;
		public OrbbecPlayer player2
		{
			get
			{
				return _player2;
			}
			set
			{
				_player2 = value;
			}
		}

		//是否标定有效
		public bool isActive
		{
			get
			{
				if (playerMode == PlayerMode.single)
				{
					return isPlayer1Active;
				}
				else if (playerMode == PlayerMode.two)
				{
					return isPlayer1Active && isPlayer2Active;
				}
				return false;
			}
		}

		//左边玩家是否有效
		public bool isPlayer1Active
		{
			get
			{
				return player1 != null && player1.isActive;
			}
		}

		//右边玩家是否有效
		public bool isPlayer2Active
		{
			get
			{
				return player2 != null && player2.isActive;
			}
		}

		//游戏模式，单人还是双人
		public enum PlayerMode 
		{
			single,
			two,
		}

		//游戏模式，单人还是双人
		private PlayerMode _playerMode = PlayerMode.single;
		public PlayerMode playerMode
		{
			get
			{
				return _playerMode;
			}
			set
			{
				if (_playerMode == value)
					return;

				_playerMode = value;

				if (_playerMode == PlayerMode.single)
				{
					player2 = null;
				}
			}
		}


		///正在标定中
		private bool _isTracking = false;
		public bool isTracking
		{
			get
			{
				return _isTracking;
			}
		}

		private bool _firstTracking = true;

		//获取深度图尺寸
		private Vector3 _depthSize = Vector3.zero;
		public Vector3 depthSize
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

		//标定信息
		OrbbecPlayerOnTracking tplayer1;
		OrbbecPlayerOnTracking tplayer2;
		OrbbecUser tmpUser;
		public float needTrackingTime = 1f;
		public int allowTrackingMissCnt = 3;

		Dictionary<int, OrbbecUser> _userTable = null;
		public Dictionary<int, OrbbecUser> userTable
		{
			get
			{
				if (_userTable == null)
				{
					_userTable = OrbbecManager.Instance.TrackingUsers;
				}
				return _userTable;
			}
		}

		public bool showTrackingUI = true;

		public OrbbecTrackingUI orbbecTrackingUI = null;

		//设备初始化完成回调
		[HideInInspector]
		public Action deviceInitAction = null;

		//标定完成回调
		[HideInInspector]
		public Action trackedAction = null;

		//丢失标定回调
		[HideInInspector]
		public Action unTrackedAction = null;

		//左出拳回调
		[HideInInspector]
		public Action leftAtkAction = null;

		//右出拳回调
		[HideInInspector]
		public Action rightAtkAction = null;
		//举左手回调
		[HideInInspector]
		public Action leftHandRaiseAction = null;
		//举右手回调
		[HideInInspector]
		public Action rightHandRaiseAction = null;
		#endregion


		//Update
		void Update () 
		{
			if (!IsDeviceInited)
				return;
			
			//每帧检测标定,无标定则要求标定
			if (isActive)
			{
				orbbecTrackingUI.gameObject.SetActive (false);
			}
			else
			{
				if (!_isTracking)
				{
					_isTracking = true;

					if (playerMode == PlayerMode.single)
					{
						//初始化user1
						tplayer1 = new OrbbecPlayerOnTracking ();
						tplayer2 = new OrbbecPlayerOnTracking ();
					}
					else
					{
						if (isPlayer1Active)
						{
							tplayer1 = OrbbecPlayerOnTracking.FromOrbbecPlayer (player1);
						}
						else
						{
							tplayer1 = new OrbbecPlayerOnTracking ();
						}

						if (isPlayer2Active)
						{
							tplayer2 = OrbbecPlayerOnTracking.FromOrbbecPlayer (player2);
						}
						else
						{
							tplayer2 = new OrbbecPlayerOnTracking ();
						}
					}

					//丢失标定 回调
					if (!_firstTracking && unTrackedAction != null)
					{
						unTrackedAction ();
					}

					_firstTracking = false;
				}

				if (playerMode == PlayerMode.single)
				{
					ProcessTrackingSingle ();
				}
				else
				{
					ProcessTrackingTwo ();
				}
			}

			//检测出拳
			MotoUpdate ();

		}

		//单人标定检测
		void ProcessTrackingSingle()
		{
			//标定成功
			if (tplayer1.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
			{
				player1 = tplayer1.ToOrbbecPlayer ();
				tplayer1 = null;
				tplayer2 = null;
				_isTracking = false;

				MotoOnTracked ();

				//标定成功 回调
				if (trackedAction != null)
				{
					trackedAction ();
				}
				return;
			}

			//标定玩家1
			TrackingPlayer (tplayer1);

			if (showTrackingUI)
			{
				orbbecTrackingUI.gameObject.SetActive (true);

				RefreshTrackingView ();
			}
			else
			{
				orbbecTrackingUI.gameObject.SetActive (false);
			}
		}

		//双人标定检测
		void ProcessTrackingTwo()
		{
			//标定成功
			if (tplayer1.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked
				&& tplayer2.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
			{
				//根据胸点，区分左右。
				OrbbecPlayerOnTracking lplayer;
				OrbbecPlayerOnTracking rplayer;

				if (tplayer1.torsoPos.x <= tplayer2.torsoPos.x)
				{
					lplayer = tplayer1;
					rplayer = tplayer2;
				}
				else
				{
					lplayer = tplayer2;
					rplayer = tplayer1;
				}

				player1 = lplayer.ToOrbbecPlayer ();
				player2 = rplayer.ToOrbbecPlayer ();
				tplayer1 = null;
				tplayer2 = null;
				lplayer = null;
				rplayer = null;
				_isTracking = false;

				MotoOnTracked ();

				//标定成功 回调
				if (trackedAction != null)
				{
					trackedAction ();
				}
				return;
			}

			//标定玩家1
			TrackingPlayer (tplayer1);

			//标定玩家2
			TrackingPlayer (tplayer2);

			if (showTrackingUI)
			{
				orbbecTrackingUI.gameObject.SetActive (true);

				RefreshTrackingView ();
			}
			else
			{
				orbbecTrackingUI.gameObject.SetActive (false);
			}
		}

		//检测标定
		void TrackingPlayer(OrbbecPlayerOnTracking tplayer)
		{
			//未开始标定，获取新user，启动标定
			if (tplayer.trackingState == OrbbecPlayerOnTracking.TrackingState.none)
			{
				tmpUser = GetNewUser ();
				if (tmpUser != null)
				{
					tplayer.bindUser = tmpUser;
					tplayer.StartTracking ();
				}
			}
			//正在标定，判定是否失去焦点，有焦点则记录信息，失去焦点或不举手则取消标定
			else if (tplayer.trackingState == OrbbecPlayerOnTracking.TrackingState.tracking)
			{
				bool isMissing = false;
				bool IsRisingHands = false;
				if (OrbbecUtils.IsUserActive (tplayer.bindUser))
				{
					if (OrbbecManager.Instance.IsRisingHands (tplayer.UserId))
					{
						IsRisingHands = true;
					}
					else
					{
						//允许中途连续丢失3帧举手
						tplayer.trackingMissCnt++;
						if (tplayer.trackingMissCnt >= allowTrackingMissCnt)
						{
							isMissing = true;
						}
					}
				}
				else
				{
					isMissing = true;
				}

				if (!isMissing)
				{
					if (IsRisingHands)
					{
						tplayer.UpdateTracking (Time.unscaledDeltaTime);//lth: 使用unscaledTime以适应游戏中暂停
						if (tplayer.trackingTime >= needTrackingTime)
						{
							tplayer.FinishTracking ();
						}
					}
				}
				else
				{
					tplayer.CancelTracking ();
				}
			}
			else if (tplayer.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
			{
				bool isMissing = false;
				if (!OrbbecUtils.IsUserActive (tplayer.bindUser))
				{
					isMissing = true;
				}

				if (isMissing)
				{
					tplayer.CancelTracking ();
				}
			}
		}

		//获取新用户
		OrbbecUser GetNewUser()
		{
			OrbbecUser ret = null;

			if (userTable != null && userTable.Count > 0)
			{
				foreach (KeyValuePair<int, OrbbecUser> kv in userTable)
				{
					tmpUser = kv.Value;

					if (tmpUser == tplayer1.bindUser || tmpUser == tplayer2.bindUser)
					{
						continue;
					}
					else
					{
						if (OrbbecManager.Instance.IsRisingHands (tmpUser.UserID))
						{
							ret = tmpUser;
							break;
						}
					}
				}
			}
			return ret;
		}


		//显示刷新标定视图
		void RefreshTrackingView()
		{
			orbbecTrackingUI.userLabelImg.texture = OrbbecManager.Instance.GetUserLabelMap ();

			if (playerMode == PlayerMode.single)
			{
				orbbecTrackingUI.isSingleMode = true;

				if (tplayer1.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
				{
					orbbecTrackingUI.player1Percent = 1f;
				}
				else
				{
					orbbecTrackingUI.player1Percent = (tplayer1.trackingTime / needTrackingTime);
				}
			}
			else
			{
				orbbecTrackingUI.isSingleMode = false;

				//双人标定，根据规则，判定左右位置
				int trackCnt = 0;
				if (tplayer1.trackingState != OrbbecPlayerOnTracking.TrackingState.none)
				{
					trackCnt++;
				}
				if (tplayer2.trackingState != OrbbecPlayerOnTracking.TrackingState.none)
				{
					trackCnt++;
				}	

				//无标定情况，都为0
				orbbecTrackingUI.player1Percent = 0f;
				orbbecTrackingUI.player2Percent = 0f;

				//只有一个人在标定 或者标定成功 根据那个人的胸点左右来判断， 是左边还是右边
				if (trackCnt == 1)
				{
					OrbbecPlayerOnTracking tplayer = tplayer1;
					bool isLeft = true;
					if (tplayer1.trackingState != OrbbecPlayerOnTracking.TrackingState.none)
					{
						tplayer = tplayer1;
					}
					else if (tplayer2.trackingState != OrbbecPlayerOnTracking.TrackingState.none)
					{
						tplayer = tplayer2;
					}

					if (tplayer.torsoPos.x <= 0)
					{
						isLeft = true;
					}
					else
					{
						isLeft = false;
					}

					if (tplayer.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
					{
						if (isLeft)
						{
							orbbecTrackingUI.player1Percent = 1f;
						}
						else
						{
							orbbecTrackingUI.player2Percent = 1f;
						}
					}
					else
					{
						if (isLeft)
						{
							orbbecTrackingUI.player1Percent = (tplayer.trackingTime / needTrackingTime);
						}
						else
						{
							orbbecTrackingUI.player2Percent = (tplayer.trackingTime / needTrackingTime);
						}
					}

				}
				//两个人在标定 或者标定成功 根据2个人的胸点位置，来判断左边还是右边
				else if (trackCnt == 2)
				{
					OrbbecPlayerOnTracking lplayer;
					OrbbecPlayerOnTracking rplayer;

					if (tplayer1.torsoPos.x <= tplayer2.torsoPos.x)
					{
						lplayer = tplayer1;
						rplayer = tplayer2;
					}
					else
					{
						lplayer = tplayer2;
						rplayer = tplayer1;
					}

					if (lplayer.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
					{
						orbbecTrackingUI.player1Percent = 1f;
					}
					else
					{
						orbbecTrackingUI.player1Percent = (lplayer.trackingTime / needTrackingTime);
					}

					if (rplayer.trackingState == OrbbecPlayerOnTracking.TrackingState.tracked)
					{
						orbbecTrackingUI.player2Percent = 1f;
					}
					else
					{
						orbbecTrackingUI.player2Percent = (rplayer.trackingTime / needTrackingTime);
					}
				}
			}
		}

		#endregion

		#region region 全民暴力摩托，参数接口

		//上下值
		//左右值
		//丢标回调
		//左右攻击

		void MotoOnTracked ()
		{
			//记录标定时头到胸的距离
			Vector3 tmpTrackingHeadPos = player1.GetTrackingWorldPos (SkeletonType.Head);
			Vector3 tmpTrackingTorsoPos = player1.GetTrackingWorldPos (SkeletonType.Torso);
			trackingDistanceFromHeadToTorso = Vector3.Distance (tmpTrackingHeadPos, tmpTrackingTorsoPos);

			//记录标定时右手长度
			Vector3 tmpTrackingRightHandPos = player1.GetTrackingWorldPos (SkeletonType.RightHand);
			Vector3 tmpTrackingRightShoulderPos = player1.GetTrackingWorldPos (SkeletonType.RightShoulder);
			Vector3 tmpTrackingRightElbowPos = player1.GetTrackingWorldPos (SkeletonType.RightElbow);
			trackingDistanceFromRightShoulderToHand = Vector3.Distance (tmpTrackingRightShoulderPos, tmpTrackingRightElbowPos) + Vector3.Distance (tmpTrackingRightElbowPos, tmpTrackingRightHandPos);

			//记录标定时右手长度
			Vector3 tmpTrackingLeftHandPos = player1.GetTrackingWorldPos (SkeletonType.LeftHand);
			Vector3 tmpTrackingLeftShoulderPos = player1.GetTrackingWorldPos (SkeletonType.LeftShoulder);
			Vector3 tmpTrackingLeftElbowPos = player1.GetTrackingWorldPos (SkeletonType.LeftElbow);
			trackingDistanceFromLeftShoulderToHand = Vector3.Distance (tmpTrackingLeftShoulderPos, tmpTrackingLeftElbowPos) + Vector3.Distance (tmpTrackingLeftElbowPos, tmpTrackingLeftHandPos);
		}

		void MotoUpdate()
		{
			if (isActive)
			{
				//左右上下
				tmpHeadPos = player1.GetWorldPos (SkeletonType.Head);
				tmpTorsoPos = player1.GetWorldPos (SkeletonType.Torso);

				tmpVectorFromTorsoToHead = tmpHeadPos - tmpTorsoPos;
				tmpVectorFromTorsoToHead.z = -tmpVectorFromTorsoToHead.z;
				_horizontalValue = (tmpVectorFromTorsoToHead.x / trackingDistanceFromHeadToTorso);
				_verticalValue = (tmpVectorFromTorsoToHead.z / trackingDistanceFromHeadToTorso);

				//右攻击
				tmpRightHandPos = player1.GetWorldPos (SkeletonType.RightHand);
				tmpRightShoulderPos = player1.GetWorldPos (SkeletonType.RightShoulder);

				tmpVectorFromRightShoulderToHand = tmpRightHandPos - tmpRightShoulderPos;
				rightAtkValue = (tmpVectorFromRightShoulderToHand.x / trackingDistanceFromRightShoulderToHand);
				if (rightAtkValue > 0.7f)
				{
					if (!isRightAtk)
					{
						if (rightAtkAction != null)
						{
							rightAtkAction ();
						}
						isRightAtk = true;
					}
				}
				else
				{
					isRightAtk = false;
				}
//				Debug.Log ("rightAtkValue:"+ rightAtkValue);

				//左手攻击

				tmpLeftHandPos = player1.GetWorldPos (SkeletonType.LeftHand);
				tmpLeftShoulderPos = player1.GetWorldPos (SkeletonType.LeftShoulder);

				tmpVectorFromLeftShoulderToHand = tmpLeftHandPos - tmpLeftShoulderPos;
				leftAtkValue = (-tmpVectorFromLeftShoulderToHand.x / trackingDistanceFromLeftShoulderToHand);
				if (leftAtkValue > 0.7f)
				{
					if (!isLeftAtk)
					{
						if (leftAtkAction != null)
						{
							leftAtkAction ();
						}
						isLeftAtk = true;
					}
				}
				else
				{
					isLeftAtk = false;
				}
//				Debug.Log ("leftAtkValue:"+ leftAtkValue);

				leftHandRaiseValue = (tmpVectorFromLeftShoulderToHand.y / trackingDistanceFromLeftShoulderToHand);
				if (leftHandRaiseValue > 0.7f)
				{
					if (!isLeftHandRaise)
					{
						if (leftHandRaiseAction != null)
						{
							leftHandRaiseAction ();
						}
						isLeftHandRaise = true;
					}

				}
				else
				{
					isLeftHandRaise = false;
				}
//				Debug.Log ("leftHandRaiseValue:"+ leftHandRaiseValue);

				rightHandRaiseValue = (tmpVectorFromRightShoulderToHand.y / trackingDistanceFromRightShoulderToHand);
				if (rightHandRaiseValue > 0.7f)
				{
					if (!isRightHandRaise)
					{
						if (rightHandRaiseAction != null)
						{
							rightHandRaiseAction ();
						}
						isRightHandRaise = true;
					}

				}
				else
				{
					isRightHandRaise = false;
				}
//				Debug.Log ("rightHandRaiseValue:"+ rightHandRaiseValue);
			}
			else
			{
				_horizontalValue = 0.0f;
				_verticalValue = 0.0f;
			}
				
		}

		float trackingDistanceFromHeadToTorso = 100.0f;		//标定时头到胸的距离
		Vector3 tmpHeadPos;									//当前头点坐标
		Vector3 tmpTorsoPos;								//当前胸点坐标
		Vector3 tmpVectorFromTorsoToHead;					//胸到头的向量

		float trackingDistanceFromRightShoulderToHand = 100.0f;		//标定时右手长度
		Vector3 tmpRightHandPos;					//当前右手坐标
		Vector3 tmpRightShoulderPos;				//当前右肩坐标
		Vector3 tmpVectorFromRightShoulderToHand;					//右肩到手的向量
		float rightAtkValue = 0.0f;

		float trackingDistanceFromLeftShoulderToHand = 100.0f;		//标定时左手长度
		Vector3 tmpLeftHandPos;					//当前左手坐标
		Vector3 tmpLeftShoulderPos;				//当前左肩坐标
		Vector3 tmpVectorFromLeftShoulderToHand;					//左肩到手的向量
		float leftAtkValue = 0.0f;

		float leftHandRaiseValue = 0f;
		float rightHandRaiseValue = 0f;

		bool isRightAtk = false;
		bool isLeftAtk = false;
		bool isLeftHandRaise = false;
		bool isRightHandRaise = false;

		private float _horizontalValue = 0.0f;
		public float horizontalValue
		{
			get
			{
				return _horizontalValue;
			}
		}

		private float _verticalValue = 0.0f;
		public float verticalValue
		{
			get
			{
				return _verticalValue;
			}
		}



		#endregion
	}
}

