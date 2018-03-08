using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Orbbec;

namespace OrbbecGestures
{
	public abstract class PlayerStateBase
	{
		public OrbbecUser PlayerBindUser = null;
		public bool IsPlayerUpdating = false;

		public Point3D[] SkeletonVelocityInfo = null;

		public abstract IntPtr GetGestureInfoPtr();

		public abstract void ResetGestureInfoSize(int Size);
	}
}
