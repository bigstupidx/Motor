using UnityEngine;
using Game;
using UnityEngine.EventSystems;

namespace GameUI
{
    public class BtnPause : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
			//游戏中才响应按钮事件
			if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
			{
				return;
			}

			GameModeBase.Ins.Pause();
			PauseDialog.Show();
			//GameOverBoard.Show();
			//RankDialog.Show();
		}
	}


}
