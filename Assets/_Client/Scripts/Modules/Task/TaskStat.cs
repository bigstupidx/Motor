//
// TaskStat.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using XPlugin.Data.Json;

namespace GameClient {
	public interface ITaskStat {
		JObject Root { get; }
		void SetStat(string key, object value);
		object GetStat(string key);
	}

	public class NullTaskStat : ITaskStat {
		public JObject Root {
			get {
				return new JObject();
			}
		}

		public void SetStat(string key, object value) {
		}

		public object GetStat(string key) {
			return null;
		}
	}
}