using UnityEngine;
using System;
using System.Collections.Generic;

namespace Orbbec
{
	[System.Serializable]
	public class OrbbecHand
	{
		public int HandID = 0;
		public CatchState CurCatchState = CatchState.UnCatch;
		Vector3 m_OriginHandsPos	= Vector3.zero;
		Vector3 m_CurHandsPos		= Vector3.zero;
		Vector3 m_CurScreenPercent = Vector3.zero;

		HandInfo info = new HandInfo();

		public Vector3 OriginHandsPos
		{
			get
			{
				return m_OriginHandsPos;
			}
		}


		public Vector3 CurHandsPos
		{
			get
			{
				return m_CurHandsPos;
			}
		}

		public Vector3 CurScreenPercent
		{
			get
			{
				return m_CurScreenPercent;
			}
		}

		public OrbbecHand(int handID)
		{
			HandID = handID;
		}

		public void Update()
		{
			OrbbecManager refOrbbecManager = OrbbecManager.Instance;
			if (refOrbbecManager == null)
				return;

			if (!refOrbbecManager.IsTrackingHand(HandID))
			{
				return;
			}

			if (!refOrbbecManager.GetHandInfo(HandID, ref info))
			{
				return;
			}

			Point3D.Point3ToVec3(ref m_OriginHandsPos, ref info.OriginHandsPos);
			Point3D.Point3ToVec3(ref m_CurHandsPos, ref info.CurHandsPos);
			Point3D.Point3ToVec3(ref m_CurScreenPercent, ref info.ScreenPercent);

			CurCatchState = refOrbbecManager.GetHandCatchState(HandID);
		}
	}
}
