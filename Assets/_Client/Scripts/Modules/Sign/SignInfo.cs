namespace GameClient {
	public class SignInfo {
		/// <summary>
		/// 上一次签到时间
		/// </summary>
		public long LastSignTime {
			get { return _lastSignTime; }
			set {
				if (this._lastSignTime != value) {
					_lastSignTime = value;
					Client.Sign.SaveData();
				}
			}
		}

		private long _lastSignTime = 0;

		/// <summary>
		/// 连续签到进度
		/// </summary>
		public int Schedule {
			get { return _schedule; }
			set {
				if (this._schedule != value) {
					_schedule = value;
					Client.Sign.SaveData();
				}
			}
		}

		private int _schedule = -1;
	}


}
