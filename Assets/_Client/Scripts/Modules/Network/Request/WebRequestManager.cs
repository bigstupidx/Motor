using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XPlugin.Security;

public class WebRequestManager : MonoBehaviour {
	public static WebRequestManager Instance = null;
	public float Timeout = 5.0f;
	protected List<WebRequest> addList = new List<WebRequest> ();
	protected List<WebRequest> requestList = new List<WebRequest> ();

	public void Awake () {
		Instance = this;
	}

	public void Update () {
		float now = Time.unscaledTime;
		requestList.AddRange (addList);
		addList.Clear ();

		for (int i = 0; i < requestList.Count;) {
			WebRequest webReq = requestList [i];
			if (webReq.www.isDone) {
				requestList.RemoveAt (i);
				if (webReq.www.error != null)
					webReq.errMsg = webReq.www.error;
				else if ((webReq.www.bytes.Length == 0) || string.IsNullOrEmpty(webReq.www.text)) {
					webReq.errMsg = (LString.WEBREQUESTMANAGER).ToLocalized();
				} else {

					if(webReq.www.text.Length > 20){
						if (!WebManager.Ins.isEncrypt) {
							webReq.content = webReq.www.text;
						} else {
							webReq.content = AESUtil.Decrypt (webReq.www.text, "3b5949e0c26b8776", "7a4752a276de9570");
						}
					}else{
						webReq.errMsg = (LString.WEBREQUESTMANAGER_1).ToLocalized();
					}

				}
				webReq.www.Dispose ();
				
				if (webReq.callback != null) {
					webReq.callback (webReq);
				}
			} else if (now > (webReq.startTime + Timeout)) {
				requestList.RemoveAt (i);
				webReq.errMsg = (LString.WEBREQUESTMANAGER_2).ToLocalized();

				if (webReq.callback != null) {
					webReq.callback (webReq);
				}
			} else {				i++;
			}
		}
	}

	public void SendRequest (string url, Action<WebRequest> callback) {
		if (url != null) {
			WWW www = new WWW (url);
			WebRequest item = new WebRequest (www, callback);
			addList.Add (item);
		}
	}

	public void SendRequest (string url, WWWForm data, Action<WebRequest> callback) {
		if (url != null) {
			WWW www = new WWW (url, data);
			WebRequest item = new WebRequest (www, callback);
			addList.Add (item);
		}
	}

	public void Clear () {
		requestList.Clear ();
		addList.Clear ();
	}

	public void Dispose () {
		requestList.Clear ();
	}
}

public class WebRequest {
	public WWW www;
	public float startTime;
	public Action<WebRequest> callback;
	public string content = "";
	public string errMsg = null;

	public bool Success { get { return errMsg == null; } }

	public WebRequest (WWW www, Action<WebRequest> callback) {
		startTime = Time.unscaledTime;
		this.www = www;
		this.callback = callback;
	}
}
