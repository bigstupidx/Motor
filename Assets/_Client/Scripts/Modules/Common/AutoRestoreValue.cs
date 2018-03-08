//
// AutoRestoreValue.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using XPlugin.Data.Json;
using XPlugin.Security.AnitiCheatValue;

namespace GameClient {
	/// <summary>
	/// 随时间自动恢复的值
	/// </summary>
	public class AutoRestoreValue {
		/// <summary>
		/// 恢复时间（秒）
		/// </summary>
		[JsonIgnore]
		public int RestoreTime;

		/// <summary>
		/// 最大值
		/// </summary>
		[JsonIgnore]
		public int MaxValue;
		/// <summary>
		/// 值改变时候调用(old,new)
		/// </summary>
		[JsonIgnore]
		public Action<int, int> OnChange = null;

		protected bool canOverflow;
		protected cint value = 0;
		protected long restoreStart = 0;
		protected long restoreEnd = 0;

		public long RestoreStart {
			get { return this.restoreStart; }
		}

		public AutoRestoreValue() {
		}

		public AutoRestoreValue(bool canOverflow, int maxValue, int restoreTime) {
			this.canOverflow = canOverflow;
			this.MaxValue = maxValue;
			this.RestoreTime = restoreTime;
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="restoreStart">恢复开始时间</param>
		public void SetValue(int value, long restoreStart) {
			int oldValue = NowValue;
			this.value = value;
			this.restoreStart = restoreStart;
			ReCalc();
			int newValue = NowValue;
			if (oldValue != newValue && OnChange != null) {
				OnChange(oldValue, newValue);
			}
		}

		/// <summary>
		/// 设置最大值
		/// </summary>
		/// <param name="maxValue"></param>
		public void SetMaxValue(int maxValue) {
			int oldValue = NowValue;
			MaxValue = maxValue;
			ReCalc();
			int newValue = NowValue;
			if (oldValue != newValue && OnChange != null) {
				OnChange(oldValue, newValue);
			}
		}

		/// <summary>
		/// 设置恢复时间（中途改变恢复间隔会引起值变化）
		/// </summary>
		/// <param name="restoreTime"></param>
		public void SetRestoreTime(int restoreTime) {
			int oldValue = NowValue;
			RestoreTime = restoreTime;
			ReCalc();
			int newValue = NowValue;
			if (oldValue != newValue && OnChange != null) {
				OnChange(oldValue, newValue);
			}
		}

		protected void ReCalc() {
			if (!canOverflow && value > MaxValue) {
				value = MaxValue;
			}

			if (value < MaxValue) {
				if (restoreStart == 0) {
					this.restoreStart = Client.System.TimeOnLocal;
				}
				restoreEnd = restoreStart + RestoreTime * (MaxValue - value);
			} else {
				this.restoreStart = restoreEnd = 0;
			}

		}

		/// <summary>
		/// 当前值
		/// </summary>
		public virtual int NowValue {
			get {
				if (restoreStart == 0) {
					return value;
				}

				long now = Client.System.TimeOnLocal;
				if (now >= restoreEnd) {
					return MaxValue;
				}

				long span = now - restoreStart;
				int get = (int)(span / RestoreTime);
				int total = value + get;
				total = (total <= MaxValue ? total : MaxValue);
				return total;
			}
		}

		/// <summary>
		/// 是否在恢复
		/// </summary>
		[JsonIgnore]
		public bool IsRestoring {
			get {
				if (restoreStart == 0 || value >= MaxValue) {
					return false;
				} else {
					return Client.System.TimeOnLocal < restoreEnd;
				}
			}
		}

		/// <summary>
		/// 是否达到最大值
		/// </summary>
		[JsonIgnore]
		public bool IsReachMax {
			get {
				return NowValue >= MaxValue;
			}
		}

		/// <summary>
		/// 恢复下一点剩余时间
		/// </summary>
		[JsonIgnore]
		public int RestoreNextTime {
			get {
				if (!IsRestoring) {
					return 0;
				} else {
					return RestoreTime - (int)(Client.System.TimeOnLocal - restoreStart) % RestoreTime;
				}
			}
		}

		/// <summary>
		/// 恢复全部剩余时间
		/// </summary>
		[JsonIgnore]
		public int RestoreAllTime {
			get {
				if (!IsRestoring) {
					return 0;
				} else {
					return (int)(restoreEnd - Client.System.TimeOnLocal);
				}
			}
		}

		/// <summary>
		/// 改变值
		/// </summary>
		/// <param name="delta"></param>
		/// <returns>是否改变成功</returns>
		public virtual bool ChangeValue(int delta) {
			int nowValue = NowValue;
			if (nowValue + delta < 0) {//不够
				return false;
			} else {
				bool lastIsFull = IsReachMax;
				int oldValue = nowValue;
				value = nowValue + delta;

				if (!canOverflow) {
					if (value > MaxValue) {
						value = MaxValue;
					}
				}

				if (value < MaxValue) {
					long now = Client.System.TimeOnLocal;
					if (restoreStart > 0&&!lastIsFull) {
						var v = (now - restoreStart) % RestoreTime;
						Debug.Log("需要减掉 " + v);
						now -= v;
					}
					restoreStart = now;
					restoreEnd = restoreStart + RestoreTime * (MaxValue - value);
				} else {
					restoreStart = 0;
					restoreEnd = 0;
				}
				Debug.Log("开始恢复时间是 " + this.restoreStart.ToDateTime().ToLongTimeString());
				Debug.Log("结束恢复时间是 " + this.restoreEnd.ToDateTime().ToLongTimeString());
				ReCalc();
				if (this.OnChange != null) {
					this.OnChange(oldValue, this.value);
				}
				return true;
			}
		}
	}
}