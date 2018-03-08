using UnityEngine;
using System.Collections;

namespace GameClient {
	/// <summary>
	/// 物品类型
	/// </summary>
	public enum ItemType {
		/// <summary>
		/// 金币
		/// </summary>
		Coin = 1001,

		/// <summary>
		/// 钻石
		/// </summary>
		Diamond = 1000,

		/// <summary>
		/// 体力
		/// </summary>
		Stamina = 1002,

		/// <summary>
		/// 人民币
		/// </summary>
		RMB = 1004,

		Prop = 6000,
		Hero = 3000,
		Bike = 4000,
		Weapon = 5000


	}

	public enum PropType {
		None = -1,

		Coin = 6666,

		/// <summary>
		/// 随机道具
		/// </summary>
		Random = 6000,

		/// <summary>
		/// 导弹
		/// </summary>
		Missile = 6001,

		/// <summary>
		/// 护盾
		/// </summary>
		Shield = 6002,

		/// <summary>
		/// 氮气能量
		/// </summary>
		Energy = 6005,

	}
}
