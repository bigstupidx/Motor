using UnityEngine;
using UnityEngine.UI;

public class KillEnermyTip : MonoBehaviour
{
	public UITweener[] Tweeners;
	public Text Count;
    public int Num;
	public void Reset()
	{
		for (int i = 0; i < Tweeners.Length; i++)
		{
			Tweeners[i].ResetToBeginning();
		}
	}

	public void PlayForward()
	{
		for (int i = 0; i < Tweeners.Length; i++)
		{
			Tweeners[i].PlayForward();
		}
	}

	public void SetCount(int i)
	{
        Num = i;
		Count.text = "<size=130>K.O. x</size><size=160>" + i + "</size>";
	}
}
