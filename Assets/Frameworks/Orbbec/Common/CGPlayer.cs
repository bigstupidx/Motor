﻿using UnityEngine;
using System.Collections;

public class CGPlayer : MonoBehaviour
{
#if ((!UNITY_ANDROID && !UNITY_IPHONE) || UNITY_EDITOR)
	public MovieTexture movTexture;
#endif
	public string CGPath;

	public bool IsChangeSceneOnFinish = true;
	public int NextSceneID = 1;

#if ((UNITY_ANDROID || UNITY_IPHONE  && !UNITY_EDITOR) )
	static bool IsSaveExitOnPause = false;
	static bool IsExitOnPauseBefore = false;
#endif

	Rect rt;

	bool IsEnterPauseBefore = false;

	// Use this for initialization
	void Awake()
	{
#if ((UNITY_ANDROID || UNITY_IPHONE ) && !UNITY_EDITOR )
		IsEnterPauseBefore = false;
		if (Orbbec.OrbbecManager.Instance != null)
		{
			Log.Print(Log.Level.Log, "Reset ExitOnPause to false");
			IsExitOnPauseBefore = Orbbec.OrbbecManager.Instance.Param.IsExitOnPause;
			Orbbec.OrbbecManager.Instance.Param.IsExitOnPause = false;
			IsSaveExitOnPause = true;
		}
		Handheld.PlayFullScreenMovie(CGPath, Color.black,
							FullScreenMovieControlMode.CancelOnInput,
							FullScreenMovieScalingMode.AspectFit);
#else
		if (movTexture == null)
		{
			movTexture = Resources.Load(CGPath) as MovieTexture;
		}
		if (movTexture == null)
			return;
		rt = new Rect(0, 0, Screen.width, Screen.height);
		movTexture.loop = false;
		movTexture.Play();
#endif
	}

	void OnGUI()
	{
#if ((!UNITY_ANDROID && !UNITY_IPHONE  && !UNITY_EDITOR))
		GUI.DrawTexture( rt, movTexture);
#endif
	}

	public void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			IsEnterPauseBefore = true;
		}
		else
		{
#if ((UNITY_ANDROID || UNITY_IPHONE ))
			if (!IsEnterPauseBefore)
				return;

			Destroy(gameObject);
			if (IsSaveExitOnPause)
			{
				Log.Print(Log.Level.Log, "Reset ExitOnPause to true");
				Orbbec.OrbbecManager.Instance.Param.IsExitOnPause = IsExitOnPauseBefore;
			}

			if (IsChangeSceneOnFinish)
			{
#if UNITY_4_6 || UNITY_4_7
				Application.LoadLevel(NextSceneID);
#else
				UnityEngine.SceneManagement.SceneManager.LoadScene(NextSceneID);
#endif
			}
#else
		if (!movTexture.isPlaying)
		{
			Destroy(gameObject);
		}
#endif
		}
	}

	// Update is called once per frame
	void Update()
	{
#if ((UNITY_ANDROID || UNITY_IPHONE ))
		// 		Destroy(gameObject);
		// 		if (IsSaveExitOnPause)
		// 		{
		// 			Log.Print(Log.Level.Log, "Reset ExitOnPause to true");
		// 			Orbbec.OrbbecManager.Instance.m_Param.IsExitOnPause = IsExitOnPauseBefore;
		// 		}
		// 
		// 		if (IsChangeSceneOnFinish)
		// 		{
		// 			Application.LoadLevel(NextSceneID);
		// 		}
#else
		if (!movTexture.isPlaying)
		{
			Destroy(gameObject);
		}
#endif
	}
}
