using UnityEngine;
using System.Collections;
using Orbbec;

public class HomeKeyListenrCallBack : AndroidJavaProxy {

    public HomeKeyListenrCallBack() : base("com.orbbec.unityadapt.HomeKeyListener")
    {

    }

    void onHomeKeyDown()
    {
        Debug.LogError("HomeKeyListenrCallBack:onHomeKeyDown Start");

        if (OrbbecManager.Instance != null)
        {
            OrbbecManager.Instance.DoDestroy();
            OrbbecManager.DoExit();
        }
        else
        {
            OrbbecManager.DoExit();
           // Application.Quit();
        }
        Debug.LogError("HomeKeyListenrCallBack:onHomeKeyDown Finsh");
    }

    void onLongHomeKeyDown()
    {

    }

}
