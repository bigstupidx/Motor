/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System.Collections;
using UnityEngine;

public sealed class ExplodePartManager : MonoBehaviour
{
	private enum AnimationDirection
	{
		Forward,
		Reverse,
	}

	[SerializeField]
	private float m_animationTime;

	[SerializeField]
	private KeyCode m_key;

	private float m_animationProgress;
	private AnimationDirection m_currentDirection;
	private ExplodePart[] m_explodeParts;

	#region Unity core events.
	private void Awake()
	{
		m_animationProgress = 0f;
		m_currentDirection = AnimationDirection.Reverse;
		m_explodeParts = GetComponentsInChildren<ExplodePart>(true);
	}

	private void Update()
	{
		if (Input.GetKeyUp(m_key))
		{
			m_currentDirection = m_currentDirection == AnimationDirection.Forward ?
				AnimationDirection.Reverse : AnimationDirection.Forward;

			StopCoroutine("PlayExplodeAnimation");
			StartCoroutine("PlayExplodeAnimation");
		}
	}
	#endregion //Unity core events.

	#region Class functions.
	private void ApplyExplodeOffset(float offset)
	{
		foreach (ExplodePart part in m_explodeParts)
		{
			part.ApplyExplodeOffset(offset);
		}
	}

	private float EaseInOutSine(float value)
	{
		return -0.5f * (Mathf.Cos(Mathf.PI * value) - 1f);
	}

	private IEnumerator PlayExplodeAnimation()
	{
		do
		{
			ApplyExplodeOffset(EaseInOutSine(m_animationProgress / m_animationTime));
			yield return null;

			m_animationProgress += m_currentDirection == AnimationDirection.Forward ? Time.deltaTime : -Time.deltaTime;
		}
		while (m_animationProgress > 0f && m_animationProgress < m_animationTime);

		m_animationProgress = Mathf.Clamp(m_animationProgress, 0f, m_animationTime);
		ApplyExplodeOffset(EaseInOutSine(m_animationProgress / m_animationTime));
	}
	#endregion //Class functions.
}
