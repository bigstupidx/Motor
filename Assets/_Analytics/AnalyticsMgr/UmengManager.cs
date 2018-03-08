//
//  UmengManager.cs
//
//  Created by ZhuCong on 1/1/14.
//  Copyright 2014 Umeng.com . All rights reserved.
//  Version 1.31

using UnityEngine;
using System.Collections;
using Umeng;
using System.Collections.Generic;
using GameClient;
using System;

public class UmengManager : AnalyticsMgrBase {

	public string UMengAppID {
		get {
#if UNITY_ANDROID
			return Client.Config.AnaylseAppKey;
			return "59dec88276661321d200002b";
#elif UNITY_IPHONE
        return "";
#else
		return "";
#endif
		}
	}

	/// <summary>
	/// 进入游戏或游戏从后台恢复时调用
	/// </summary>
	public override void OnStart() {
		if (UseAnalytics) {
			string channel = "";
			GA.SetLogEnabled(true);
			channel = SDKManager.Instance.GetChannelName();
			if (string.IsNullOrEmpty(UMengAppID)) {
				Debug.LogError("Analytics appid is null!");
			}
			if (string.IsNullOrEmpty(channel)) {
				Debug.LogError("Analytics channel is null!");
			}
			GA.StartWithAppKeyAndChannelId(UMengAppID, channel);
			Debug.Log("[AnalyticsMgr] Umeng Start " + UMengAppID + " " + channel);
		}
	}

	void OnApplicationPause(bool isPause) {
		if (UseAnalytics) {
			if (isPause) {
				GA.onPause();
			} else {
				GA.onResume();
			}
		}
	}



	/// <summary>
	/// 设置等级
	/// </summary>
	/// <param name="level">等级</param>
	public override void SetLevel(int level) {
		if (UseAnalytics && Account != null) {
			GA.SetUserLevel(level);
		}
	}

	/// <summary>
	/// 充值
	/// </summary>
	/// <param name="iapID">充值订单号</param>
	/// <param name="price">充值金额</param>
	/// <param name="currency">获得虚拟币</param>
	public override void Pay(string iapID, double price, double currency) {
		if (UseAnalytics) {
			var orderID = Guid.NewGuid().ToString();
			// Debug.Log("<color=yellow>[ TGA Pay ]</color> \t" + iapID + "\t" + price.ToString("f2") + "\t" +currency.ToString("f2"));
			GA.Pay(price, GA.PaySource.Paypal, currency);
		}
	}

	/// <summary>
	/// 奖励虚拟币
	/// </summary>
	/// <param name="amount">数量</param>
	/// <param name="reason">理由</param>
	public override void Reward(double amount, string reason) {
		if (UseAnalytics) {
			GA.Bonus(amount, GA.BonusSource.Source2);//系统奖励
		}
	}

	/// <summary>
	/// 购买道具或者消费点
	/// </summary>
	/// <param name="data"></param>
	public override void Purchase(IAPData data) {
		if (UseAnalytics) {
			if (data.Currency.Type == ItemType.Diamond) {
				var item = Client.Item.GetItem(data.Item.ID);
				Ins.Purchase(item.Data.Name, data.Amount, data.CurrencyAmount * 1.0 / data.Amount);
			}
		}
	}

	/// <summary>
	/// 购买道具或者消费点
	/// </summary>
	/// <param name="item">名称</param>
	/// <param name="amount">数量</param>
	/// <param name="price">单价</param>
	public override void Purchase(string item, int amount, double price) {
		if (UseAnalytics) {
			GA.Use(item, amount, price);
		}
	}

	/// <summary>
	/// 武器购买
	/// </summary>
	/// <param name="itemid">武器id</param>
	/// <param name="amount">数量</param>
	/// <param name="price">价格</param>
	/// <param name="currcurrencyID">货币id</param>
	public override void BuyWeapon(string itemid, int amount, double price, int currcurrencyID) {
		Dictionary<string, string> args = new Dictionary<string, string>();
		args["itemid"] = itemid;
		args["amount"] = amount.ToString();
		args["prices"] = price.ToString();
		args["currcurrencyID"] = currcurrencyID.ToString();
		GA.Event("Weapon_Buy", args);
	}

	/// <summary>
	/// 道具购买
	/// </summary>
	/// <param name="itemid">道具id</param>
	/// <param name="amount">数量</param>
	/// <param name="price">价格</param>
	/// <param name="currcurrencyID">货币id</param>
	public override void BuyProp(string itemid, int amount, double price, int currcurrencyID) {
		Dictionary<string, string> args = new Dictionary<string, string>();
		args["itemid"] = itemid;
		args["amount"] = amount.ToString();
		args["prices"] = price.ToString();
		args["currcurrencyID"] = currcurrencyID.ToString();
		GA.Event("Prop_Buy", args);
	}

	/// <summary>
	/// 游戏开始
	/// </summary>
	/// <param name="gameID">关卡ID</param>
	public override void GameStart(string gameID) {
		if (UseAnalytics) {
			GA.StartLevel(gameID);
			Debug.Log("umeng startLevel" + gameID);
		}
	}

	/// <summary>
	/// 游戏胜利
	/// </summary>
	/// <param name="gameID">关卡ID</param>
	public override void GameWin(string gameID) {
		if (UseAnalytics) {
			GA.FinishLevel(gameID);
			Debug.Log("UmengFinishLevel" + gameID);
		}
	}

	/// <summary>
	/// 游戏失败
	/// </summary>
	/// <param name="gameID">关卡ID</param>
	public override void GameFail(string gameID, string reason = "") {
		if (UseAnalytics) {
			GA.FailLevel(gameID);
			Debug.Log("UmengFail" + gameID);
		}
	}

	/// <summary>
	/// 自定义事件
	/// </summary>
	/// <param name="eventID">时间ID</param>
	/// <param name="args">事件参数</param>
	public override void Event(string eventID, Dictionary<string, object> args = null) {
		if (UseAnalytics) {
			if (args == null) {
				args = new Dictionary<string, object>();
			}
			Dictionary<string, string> strs = new Dictionary<string, string>(); string log = "";
			foreach (var item in args) {
				strs.Add(item.Key, item.Value.ToString());
				log += (item.Key + "," + item.Value.ToString() + "|");
			}
			GA.Event(eventID, strs);
			Debug.Log(eventID + " " + log);

		}
	}

	/// <summary>
	/// 自定义事件
	/// </summary>
	/// <param name="eventID">时间ID</param>
	/// <param name="args">事件参数</param>
	/// <param name="number">统计数字</param>
	public override void Event(string eventID, Dictionary<string, object> args, double number) {
		if (UseAnalytics) {
			if (args == null) {
				args = new Dictionary<string, object>();
			}
			args.Add("Amount", number);
			Event(eventID, args);
		}
	}

}
