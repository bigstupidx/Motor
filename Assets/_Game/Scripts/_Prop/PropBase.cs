//
// PropBase.cs
//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using GameClient;

namespace Game {
	public abstract class PropBase {
		public virtual PropType Type {
			get {
				return  PropType.None;
			}
		}

		/// <summary>
		/// 使用
		/// </summary>
		/// <param name="bike"></param>
		public virtual bool Use(BikeBase bike) {
			return true;
		}

	}
}
