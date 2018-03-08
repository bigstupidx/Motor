//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

public class SDKManager {
	public static SDKManagerBase Instance = null;

	static SDKManager() {
#if UNITY_EDITOR
		Instance = new SDKManagerEditor();
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
		Instance = new SDKManagerWin();
#elif UNITY_ANDROID && !UNITY_EDITOR
		Instance = new SDKManagerAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
		Instance = new SDKManagerIOS();
#endif
	}

}

public enum UserResultCode {
	None = 0,
	NotSupport,

	InitSuccess,
	InitFailed,

	LoginSuccess,
	LoginFailed,

	LogoutSuccess,
	LogoutFailed,

	EnterUserCenter,
	LeaveUserCenter,

	ChangeUserSuccess,
	ChangeUserFailed
}

public enum PayResultCode {
	None = 0,
	NotSupport,

	PayInitSuccess,
	PayInitFailed,

	PaySuccess,
	PayFailed,

	ShowWaiting,
	CloseWaiting

}