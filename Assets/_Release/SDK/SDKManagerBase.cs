//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;

public class SDKManagerBase {
	public virtual bool IsSupport(string method) {
		return false;
	}

	public string UID { get; internal set; }

	/// <summary>
	/// 支付
	/// </summary>
	public virtual void Pay(SDKPayInfo info) {
		SDKPay.Ins._PayCallbackForDebug(true);
	}

	public virtual string GetProductInfo(int payCode) {
		return "";
	}

	/// <summary>
	/// 设置支付回调
	/// </summary>
	/// <param name="gameObject"></param>
	/// <param name="method"></param>
	public virtual void SetPayCallBack(MonoBehaviour gameObject, string method) {
	}

	public virtual void Login() {
	}

	public virtual void SetLoginCallBack(MonoBehaviour gameObject, string method) {
	}

	public virtual string GetSdkNickname() {
		return null;
	}

	/// <summary>
	/// 获取设备ID
	/// </summary>
	/// <returns></returns>
	public virtual string GetUID() {
		if (string.IsNullOrEmpty(UID)) {
			UID = SystemInfo.deviceUniqueIdentifier;
			CheckUID();
		}
		return UID;
	}

	protected void CheckUID() {
		UID = UID.Replace(" ", "");
		if (string.IsNullOrEmpty(UID) || UID.Equals("00000000-0000-0000-0000-000000000000")) {
			UID = SystemInfo.deviceUniqueIdentifier;
		}
		if (string.IsNullOrEmpty(UID)) {
			UID = "Unknown";
		}
	}

	/// <summary>
	/// 获取渠道ID
	/// </summary>
	/// <returns></returns>
	public virtual int GetChannelId() {
		return 0;
	}

	/// <summary>
	/// 获取渠道名称
	/// </summary>
	/// <returns></returns>
	public virtual string GetChannelName() {
		return "Xiaoxi";
	}

	public virtual RegionEnum GetCountryOrRegionId() {
		return RegionEnum.UnKnown;
	}

	public virtual string GetVersionCode() {
		return "0.0.0";
	}

	/// <summary>
	/// 获取城市ID
	/// </summary>
	/// <returns></returns>
	public virtual int GetCityId() {
		return 999999;
	}

	/// <summary>
	/// 获取省份ID
	/// </summary>
	/// <returns></returns>
	public virtual int GetProvinceId() {
		return 999998;
	}

	/// <summary>
	/// 获取运营商ID（1移动；2电信；3联通）
	/// </summary>
	/// <returns></returns>
	public virtual int GetNetworkID() {
		return 0;
	}

	/// <summary>
	/// 网络是否连接
	/// </summary>
	/// <returns></returns>
	public virtual bool isNetworkConnected() {
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	/// <summary>
	/// 渠道类型（：  ChannelID)
	/// </summary>
	/// <returns></returns>
	public virtual int GetChannelType() {
		return 1;
	}

	public virtual void OpenRate(string appid) {
	}

	/// <summary>
	/// 更多游戏
	/// </summary>
	public virtual void MoreGame() {
	}

	/// <summary>
	/// 退出游戏
	/// </summary>
	public virtual void ExitGame() {
	}
}