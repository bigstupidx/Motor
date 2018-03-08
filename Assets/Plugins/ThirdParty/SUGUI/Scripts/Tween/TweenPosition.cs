//----------------------------------------------
//			        SUGUI
// Copyright © 2012-2015 xiaobao1993.com
//----------------------------------------------

using UnityEngine;

/// <summary>
/// 坐标补间
/// </summary>

[AddComponentMenu("SUGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;
	public Vector3 to;

    /// <summary>
    /// 是否是世界坐标
    /// </summary>
	[HideInInspector]
	public bool worldSpace = false;
    /// <summary>
    /// 是否有ugui
    /// </summary>
    public bool notUGUI = false;
    RectTransform mRectTransform;

    public Transform cachedTransform 
    { 
        get 
        {
            if (!notUGUI)
            {
                if (mRectTransform == null)
                {
                    mRectTransform = gameObject.GetComponent<RectTransform>();
                    if(mRectTransform == null)
                    {
                        notUGUI = true;
                        return transform;
                    }
                }
                return mRectTransform;
            }
            else
            {
                return transform;
            }
        } 
    }
	public Vector3 value
	{
		get
		{
			return worldSpace ? cachedTransform.position : notUGUI? cachedTransform.localPosition: V2toV3(((RectTransform)cachedTransform).anchoredPosition) ;
		}
		set
		{
            if (worldSpace) cachedTransform.position = value;
			else{
				if (notUGUI)
					cachedTransform.localPosition = value;
				else ((RectTransform)cachedTransform).anchoredPosition = V3toV2(value);
			}
				

        }
	}

	public Vector3 V2toV3(Vector2 origin){
		return new Vector3 (origin.x, origin.y, 0f);
	}

	public Vector2 V3toV2(Vector3 origin){
		return new Vector2 (origin.x, origin.y);
	}

	void Awake () 
    {
        mRectTransform = GetComponent<RectTransform>();
        if (mRectTransform == null) notUGUI = true;
    }


	protected override void OnUpdate (float factor, bool isFinished) 
    { 
        value = from * (1f - factor) + to * factor; 
    }

	/// <summary>
	/// 开始补间操作
	/// </summary>

	static public TweenPosition Begin(GameObject go, float duration, Vector3 pos, bool isWorldSpace = false) 
	{
		TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
		comp.from = comp.value;
		comp.to = pos;

		comp.worldSpace = isWorldSpace;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("设置当前值为From的值")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("设置当前值为To的值")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("切换到From值状态")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("切换到To值状态")]
	void SetCurrentValueToEnd () { value = to; }
}
