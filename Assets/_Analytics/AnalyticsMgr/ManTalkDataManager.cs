using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameClient;

public class ManTalkDataManager : AnalyticsMgrBase {

	public string TalkingDataAppId = "91B94B69079848A68CE8C7CEF3DD0C6B";


	public override void OnStart() {
        if (UseAnalytics) {
            string appid = this.TalkingDataAppId;
            string channel = SDKManager.Instance.GetChannelName();
            if (string.IsNullOrEmpty(appid)) {
                Debug.LogError("Analytics appid is null!");
            }
            if (string.IsNullOrEmpty(channel)) {
                Debug.LogError("Analytics channel is null!");
            }
            TalkingDataGA.OnStart(appid, channel);
            Debug.Log("[AnalyticsMgr]TalkData Start " + appid + " " + channel);

        }
    }


    public override void OnEnd() {
        if (UseAnalytics) {
            TalkingDataGA.OnEnd();
        }
    }

    /// <summary>
    /// 设置账号ID
    /// </summary>
    /// <param name="uid"></param>
    public override void SetAccountID(string id) {
        if (UseAnalytics) {
            Account = TDGAAccount.SetAccount(id);
        }
    }

    /// <summary>
    /// 设置账户名
    /// </summary>
    /// <param name="name">账户名</param>
    public virtual void SetAccountName(string name) {
        if (UseAnalytics && Account != null) {
            Account.SetAccountName(name);
        }
    }
    /// <summary>
    /// 设置服务器名
    /// </summary>
    /// <param name="server">服务器名</param>
    public virtual void SetServer(string server) {
        if (UseAnalytics && Account != null) {
            Account.SetGameServer(server);
        }
    }

    /// <summary>
    /// 设置等级
    /// </summary>
    /// <param name="level">等级</param>
    public virtual void SetLevel(int level) {
        if (UseAnalytics && Account != null) {
            Account.SetLevel(level);

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
            TDGAVirtualCurrency.OnChargeRequest(orderID, iapID, price, "CNY", currency, SDKManager.Instance.GetChannelName());
            TDGAVirtualCurrency.OnChargeSuccess(orderID);
        }
    }

    /// <summary>
    /// 奖励虚拟币
    /// </summary>
    /// <param name="amount">数量</param>
    /// <param name="reason">理由</param>
    public override void Reward(double amount, string reason) {
        if (UseAnalytics) {
            TDGAVirtualCurrency.OnReward(amount, reason);

        }
    }

    /// <summary>
    /// 购买道具或者消费点
    /// </summary>
    /// <param name="data"></param>
    public virtual void Purchase(IAPData data) {
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
    public virtual void Purchase(string item, int amount, double price) {
        if (UseAnalytics) {
            TDGAItem.OnPurchase(item, amount, price);

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

    }

    /// <summary>
    /// 道具购买
    /// </summary>
    /// <param name="itemid">道具id</param>
    /// <param name="amount">数量</param>
    /// <param name="price">价格</param>
    /// <param name="currcurrencyID">货币id</param>
    public override void BuyProp(string itemid, int amount, double price, int currcurrencyID) {
       
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="item">名称</param>
    /// <param name="amount">数量</param>
    public virtual void Use(string item, int amount) {
        if (UseAnalytics) {
            TDGAItem.OnUse(item, amount);
        }
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="gameID">关卡ID</param>
    public override void GameStart(string gameID) {
        if (UseAnalytics) {
            TDGAMission.OnBegin(gameID);
        }
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    /// <param name="gameID">关卡ID</param>
    public override void GameWin(string gameID) {
        if (UseAnalytics) {
            TDGAMission.OnCompleted(gameID);
        }
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    /// <param name="gameID">关卡ID</param>
    public override void GameFail(string gameID, string reason = "") {
        if (UseAnalytics) {
            TDGAMission.OnFailed(gameID, reason);
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
            TalkingDataGA.OnEvent(eventID, args);
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
