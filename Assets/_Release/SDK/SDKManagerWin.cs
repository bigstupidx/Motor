//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
public class SDKManagerWin : SDKManagerBase {
	public override void Pay(SDKPayInfo payInfo) {
		SDKPay.Ins._PayCallbackForDebug(true);
	}
}
