using UnityEngine;
using GameClient;
using UnityEngine.EventSystems;

namespace GameUI
{
	public class BtnSpree : MonoBehaviour,IPointerClickHandler
	{

		public ShowType Board = ShowType.BikeBoard;

		public void OnPointerClick(PointerEventData eventData)
		{
			var data = Client.Spree.GetSpreeDataByShowType(Board);
			RMBSpreeDialog.Show(data, false);
		}
	}


}
