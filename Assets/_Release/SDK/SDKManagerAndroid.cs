//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;

public class SDKManagerAndroid : SDKManagerBase {

	private static AndroidJavaObject jIns = null;

	private static AndroidJavaObject JIns {
		get {
			if (jIns == null) {
				AndroidJavaClass jclass = new AndroidJavaClass("com.xiaoxi.SDKManagerImpl");
				jIns = jclass.CallStatic<AndroidJavaObject>("getInstance");
			}
			return jIns;
		}
	}

	public override bool IsSupport(string method) {
		return JIns.Call<bool>("isSupport", method);
	}

	public override void Pay(SDKPayInfo info) {
		JIns.Call("pay", info.ToJson());
	}

	public override string GetProductInfo(int payCode) {
		return JIns.Call<string>("getProductInfo", payCode + "");
	}

	public override void SetPayCallBack(MonoBehaviour gameObject, string method) {
		JIns.Call("setPayCallback", gameObject.name, method);
	}

	public override void Login() {
		JIns.Call("login");
	}

	public override void SetLoginCallBack(MonoBehaviour gameObject, string method) {
		JIns.Call("setLoginCallback", gameObject.name, method);
	}

	//版本号
	public override string GetVersionCode() {
		return JIns.Call<string>("getVersionCode");
	}

	//运营商ID
	public override int GetNetworkID() {
		return JIns.Call<int>("getNetworkID");
	}

	//设备ID
	public override string GetUID() {
		if (string.IsNullOrEmpty(UID)) {
			UID = JIns.Call<string>("getDeviceID");
			CheckUID();
		}
		return UID;
	}

	public override string GetSdkNickname() {
		return jIns.Call<string>("getSdkNickname");
	}

	//渠道ID
	public override int GetChannelId() {
		return JIns.Call<int>("getChannelID");
	}

	//渠道名称
	public override string GetChannelName() {
		return JIns.Call<string>("getChannelName");
	}

	public override RegionEnum GetCountryOrRegionId() {
		RegionEnum region;
		try {
			string id = JIns.Call<string>("getCountryId");
			if (string.IsNullOrEmpty(id)) {
				region = RegionEnum.UnKnown;
			} else {

				region = (RegionEnum)Enum.Parse(typeof(RegionEnum), id, true);
			}
		} catch (Exception e) {
			Debug.Log(e.ToString());
			region = RegionEnum.UnKnown;
		}
		return region;
	}

	//省份ID
	public override int GetProvinceId() {
		return JIns.Call<int>("getProvinceId");
	}

	//城市ID
	public override int GetCityId() {
		return JIns.Call<int>("getCityId");
	}

	public override void MoreGame() {
		JIns.Call("moreGame");
	}

	public override void ExitGame() {
		JIns.Call("exitGame");
	}
}
