using UnityEngine;
using System.Collections.Generic;
using GameClient;
using System;
public interface IAnalyticsgr  {

    /// <summary>
    ///设置账户id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    void SetAccountID(string id);

    /// <summary>
    /// 设置账户名
    /// </summary>
    /// <param name="name"></param>
    void SetAccountName(string name);

    /// <summary>
    /// 设置服务器名
    /// </summary>
    /// <param name="server"></param>
    void SetServer(string server);

    /// <summary>
    /// 设置等级
    /// </summary>
    /// <param name="level"></param>
    void SetLevel(int level);

    /// <summary>
    /// 充值
    /// </summary>
    /// <param name="ipaID">充值订单号</param>
    /// <param name="price">充值金额</param>
    /// <param name="currency">获得虚拟币</param>
    void Pay(string ipaID, double price, double currency);

    /// <summary>
    /// 获得虚拟币
    /// </summary>
    /// <param name="amount">数量</param>
    /// <param name="reson">缘由</param>
    void Reward(double amount, string reson);

    /// <summary>
    /// 购买道具或消费点数
    /// </summary>
    /// <param name=""></param>
    void Purchase(IAPData data);

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="item">名次</param>
    /// <param name="amount">数量</param>
    void Use(string item, int amount);

    /// <summary>
    /// 武器购买
    /// </summary>
    /// <param name="itemid">武器名字</param>
    /// <param name="amount">价格</param>
    /// <param name="price">货币id</param>
    /// <param name="currcurrencyID"></param>
    void BuyWeapon(string itemid, int amount, double price, int currcurrencyID);

    /// <summary>
    /// 道具购买
    /// </summary>
    /// <param name="itemid">道具id</param>
    /// <param name="amount">数量</param>
    /// <param name="price">价格</param>
    /// <param name="currcurrencyID">货币id</param>
    void BuyProp(string itemid, int amount, double price, int currcurrencyID);

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="gameID"></param>
    void GameStart(string gameID);

    /// <summary>
    /// 游戏胜利
    /// </summary>
    /// <param name="gameid">关卡</param>
    void GameWin(string gameid);

    /// <summary>
    /// 游戏失败
    /// </summary>
    /// <param name="eventid"></param>
    /// <param name="arg"></param>
    void GameFail(string gameID, string reason = "");

    /// <summary>
    /// 自定义事件
    /// </summary>
    /// <param name="eventid"></param>
    /// <param name="arg"></param>
    void Event(string eventid, Dictionary<string, object> arg = null);

    /// <summary>
    /// 自定义事件
    /// </summary>
    /// <param name="evevntid"></param>
    /// <param name="arg"></param>
    /// <param name="value"></param>
    void Event(string evevntid, Dictionary<string, object> arg, double value);


}
