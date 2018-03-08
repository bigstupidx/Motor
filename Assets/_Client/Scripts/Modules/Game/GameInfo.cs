
using System.Collections.Generic;

namespace GameClient
{
	public class GameInfo //TODO 这应该是车辆信息
	{
		/// <summary>
		/// 上场角色
		/// </summary>
		public HeroInfo ChoosedHero;

		/// <summary>
		/// 上场赛车
		/// </summary>
		public BikeInfo ChooseBike;

		/// <summary>
		/// 装备道具
		/// </summary>
		public List<PropInfo> EquipedProps;

		/// <summary>
		/// AI等级
		/// </summary>
		public int AI = 5;

	}

}

