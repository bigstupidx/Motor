/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using UnityEngine;

public sealed class ExplodePart : MonoBehaviour
{
	[Serializable]
	public class TransformParams
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
	}

	public TransformParams FinalTransform
	{
		get
		{
			return m_finalTransform;
		}

		set
		{
			m_finalTransform = value;
		}
	}

	public TransformParams InitialTransform
	{
		get
		{
			return m_initialTransform;
		}

		set
		{
			m_initialTransform = value;
		}
	}

	[SerializeField]
	private TransformParams m_finalTransform;

	[SerializeField]
	private TransformParams m_initialTransform;

	#region Unity core events.
	#endregion //Unity core events.

	#region Class functions.
	public void ApplyExplodeOffset(float offset)
	{
		Transform cachedTransform = transform;
		offset = Mathf.Clamp01(offset);

		cachedTransform.localPosition = Vector3.Lerp(InitialTransform.Position, FinalTransform.Position, offset);
		cachedTransform.localRotation = Quaternion.Slerp(InitialTransform.Rotation, FinalTransform.Rotation, offset);
		cachedTransform.localScale = Vector3.Lerp(InitialTransform.Scale, FinalTransform.Scale, offset);
	}
	#endregion //Class functions.
}
