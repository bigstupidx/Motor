//
// EventEnum.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
namespace GameClient {


	public delegate void Game_Boosting(float disPerFrame, float deltaTime);
	public delegate void Game_Boost(float dis, float time);
	public delegate void Game_Drifting(float disPerFrame, float deltaTime);
	public delegate void Game_Drift(float dis, float time);

	/// <summary>
	/// 事件枚举
	/// </summary>
	public enum EventEnum {
		None,

		/// <summary>
		/// 表示所有事件，注册后可得到所有事件的通知<para/>
		/// 参数：具体事件的参数
		/// </summary>
		AllEvent,

		#region SystemEvent
		/// <summary>
		/// 签到<para/>
		/// 参数：天数
		/// </summary>
		System_SignToday,

		/// <summary>
		/// 登录<para/>
		/// </summary>
		System_Login,

		/// <summary>
		/// 注销<para/>
		/// 参数：无
		/// </summary>
		System_Logout,

		/// <summary>
		/// 客户端初始化完成<para/>
		/// 参数：无
		/// </summary>
		System_ClientInited,

		/// <summary>
		/// WebSocket客户端已连接<para/>
		/// 参数：无
		/// </summary>
		System_WsClientConnected,

		/// <summary>
		/// WebSocket客户端已断开<para/>
		/// 参数：无
		/// </summary>
		System_WsClientClosed,

		/// <summary>
		/// 数据包请求
		/// </summary>
		System_PackOnRequest,

		/// <summary>
		/// 数据包响应
		/// </summary>
		System_PackOnResponse,

		/// <summary>
		/// 登陆后进入游戏<para/>
		/// 参数：无
		/// </summary>
		System_EnterGame,

		/// <summary>
		/// 跨天<para/>
		/// 参数：无
		/// </summary>
		System_OverDay,

		/// <summary>
		/// 新功能解锁<para/>
		/// 参数：SystemFunc
		/// </summary>
		System_NewFuncUnlocked,

		/// <summary>
		/// 充值完成<para/>
		/// 参数：订单ID
		/// </summary>
		System_OrderPaied,

		/// <summary>
		/// 使用兑换码<para/>
		/// 参数：无
		/// </summary>
		System_UseRewardCode,

        /// <summary>
        /// 活动查询完毕
        /// </summary>
        System_ActivityInited,

        /// <summary>
        /// 开服狂欢活动查询完毕
        /// </summary>
        System_CarnivalInited,

		/// <summary>
		/// 限时英雄查询完毕
		/// </summary>
		System_LimitHeroInited,

		#endregion

		#region Game

		/// <summary>
		/// 完成新手引导
		/// 参数：引导ID
		/// </summary>
		Game_CompleteGuide,

		/// <summary>
		/// 结束游戏<para/>
		/// 参数：模式，关卡ID，是否网络模式，是否是锦标赛, 英雄ID，车辆ID，名次，完成时间，撞毁次数，是否通过了终点
		/// </summary>
		Game_End,

		/// <summary>
		/// 获得星星<para/>
		/// 参数：星数
		/// </summary>
		Game_Star,
		/// <summary>
		/// 中途退出游戏<para/>
		/// 参数：模式，关卡ID，通关星级(0)
		/// </summary>
		Game_Abort,

		/// <summary>
		/// 创建怪物<para/>
		/// 参数：模式，关卡ID，波数ID，怪物列表
		/// </summary>
		Game_CreateEnemies,

		/// <summary>
		/// 击杀敌人<para/>
		/// 参数：数量
		/// </summary>
		Game_KillEnemy,

		/// <summary>
		/// 获得道具<para/>
		/// 参数：道具类型，数量
		/// </summary>
		Game_GainProp,

		/// <summary>
		/// 使用道具<para/>
		/// 参数：道具类型，数量
		/// </summary>
		Game_UseProp,

		/// <summary>
		/// 通过加速带<para/>
		/// 参数：数量
		/// </summary>
		Game_PassAccelerateField,

		/// <summary>
		/// /倒计时结束
		/// </summary>
		Game_CountDownFinish,

		#endregion

		#region Bike
		/// <summary>
		/// 解锁车辆<para/>
		/// 参数：车辆ID
		/// </summary>
		Bike_Unlock,
		/// <summary>
		/// 升级车辆<para/>
		/// 参数：车辆ID，升级类型
		/// </summary>
		Bike_Upgrade,
		#endregion

		#region Hero
		/// <summary>
		/// 解锁英雄<para/>
		/// 参数：英雄ID
		/// </summary>
		Hero_Unlock,
		/// <summary>
		/// 升级英雄<para/>
		/// 参数：英雄ID
		/// </summary>
		Hero_Upgrade,
		#endregion

		#region Item
		/// <summary>
		/// 获得物品<para/>
		/// 参数：物品类型，数量
		/// </summary>
		Item_Gain,//TODO 暂未发送该事件
		/// <summary>
		/// 消耗物品<para/>
		/// 参数：物品类型，数量
		/// </summary>
		Item_Use,
		/// <summary>
		/// 物品数量改变<para/>
		/// 参数：物品ID，数量
		/// </summary>
		Item_ChangeAmount,
		/// <summary>
		/// 物品购买<para/>
		/// 参数：物品ID，数量
		/// </summary>
		Item_Buy,
		#endregion

		#region UI

		UI_MainMenuBoard_Inited,

        UI_MovieFinished,

		/// <summary>
		/// 进入某界面<para/>
		/// 参数：界面类型
		/// </summary>
		UI_EnterMenu,

		/// <summary>
		/// 进入某对话框<para/>
		/// 参数：对话框类型
		/// </summary>
		UI_EnterDialog,

		/// <summary>
		/// 游戏胜利并显示结算界面<para/>
		/// 参数：模式，关卡ID，通关星级
		/// </summary>
		UI_GameOver_Win,

		/// <summary>
		/// 游戏失败并显示结算界面<para/>
		/// 参数：模式，关卡ID，通关星级
		/// </summary>
		UI_GameOver_Fail,

		/// <summary>
		/// 结算界面宝箱打开<para/>
		/// 参数：模式，关卡ID，通关星级
		/// </summary>
		UI_TreasureOpen,

        /// <summary>
        /// 主界面按钮开关
        /// 参数：开，关
        /// </summary>
        UI_OpenTopRightButton,

		#endregion

		#region Player
		/// <summary>
		/// 更改玩家昵称<para/>
		/// 参数：名字
		/// </summary>
		Player_ChangeNickName,

		/// <summary>
		/// 更改玩家头像<para/>
		/// 参数：头像ID
		/// </summary>
		Player_ChangeAvatar,

		/// <summary>
		/// 体力值数量变化<para/>
		/// 参数：原数量，新数量
		/// </summary>
		Player_StaminaChange,

		/// <summary>
		/// 购买体力<para/>
		/// 参数：无
		/// </summary>
		Player_BuyStamina,

		/// <summary>
		/// 签到<para/>
		/// 参数：普通签，VIP签
		/// </summary>
		Player_SignIn,

		#endregion

		#region Racing
		/// <summary>
		/// 到达某圈<para/>
		/// 参数：当前比赛总圈次，到达圈次
		/// </summary>
		Racing_AchieveTurn,
		#endregion

		#region Elimination
		/// <summary>
		/// 淘汰某人<para/>
		/// 参数：
		/// </summary>
		Elimination_Eliminate,
		#endregion
		
		#region Timing

		#endregion

		#region Task
		/// <summary>
		/// 领取任务奖励<para/>
		/// 参数：任务模式，任务ID
		/// </summary>
		Task_GetReward,
        #endregion
        #region Guide
        /// <summary>
        /// 教程开始
        /// 参数：教程类型
        /// </summary>
        Guide_Start,

        /// <summary>
        /// 教程操作
        /// 参数：具体操作
        /// </summary>
        Guide_Operation,

        /// <summary>
        /// 一个教程结束
        /// 参数： 教程类型
        /// </summary>
        Guide_Finish,
        #endregion
    }
}