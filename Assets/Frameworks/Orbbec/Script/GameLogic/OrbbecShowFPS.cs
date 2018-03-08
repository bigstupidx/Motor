using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OrbbecShowFPS : MonoBehaviour 
{
	int MaxFrameInCalculation = 200;

	int[] UsingTimeInFrame = null;

	int TotalTime = 0;

	int CalCount = 0;

	bool HasFullList = false;

	public int FPS = 0;

	void Start () 
	{
		UsingTimeInFrame = new int[MaxFrameInCalculation];
		CalCount = 0;
		HasFullList = false;
// 		// Normal Version do not need to show FPS
// #if !TEST
// 		Destroy(gameObject);
// #endif
	}

	// Update is called once per frame
	void Update () 
	{
		int curTime = (int)(Time.unscaledDeltaTime * 1000);

		if (HasFullList)
		{
			TotalTime -= UsingTimeInFrame[CalCount];
		}

		UsingTimeInFrame[CalCount] = curTime;
		TotalTime += curTime;

		++CalCount;


		HasFullList |= CalCount >= MaxFrameInCalculation;
		CalCount %= MaxFrameInCalculation;

		if (TotalTime == 0)
			return;

		if (HasFullList)
		{
			FPS = MaxFrameInCalculation * 1000 / TotalTime;
		}
		else
		{
			FPS = CalCount * 1000 / TotalTime;
		}
	}
}
