using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbbecGestures
{
	class PlayerState_State : PlayerStateBase
	{
		public override IntPtr GetGestureInfoPtr()
		{
			return GestureInfoData.GetItemHeadPtr();
		}

		public override void ResetGestureInfoSize(int Size)
		{
			GestureInfoData.Resize(Size);
		}

		public OGList<GestureInfo_State> GestureInfoData = new OGList<GestureInfo_State>();
	}
}
