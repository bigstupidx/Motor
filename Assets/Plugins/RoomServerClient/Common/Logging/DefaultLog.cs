// Copyright (c) Johnny Z. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Logging {
	using UnityEngine;
	using System;
	using System.Globalization;

	public sealed class DefaultLog : ILog {
//		static readonly Func<object, Exception, string> MessageFormatter = Format;
		readonly ILogger logger;

		public DefaultLog(ILogger logger) {
			this.logger = logger;
		}

		static string Format(object target, Exception exception) {
			string message = target != null ? target.ToString() : string.Empty;
			return exception == null ? message : message + " " + exception;
		}

		public bool IsTraceEnabled { get { return this.logger.IsLogTypeAllowed(LogType.Log); } }
		public bool IsDebugEnabled { get { return this.logger.IsLogTypeAllowed(LogType.Log); } }
		public bool IsInfoEnabled { get { return this.logger.IsLogTypeAllowed(LogType.Log); } }
		public bool IsWarnEnabled { get { return this.logger.IsLogTypeAllowed(LogType.Warning); } }
		public bool IsErrorEnabled { get { return this.logger.IsLogTypeAllowed(LogType.Error); } }
		public bool IsCriticalEnabled { get { return this.logger.IsLogTypeAllowed(LogType.Exception); } }

		public void Trace(object obj) {
			if (!this.IsTraceEnabled) {
				return;
			}

			this.Trace(obj, null);
		}

		public void Trace(object obj, Exception exception) {
			if (!this.IsTraceEnabled) {
				return;
			}
			if (this.logger != null) {
				this.logger.Log(LogType.Log, Format(obj, exception));
			}
		}

		public void TraceFormat(string format, params object[] args) {
			if (!this.IsTraceEnabled) {
				return;
			}
			this.TraceFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args) {
			if (!this.IsTraceEnabled
				|| formatProvider == null
				|| string.IsNullOrEmpty(format)) {
				return;
			}

			if (this.logger != null) {
				string message = string.Format(formatProvider, format, args);
				this.logger.Log(LogType.Log, message);
			}
		}

		public void Debug(object obj) {
			if (!this.IsDebugEnabled) {
				return;
			}

			this.Debug(obj, null);
		}

		public void Debug(object obj, Exception exception) {
			if (!this.IsDebugEnabled) {
				return;
			}
			if (this.logger != null) {
				this.logger.Log(LogType.Log, Format(obj, exception));
			}
		}

		public void DebugFormat(string format, params object[] args) {
			if (!this.IsDebugEnabled) {
				return;
			}

			this.DebugFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args) {
			if (!this.IsDebugEnabled
				|| formatProvider == null
				|| string.IsNullOrEmpty(format)) {
				return;
			}
			if (this.logger != null) {
				string message = string.Format(formatProvider, format, args);
				this.logger.Log(LogType.Log, message);
			}
		}

		public void Info(object obj) {
			if (!this.IsInfoEnabled) {
				return;
			}
			this.Info(obj, null);
		}

		public void Info(object obj, Exception exception) {
			if (!this.IsInfoEnabled) {
				return;
			}
			if (this.logger != null) {
				this.logger.Log(LogType.Log, Format(obj, exception));
			}
		}

		public void InfoFormat(string format, params object[] args) {
			if (!this.IsInfoEnabled) {
				return;
			}

			this.InfoFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args) {
			if (!this.IsInfoEnabled
				|| formatProvider == null
				|| string.IsNullOrEmpty(format)) {
				return;
			}
			if (this.logger != null) {
				string message = string.Format(formatProvider, format, args);
				this.logger.Log(LogType.Log, message);
			}
		}

		public void Warn(object obj) {
			if (!this.IsWarnEnabled) {
				return;
			}

			this.Warn(obj, null);
		}

		public void Warn(object obj, Exception exception) {
			if (!this.IsWarnEnabled) {
				return;
			}
			if (this.logger != null) {
				this.logger.Log(LogType.Warning, Format(obj, exception));
			}
		}

		public void WarnFormat(string format, params object[] args) {
			if (!this.IsWarnEnabled) {
				return;
			}

			this.WarnFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args) {
			if (!this.IsWarnEnabled
				|| formatProvider == null
				|| string.IsNullOrEmpty(format)) {
				return;
			}

			if (this.logger != null) {
				string message = string.Format(formatProvider, format, args);
				this.logger.Log(LogType.Warning, message);
			}
		}

		public void Error(object obj) {
			if (!this.IsErrorEnabled) {
				return;
			}

			this.Error(obj, null);
		}

		public void Error(object obj, Exception exception) {
			if (!this.IsErrorEnabled) {
				return;
			}
			if (this.logger != null) {
				this.logger.Log(LogType.Error, Format(obj, exception));
			}
		}

		public void ErrorFormat(string format, params object[] args) {
			if (!this.IsErrorEnabled) {
				return;
			}

			this.ErrorFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args) {
			if (!this.IsErrorEnabled
				|| formatProvider == null
				|| string.IsNullOrEmpty(format)) {
				return;
			}
			if (this.logger != null) {
				string message = string.Format(formatProvider, format, args);
				this.logger.Log(LogType.Error, message);
			}
		}

		public void Critical(object obj) {
			if (!this.IsCriticalEnabled) {
				return;
			}

			this.Critical(obj, null);
		}

		public void Critical(object obj, Exception exception) {
			if (!this.IsCriticalEnabled) {
				return;
			}
			if (this.logger != null) {
				this.logger.Log(LogType.Exception, Format(obj, exception));
			}
		}

		public void CriticalFormat(string format, params object[] args) {
			if (!this.IsCriticalEnabled) {
				return;
			}

			this.CriticalFormat(CultureInfo.CurrentCulture, format, args);
		}

		public void CriticalFormat(IFormatProvider formatProvider, string format, params object[] args) {
			if (!this.IsCriticalEnabled
				|| formatProvider == null
				|| string.IsNullOrEmpty(format)) {
				return;
			}
			if (this.logger != null) {
				string message = string.Format(formatProvider, format, args);
				this.logger.Log(LogType.Exception, message);
			}
		}
	}
}
