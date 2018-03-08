using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 给类 用于 home监听无法监听情况做的处理
/// </summary>
public class HomeKeyEvent : MonoBehaviour {

    // Use this for initialization
    private List<int> eventIDs=new List<int>();


	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();

    }

    void Init()
    {
	    try {
			Debug.Log("Init1");
			AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass orbbecPlayerActivity = new AndroidJavaClass("com.orbbec.hidetips.OrbbecPlayerActivity");
			HomeKeyListenrCallBack listenCallBack = new HomeKeyListenrCallBack();
			orbbecPlayerActivity.CallStatic("SetHomeKeyDownCallBack", listenCallBack);
	        Debug.Log("Init2");
	    }
	    catch (Exception e) {
			Debug.LogException(e);
	    }
    }

    public void QuitGame(string value)
    {
        Debug.LogError("User KeyDown Home trigger QuitGame");
        QuitGameEvent();
    }

    /// <summary>
    /// 在这里填写该游戏的退出逻辑
    /// </summary>
    private void QuitGameEvent()
    {
        if (Orbbec.OrbbecManager.Instance != null) Orbbec.OrbbecManager.Instance.DoDestroy();
        Orbbec.OrbbecManager.DoExit();
    }


    // 下面是第二套Home键 处理方案 可正常退出 不过只限于一个activity
    void OnApplicationPause()
    {
        CheckQuitCondition();
        eventIDs.Add(0);
    }

    void OnApplicationFocus()
    {
        CheckQuitCondition();
        eventIDs.Add(1);
    }

    void CheckQuitCondition()
    {
        return;
        bool isQuit = false;
        Debug.LogError("CheckQuitCondition:" + Time.time);
        if(Time.time>10)
        switch (eventIDs.Count)
        {
            case 2:
                if (eventIDs[0] == 0 && eventIDs[1] == 1)
                {
                    isQuit = true;
                        Debug.LogError("CheckQuitCondition:222");
                }
                break;
            case 6:
                if ((eventIDs[0] == 0 && eventIDs[1] == 1 && eventIDs[2] == 0 && eventIDs[3] == 1 && eventIDs[4] == 1 && eventIDs[5] == 0)|| eventIDs[0] == 0 && eventIDs[1] == 1 && eventIDs[2] == 1 && eventIDs[3] == 0 && eventIDs[4] == 1 && eventIDs[5] == 0)
                {
                    isQuit = true;
                    Debug.LogError("CheckQuitCondition:666");
                }
                break;
        }
        if (isQuit)
        {
            Debug.LogError("CheckQuitCondition True");
            QuitGameEvent();
        }
        else Debug.LogError("CheckQuitCondition False");
    }
}
