using System.Collections.Generic;

namespace GameClient
{
	public class SpreeInfo {

		/// <summary>
		/// 已购买过的计费点（计费点id，次数）
		/// </summary>
		public Dictionary<int, int> AlreadyPay = new Dictionary<int, int>();


		private bool _isGetFirstSpree = false;
		/// <summary>
		/// 是否已经领取首充礼包
		/// </summary>
		public bool IsGetFirstSpree
		{
			get
			{
				return this._isGetFirstSpree;
			}
			set
			{
				this._isGetFirstSpree = value;
				Client.Spree.SaveData();
			}
		}

		private bool _canGetFirstSpree = false;
		/// <summary>
		/// 是否达到领取首充礼包的资格
		/// </summary>
		public bool CanGetFirstSpree
		{
			get
			{
				return this._canGetFirstSpree;
			}
			set
			{
				this._canGetFirstSpree = value;
				Client.Spree.SaveData();
			}
		}
	}

}
