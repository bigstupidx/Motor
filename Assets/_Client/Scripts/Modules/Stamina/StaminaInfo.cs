using XPlugin.Data.Json;

namespace GameClient {
	public class StaminaInfo : AutoRestoreValue {
		/// <summary>
		/// 限制值
		/// </summary>
		[JsonIgnore]
		public int LimitValue;

		/// <summary>
		/// 是否达到限制值
		/// </summary>
		[JsonIgnore]
		public bool IsReachLimit {
			get {
				return LimitValue > 0 && NowValue >= LimitValue;
			}
		}

		public StaminaInfo() {
			this.OnChange += _OnChange;
		}

		public StaminaInfo(int maxValue, int restoreTime, int limitValue)
			: base(true, maxValue, restoreTime) {
			this.LimitValue = limitValue == 0 || limitValue >= maxValue ? limitValue : maxValue;
			this.OnChange += _OnChange;
		}

		void _OnChange(int oldValue, int newValue) {
			Client.Stamina.SaveData();
			Client.EventMgr.SendEvent(EventEnum.Player_StaminaChange, oldValue, newValue);
		}

	}
}