//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using System.Collections.Generic;
using GameClient;
using UnityEngine;
public class AnalyticsMgrBase : MonoBehaviour , IAnalyticsgr {
	public TDGAAccount Account = null;
    public static bool UseAnalytics { get; private set; }

	public static AnalyticsMgrBase Ins { get; private set; }

	protected virtual void Awake() {
		Ins = this;
#if (UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR)
		UseAnalytics = Client.Ins.EnableAnalyze;
#else
		UseAnalytics = false;
#endif
	}
	/// <summary>
	/// 进入游戏或游戏从后台恢复时调用
	/// </summary>
	public virtual void OnStart() {
    }

	/// <summary>
	/// 退出游戏或游戏切换到后台时调用
	/// </summary>
	public virtual void OnUpdate() {
	}

    public virtual void OnEnd() { 
	}

    /// <summary>
    /// 设置账号ID
    /// </summary>
    /// <param name="uid"></param>
    public virtual void SetAccountID(string id) {
	}

	/// <summary>
	/// 设置账户名
	/// </summary>
	/// <param name="name">账户名</param>
	public virtual void SetAccountName(string name) {
	}
	/// <summary>
	/// 设置服务器名
	/// </summary>
	/// <param name="server">服务器名</param>
	public virtual void SetServer(string server) {
	}

	/// <summary>
	/// 设置等级
	/// </summary>
	/// <param name="level">等级</param>
	public virtual void SetLevel(int level) {
	}

	/// <summary>
	/// 充值
	/// </summary>
	/// <param name="iapID">充值订单号</param>
	/// <param name="price">充值金额</param>
	/// <param name="currency">获得虚拟币</param>
	public virtual void Pay(string iapID, double price, double currency) {
	}

	/// <summary>
	/// 奖励虚拟币
	/// </summary>
	/// <param name="amount">数量</param>
	/// <param name="reason">理由</param>
	public virtual void Reward(double amount, string reason) {
	}

	/// <summary>
	/// 购买道具或者消费点
	/// </summary>
	/// <param name="data"></param>
	public virtual void Purchase(IAPData data) {
	}

	/// <summary>
	/// 购买道具或者消费点
	/// </summary>
	/// <param name="item">名称</param>
	/// <param name="amount">数量</param>
	/// <param name="price">单价</param>
	public virtual void Purchase(string item, int amount, double price) {
	}

    /// <summary>
    /// 武器购买
    /// </summary>
    /// <param name="itemid">武器id</param>
    /// <param name="amount">数量</param>
    /// <param name="price">价格</param>
    /// <param name="currcurrencyID">货币id</param>
    public virtual void BuyWeapon(string itemid, int amount, double price, int currcurrencyID) {
    }

    /// <summary>
    /// 道具购买
    /// </summary>
    /// <param name="itemid">道具id</param>
    /// <param name="amount">数量</param>
    /// <param name="price">价格</param>
    /// <param name="currcurrencyID">货币id</param>
    public virtual void BuyProp(string itemid, int amount, double price, int currcurrencyID) {
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="item">名称</param>
    /// <param name="amount">数量</param>
    public virtual void Use(string item, int amount) {
	}

	/// <summary>
	/// 游戏开始
	/// </summary>
	/// <param name="gameID">关卡ID</param>
	public virtual void GameStart(string gameID) {
	}

	/// <summary>
	/// 游戏胜利
	/// </summary>
	/// <param name="gameID">关卡ID</param>
	public virtual void GameWin(string gameID) {
	}

	/// <summary>
	/// 游戏失败
	/// </summary>
	/// <param name="gameID">关卡ID</param>
	public virtual void GameFail(string gameID, string reason = "") {
	}

	/// <summary>
	/// 自定义事件
	/// </summary>
	/// <param name="eventID">时间ID</param>
	/// <param name="args">事件参数</param>
	public virtual void Event(string eventID, Dictionary<string, object> args = null) {
	}

	/// <summary>
	/// 自定义事件
	/// </summary>
	/// <param name="eventID">时间ID</param>
	/// <param name="args">事件参数</param>
	/// <param name="number">统计数字</param>
	public virtual void Event(string eventID, Dictionary<string, object> args, double number) {
	}


	/// <summary>
	/// 初始化统计模块，并将对应平台的实例挂载到指定模块上
	/// </summary>
	/// <param name="mod"></param>
	public static void Init(ClientModule mod) {
		if (mod == null) {
			return;
		}
#if UNITY_ANDROID && !UNITY_EDITOR
		mod.gameObject.AddComponent<UmengManager>();
#elif UNITY_IOS && !UNITY_EDITOR
		mod.gameObject.AddComponent<AnalyticsMgrIOS>();
#else
        mod.gameObject.AddComponent<UmengManager>();
#endif


    }

}
