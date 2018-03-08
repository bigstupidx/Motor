using System;
using System.Collections;
using GameClient;
using UnityEngine;
using XPlugin.Data.Json;

public class SDKPayInfo {
	public int Id;
	public string Name;
	public string Desc;
	public float Price;
	public int Amount;
	public string OrderId;
	public string UserId;
	public string CallbackUrlBase;

	public string ToJson() {
		return JsonMapper.ToJson(this);
	}
}

public class SDKPay : Singleton<SDKPay> {
	private BuyItemData currentIapData;
	Action<bool> callback = null;

	private Action<int> onCachedOrderPay = null;

	void Start() {
		SDKManager.Instance.SetPayCallBack(this, "_PayCallback");
	}

	private void Reset() {
		currentIapData = null;
		callback = null;
	}

	public void Pay(BuyItemData data, Action<bool> onDone = null) {
//		if (currentIapData != null) {
//			Debug.LogError("==>已经有一个订单正在处理");
//			return;
//		}
		currentIapData = data;
		callback = onDone;
		GetOrderId(data, orderId => {
			if (string.IsNullOrEmpty(orderId)) {
				_PayCallback("create orderId fail");
			} else {
				SDKPayInfo info = this.currentIapData.GetPayInfo();
				info.UserId = SDKManager.Instance.UID;
				info.OrderId = orderId;
				info.CallbackUrlBase = Client.Config.WebHost;
				SDKManager.Instance.Pay(info);
			}
		});
	}

	void _PayCallback(string result) {
		Debug.Log("==>SDK pay callback : " + result);
		bool success = result.Equals("success");
		if (callback != null) {
			callback(success);
		}
		Reset();
	}

	private void GetOrderId(BuyItemData data, Action<string> callback) {
		if (callback == null) throw new ArgumentNullException("callback");
		WebManager.Ins.AddItem(new WebItem {
			M = "Order",
			A = "CreateOrder",
			P = new JArray(new[] { SDKManager.Instance.GetChannelId(), data.ID }),
			Callback = item => {
				if (item.Success) {
					JObject root = JObject.Parse(item.content);
					if (root["code"].AsInt() == 0) {
						var result = root["result"].AsArray();
						var order_id = result[0].AsString();
						callback(order_id);
					} else {
						callback(null);
					}
				} else {
					callback(null);
				}
			}
		});
		//单机的时候用
		//		return (Client.System.NowTimeStamp + Random.Range(0, 10000)).ToString();
	}

	#region for debug
	public void _PayCallbackForDebug(bool result) {
		StartCoroutine(_delayCallback(result));
	}

	private IEnumerator _delayCallback(bool result) {
		yield return new WaitForSecondsRealtime(0.1f);
		if (callback != null) {
			var call = callback;
			call(result);
		}
		Reset();
	}
	#endregion
}
