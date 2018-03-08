using GameClient;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI
{
	public class BtnFirstSpree : MonoBehaviour, IPointerClickHandler {
		public void OnPointerClick(PointerEventData eventData) {
			//FirstSpreeDialog.Show(Client.Spree.GetFirstSpreeData());

			CommonDialog.Show ((LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK).ToLocalized(), (LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK_1).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, null, null);
			
		}
	}

}

