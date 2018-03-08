using System;
using UnityEngine;

namespace Logging {

	public static class LogFactory {

		public static ILog ForContext() {
			return new DefaultLog(Debug.logger);
		}

		public static ILog ForContext(Type type) {
			//			if (string.IsNullOrEmpty(name)) {
			//				name = "Unkonwn";
			//			}
			return new DefaultLog(Debug.logger);
		}

		public static ILog ForContext(string name) {
			//			if (string.IsNullOrEmpty(name)) {
			//				name = "Unkonwn";
			//			}
			return new DefaultLog(Debug.logger);
		}
	}
}
