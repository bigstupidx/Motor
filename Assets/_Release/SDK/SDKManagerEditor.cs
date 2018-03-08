//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

public class SDKManagerEditor : SDKManagerBase {
	public override void Pay(SDKPayInfo payInfo) {
		SDKPay.Ins._PayCallbackForDebug(true);
	}

	public override void ExitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
