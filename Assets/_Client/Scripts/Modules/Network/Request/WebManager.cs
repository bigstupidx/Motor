using System;
using System.Collections.Generic;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Security;

public enum WebCallBackType {
	Success,
	NoConnect,
	TimeOut
}

public class WebManager : MonoBehaviour {
	public static WebManager Ins;
	public static List<WebItem> waitList = new List<WebItem>();
	public static List<WebItem> sendList = new List<WebItem>();
	public bool isShowLog = false;
	public bool isEncrypt = false;

	public string AESKey;//"3b5949e0c26b8776"
	public string AESIV;//"7a4752a276de9570"

	private void Awake() {
		Ins = this;
	}

	void Update() {
		Send();
	}

	public void AddItem(WebItem item) {
		waitList.Add(item);
	}

	public void AddListItem(List<WebItem> list) {
		waitList.AddRange(list);
	}


	void Send() {
		if (waitList.Count <= 0) {
			return;
		}
		sendList.AddRange(waitList);
		waitList = new List<WebItem>();
		var pack = "[";
		for (var i = 0; i < sendList.Count; i++) {
			pack += sendList[i].ToJson();
			if (i < sendList.Count - 1) pack += ",";
		}
		pack += "]";
		// string pack = JsonMapper.ToJson(itemList);
		WWWForm data = new WWWForm();
		data.AddField("uid", SDKManager.Instance.GetUID());
		if (!isEncrypt) {
			data.AddField("pack", pack);
		} else {
			data.AddField("pack", AESUtil.Encrypt(pack,this.AESKey ,this.AESIV ));
		}
		if (WebManager.Ins.isShowLog) {
			Debug.Log(pack);
		}

		WebRequestManager.Instance.SendRequest(Interface.Ins.FullApiUrl, data, web => {
			if (WebManager.Ins.isShowLog) {
				Debug.Log("Res: " + web.content);
				if (!string.IsNullOrEmpty(web.errMsg)) {
					Debug.Log("Error Msg : " + web.errMsg);
				}
			}
			if (web.Success) {
				try {
					JArray arr = JArray.Parse(web.content);
					for (int i = 0; i < sendList.Count;) {
						sendList[i].content = arr[i].ToString();
						sendList[i].Success = true;
						sendList[i].CallBackType = WebCallBackType.Success;

						if (sendList[i].Callback != null) {
							sendList[i].Callback(sendList[i]);
						}
						sendList.RemoveAt(i);
					}
				} catch (Exception e) {
					if (WebManager.Ins.isShowLog) {
						Debug.LogError(e);
					}
					for (int i = 0; i < sendList.Count;) {
						sendList[i].Success = false;
						sendList[i].CallBackType = WebCallBackType.NoConnect;

						if (sendList[i].Callback != null) {
							sendList[i].Callback(sendList[i]);
						}
						sendList.RemoveAt(i);
					}
					// throw new Exception ("Server return is wrong");
				}
			} else {
				for (int i = 0; i < sendList.Count;) {
					sendList[i].Success = false;
					if (web.errMsg.Equals("Time Out")) {
						sendList[i].CallBackType = WebCallBackType.TimeOut;
					} else {
						sendList[i].CallBackType = WebCallBackType.NoConnect;
					}
					if (sendList[i].Callback != null) {
						sendList[i].Callback(sendList[i]);
					}
					sendList.RemoveAt(i);
				}
			}
		});
	}
}

[System.Serializable]
public class WebItem {
	public string A;
	public JArray P;
	public string M;
	[JsonIgnore]
	public Action<WebItem> Callback;
	[JsonIgnore]
	public bool Success = false;
	[JsonIgnore]
	public WebCallBackType CallBackType;
	[JsonIgnore]
	public string content;
	[JsonIgnore]
	public string errorCode;

	public string ToJson() {
		var result = new JObject();
		result["A"] = A;
		result["P"] = P;
		result["M"] = M;
		return result.ToString();//.Replace(@"\", "");
	}
}

