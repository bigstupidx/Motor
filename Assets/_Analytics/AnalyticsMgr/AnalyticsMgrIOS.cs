//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

public class AnalyticsMgrIOS : AnalyticsMgrBase {

	public override void OnStart() {
		base.OnStart();
#if UNITY_IPHONE
		if (UseAnalytics){
			UnityEngine.iOS.NotificationServices.RegisterForNotifications(
			UnityEngine.iOS.NotificationType.Alert |
			UnityEngine.iOS.NotificationType.Badge |
			UnityEngine.iOS.NotificationType.Sound);
		}
#endif
	}

	public override void OnUpdate() {
		base.OnUpdate();
		if (UseAnalytics) {
#if UNITY_IPHONE
			TalkingDataGA.SetDeviceToken();
			TalkingDataGA.HandleTDGAPushMessage();
#endif
		}
	}
}
